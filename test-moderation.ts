import { moderateContent, moderateContentDetailed, moderateProduct } from './content-moderation';

async function runTests() {
  console.log('🚀 Starting Content Moderation Tests\n');

  // Test 1: Basic moderation
  console.log('📝 Test 1: Basic Content Moderation');
  const testCases = [
    "Beautiful handmade wooden table for sale",
    "Professional photography services for weddings",
    "This product contains explicit adult content and violence",
    "Handcrafted jewelry made with love",
    "Explicit sexual content here",
    "Hate speech and violence in this description"
  ];

  for (const testCase of testCases) {
    const result = await moderateContent(testCase);
    console.log(`"${testCase}" → ${result}`);
  }
  console.log('');

  // Test 2: Detailed moderation
  console.log('🔍 Test 2: Detailed Content Moderation');
  const detailedTest = "This product contains explicit adult content and violence";
  const detailedResult = await moderateContentDetailed(detailedTest);
  console.log(`Input: "${detailedTest}"`);
  console.log(`Is Safe: ${detailedResult.isSafe}`);
  console.log(`Flagged: ${detailedResult.flagged}`);
  console.log(`Categories:`, detailedResult.categories);
  console.log(`Category Scores:`, detailedResult.categoryScores);
  console.log('');

  // Test 3: Product moderation
  console.log('🛍️ Test 3: Product Moderation');
  const productTests = [
    {
      title: "Handmade Wooden Coffee Table",
      description: "Beautiful handcrafted coffee table made from premium oak wood. Perfect for living room decoration.",
      sellerId: "user123"
    },
    {
      title: "Professional Camera Equipment",
      description: "High-quality DSLR camera with lenses for professional photography. Great for weddings and events.",
      sellerId: "photographer456"
    },
    {
      title: "Explicit Adult Content",
      description: "This product contains explicit sexual content and adult material that is inappropriate.",
      sellerId: "user789"
    }
  ];

  for (const product of productTests) {
    const result = await moderateProduct(product);
    console.log(`Product: "${product.title}"`);
    console.log(`Overall Safe: ${result.isSafe}`);
    console.log(`Title Safe: ${result.titleSafe}`);
    console.log(`Description Safe: ${result.descriptionSafe}`);
    console.log(`Flagged Categories: ${result.flaggedCategories.join(', ') || 'None'}`);
    console.log('');
  }

  // Test 4: Error handling
  console.log('⚠️ Test 4: Error Handling');
  const errorTests = [
    "", // Empty string
    "   ", // Whitespace only
    null as any, // Null input
    undefined as any, // Undefined input
    "A".repeat(10000) // Very long input
  ];

  for (const errorTest of errorTests) {
    const result = await moderateContent(errorTest);
    console.log(`Input: ${JSON.stringify(errorTest)} → ${result}`);
  }

  console.log('\n✅ All tests completed!');
}

// Run tests if this file is executed directly
if (require.main === module) {
  runTests().catch(console.error);
}

export { runTests }; 