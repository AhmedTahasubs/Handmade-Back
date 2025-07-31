import express from 'express';
import cors from 'cors';
import { moderateContent, moderateContentDetailed, moderateProduct } from './content-moderation';

const app = express();
const PORT = process.env.PORT || 3000;

// Middleware
app.use(cors());
app.use(express.json());

// Health check endpoint
app.get('/health', (req, res) => {
  res.json({ status: 'OK', timestamp: new Date().toISOString() });
});

// Simple content moderation endpoint
app.post('/moderate', async (req, res) => {
  try {
    const { text } = req.body;
    
    if (!text || typeof text !== 'string') {
      return res.status(400).json({ 
        error: 'Text field is required and must be a string' 
      });
    }

    const result = await moderateContent(text);
    res.json({ 
      text: text.substring(0, 100) + (text.length > 100 ? '...' : ''),
      result,
      timestamp: new Date().toISOString()
    });
  } catch (error) {
    console.error('Error in /moderate endpoint:', error);
    res.status(500).json({ 
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error'
    });
  }
});

// Detailed content moderation endpoint
app.post('/moderate-detailed', async (req, res) => {
  try {
    const { text } = req.body;
    
    if (!text || typeof text !== 'string') {
      return res.status(400).json({ 
        error: 'Text field is required and must be a string' 
      });
    }

    const result = await moderateContentDetailed(text);
    res.json({ 
      text: text.substring(0, 100) + (text.length > 100 ? '...' : ''),
      ...result,
      timestamp: new Date().toISOString()
    });
  } catch (error) {
    console.error('Error in /moderate-detailed endpoint:', error);
    res.status(500).json({ 
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error'
    });
  }
});

// Product moderation endpoint
app.post('/moderate-product', async (req, res) => {
  try {
    const { title, description, sellerId, productId } = req.body;
    
    if (!title || !description || typeof title !== 'string' || typeof description !== 'string') {
      return res.status(400).json({ 
        error: 'Title and description fields are required and must be strings' 
      });
    }

    const result = await moderateProduct({
      title,
      description,
      sellerId: sellerId || 'unknown',
      productId
    });

    res.json({ 
      title: title.substring(0, 50) + (title.length > 50 ? '...' : ''),
      description: description.substring(0, 100) + (description.length > 100 ? '...' : ''),
      ...result,
      timestamp: new Date().toISOString()
    });
  } catch (error) {
    console.error('Error in /moderate-product endpoint:', error);
    res.status(500).json({ 
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error'
    });
  }
});

// Batch moderation endpoint
app.post('/moderate-batch', async (req, res) => {
  try {
    const { texts } = req.body;
    
    if (!Array.isArray(texts) || texts.length === 0) {
      return res.status(400).json({ 
        error: 'Texts field is required and must be a non-empty array' 
      });
    }

    if (texts.length > 10) {
      return res.status(400).json({ 
        error: 'Maximum 10 texts allowed per batch request' 
      });
    }

    const results = await Promise.all(
      texts.map(async (text, index) => {
        if (typeof text !== 'string') {
          return {
            index,
            error: 'Text must be a string',
            isSafe: true,
            flagged: false
          };
        }
        
        const result = await moderateContentDetailed(text);
        return {
          index,
          text: text.substring(0, 50) + (text.length > 50 ? '...' : ''),
          ...result
        };
      })
    );

    res.json({ 
      results,
      count: results.length,
      timestamp: new Date().toISOString()
    });
  } catch (error) {
    console.error('Error in /moderate-batch endpoint:', error);
    res.status(500).json({ 
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error'
    });
  }
});

// Start server
app.listen(PORT, () => {
  console.log(`🚀 Content Moderation Server running on http://localhost:${PORT}`);
  console.log(`📋 Available endpoints:`);
  console.log(`   GET  /health - Health check`);
  console.log(`   POST /moderate - Simple moderation`);
  console.log(`   POST /moderate-detailed - Detailed moderation`);
  console.log(`   POST /moderate-product - Product moderation`);
  console.log(`   POST /moderate-batch - Batch moderation`);
  console.log(`\n🔧 Test with: npm run test-server`);
});

export default app; 