import { CohereClient } from 'cohere-ai';

// Initialize Cohere client
const cohere = new CohereClient({
  token: 'KD2nozebMtBawxWruHYQe6Z2oZ4IQn4YhugsQNdU', // Your API key
});

/**
 * Content moderation function using Cohere's moderate endpoint
 * @param input - The text content to moderate
 * @returns Promise<"Safe" | "Unsafe"> - Returns "Safe" if content is appropriate, "Unsafe" if flagged
 */
export async function moderateContent(input: string): Promise<"Safe" | "Unsafe"> {
  try {
    // Validate input
    if (!input || typeof input !== 'string') {
      console.warn('Invalid input provided to moderateContent');
      return "Safe"; // Default to safe for invalid inputs
    }

    // Trim and check if input is empty
    const trimmedInput = input.trim();
    if (trimmedInput.length === 0) {
      return "Safe";
    }

    // Call Cohere's moderate endpoint
    const response = await cohere.moderate({
      text: trimmedInput,
    });

    // Check if any flags were raised
    const flags = response.results[0]?.flagged;
    
    if (flags) {
      console.log('Content flagged as inappropriate:', {
        input: trimmedInput.substring(0, 100) + '...',
        categories: response.results[0]?.categories,
        categoryScores: response.results[0]?.categoryScores
      });
      return "Unsafe";
    }

    return "Safe";
  } catch (error) {
    console.error('Error in content moderation:', error);
    
    // Handle specific Cohere API errors
    if (error instanceof Error) {
      if (error.message.includes('401') || error.message.includes('Unauthorized')) {
        console.error('Cohere API authentication failed. Check your API key.');
      } else if (error.message.includes('429') || error.message.includes('Rate limit')) {
        console.error('Cohere API rate limit exceeded.');
      } else if (error.message.includes('500') || error.message.includes('Internal server error')) {
        console.error('Cohere API internal server error.');
      }
    }
    
    // In case of API failure, you might want to:
    // 1. Return "Unsafe" to be conservative
    // 2. Return "Safe" to avoid blocking legitimate content
    // 3. Implement a fallback moderation service
    
    // For now, returning "Safe" to avoid blocking legitimate content during API issues
    return "Safe";
  }
}

/**
 * Enhanced content moderation with detailed results
 * @param input - The text content to moderate
 * @returns Promise<ModerationResult> - Detailed moderation results
 */
export async function moderateContentDetailed(input: string): Promise<ModerationResult> {
  try {
    if (!input || typeof input !== 'string') {
      return {
        isSafe: true,
        flagged: false,
        categories: {},
        categoryScores: {},
        error: 'Invalid input provided'
      };
    }

    const trimmedInput = input.trim();
    if (trimmedInput.length === 0) {
      return {
        isSafe: true,
        flagged: false,
        categories: {},
        categoryScores: {},
        error: null
      };
    }

    const response = await cohere.moderate({
      text: trimmedInput,
    });

    const result = response.results[0];
    
    return {
      isSafe: !result?.flagged,
      flagged: result?.flagged || false,
      categories: result?.categories || {},
      categoryScores: result?.categoryScores || {},
      error: null
    };
  } catch (error) {
    console.error('Error in detailed content moderation:', error);
    
    return {
      isSafe: true, // Default to safe during errors
      flagged: false,
      categories: {},
      categoryScores: {},
      error: error instanceof Error ? error.message : 'Unknown error'
    };
  }
}

/**
 * Batch content moderation for multiple inputs
 * @param inputs - Array of text content to moderate
 * @returns Promise<ModerationResult[]> - Array of moderation results
 */
export async function moderateContentBatch(inputs: string[]): Promise<ModerationResult[]> {
  const results: ModerationResult[] = [];
  
  for (const input of inputs) {
    const result = await moderateContentDetailed(input);
    results.push(result);
  }
  
  return results;
}

// Type definitions
export interface ModerationResult {
  isSafe: boolean;
  flagged: boolean;
  categories: Record<string, boolean>;
  categoryScores: Record<string, number>;
  error: string | null;
}

// Example usage
export async function exampleUsage() {
  console.log('=== Content Moderation Examples ===\n');

  // Example 1: Safe content
  const safeContent = "This is a beautiful handmade wooden table for sale.";
  const safeResult = await moderateContent(safeContent);
  console.log('Safe content example:');
  console.log(`Input: "${safeContent}"`);
  console.log(`Result: ${safeResult}\n`);

  // Example 2: Potentially unsafe content
  const unsafeContent = "This product contains explicit adult content and violence.";
  const unsafeResult = await moderateContent(unsafeContent);
  console.log('Unsafe content example:');
  console.log(`Input: "${unsafeContent}"`);
  console.log(`Result: ${unsafeResult}\n`);

  // Example 3: Detailed moderation
  const detailedResult = await moderateContentDetailed(unsafeContent);
  console.log('Detailed moderation example:');
  console.log(`Input: "${unsafeContent}"`);
  console.log(`Is Safe: ${detailedResult.isSafe}`);
  console.log(`Flagged: ${detailedResult.flagged}`);
  console.log(`Categories:`, detailedResult.categories);
  console.log(`Category Scores:`, detailedResult.categoryScores);
  console.log(`Error: ${detailedResult.error}\n`);

  // Example 4: Batch moderation
  const batchInputs = [
    "Handmade jewelry collection",
    "Professional photography services",
    "Explicit adult content here",
    "Beautiful ceramic vase"
  ];
  
  console.log('Batch moderation example:');
  const batchResults = await moderateContentBatch(batchInputs);
  batchResults.forEach((result, index) => {
    console.log(`Input ${index + 1}: "${batchInputs[index]}"`);
    console.log(`Result: ${result.isSafe ? 'Safe' : 'Unsafe'}`);
    console.log(`Flagged: ${result.flagged}\n`);
  });
}

// Error handling examples
export async function errorHandlingExamples() {
  console.log('=== Error Handling Examples ===\n');

  // Example 1: Invalid input
  const invalidResult = await moderateContent('');
  console.log('Empty input result:', invalidResult);

  // Example 2: Very long input
  const longInput = 'A'.repeat(10000);
  const longResult = await moderateContent(longInput);
  console.log('Long input result:', longResult);

  // Example 3: Special characters
  const specialChars = '🚀🌟✨🎉🎊🎋🎌🎍🎎🎏🎐🎑🎒🎓🎔🎕🎖🎗🎘🎙🎚🎛🎜🎝🎞🎟';
  const specialResult = await moderateContent(specialChars);
  console.log('Special characters result:', specialResult);
}

// Integration with your existing .NET backend
export interface ProductModerationRequest {
  title: string;
  description: string;
  sellerId: string;
  productId?: number;
}

export interface ProductModerationResponse {
  isSafe: boolean;
  titleSafe: boolean;
  descriptionSafe: boolean;
  flaggedCategories: string[];
  error?: string;
}

/**
 * Moderate a product's title and description
 * @param product - Product information to moderate
 * @returns Promise<ProductModerationResponse> - Moderation results for the product
 */
export async function moderateProduct(product: ProductModerationRequest): Promise<ProductModerationResponse> {
  try {
    // Moderate title and description separately
    const titleResult = await moderateContentDetailed(product.title);
    const descriptionResult = await moderateContentDetailed(product.description);

    // Collect all flagged categories
    const flaggedCategories: string[] = [];
    
    Object.entries(titleResult.categories).forEach(([category, flagged]) => {
      if (flagged) flaggedCategories.push(`title_${category}`);
    });
    
    Object.entries(descriptionResult.categories).forEach(([category, flagged]) => {
      if (flagged) flaggedCategories.push(`description_${category}`);
    });

    const isSafe = titleResult.isSafe && descriptionResult.isSafe;

    return {
      isSafe,
      titleSafe: titleResult.isSafe,
      descriptionSafe: descriptionResult.isSafe,
      flaggedCategories,
      error: null
    };
  } catch (error) {
    console.error('Error moderating product:', error);
    
    return {
      isSafe: false, // Default to unsafe during errors
      titleSafe: false,
      descriptionSafe: false,
      flaggedCategories: ['error'],
      error: error instanceof Error ? error.message : 'Unknown error'
    };
  }
}

// Run examples if this file is executed directly
if (require.main === module) {
  exampleUsage()
    .then(() => errorHandlingExamples())
    .then(() => {
      console.log('=== Product Moderation Example ===\n');
      return moderateProduct({
        title: "Handmade Wooden Coffee Table",
        description: "Beautiful handcrafted coffee table made from premium oak wood. Perfect for living room decoration.",
        sellerId: "user123"
      });
    })
    .then((productResult) => {
      console.log('Product moderation result:', productResult);
    })
    .catch(console.error);
} 