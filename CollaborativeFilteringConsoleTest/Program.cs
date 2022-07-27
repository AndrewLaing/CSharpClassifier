// See https://aka.ms/new-console-template for more information
using CollaborativeFiltering;

static void AddTestData(Recommendations data)
{
    data.AddCategory("Lisa Rose");
    data.AddCategory("Gene Seymour");
    data.AddCategory("Michael Phillips");
    data.AddCategory("Claudia Puig");
    data.AddCategory("Mick LaSalle");
    data.AddCategory("Jack Matthews");
    data.AddCategory("Toby");

    data.AddValueForFeatureToCategory("Lisa Rose", "Lady in the Water", 2.5);
    data.AddValueForFeatureToCategory("Lisa Rose", "Snakes on a Plane", 3.5);
    data.AddValueForFeatureToCategory("Lisa Rose", "Just My Luck", 3.0);
    data.AddValueForFeatureToCategory("Lisa Rose", "Superman Returns", 3.5);
    data.AddValueForFeatureToCategory("Lisa Rose", "You, Me and Dupree", 2.5);
    data.AddValueForFeatureToCategory("Lisa Rose", "The Night Listener", 3.0);

    data.AddValueForFeatureToCategory("Gene Seymour", "Lady in the Water", 3.0);
    data.AddValueForFeatureToCategory("Gene Seymour", "Snakes on a Plane", 3.5);
    data.AddValueForFeatureToCategory("Gene Seymour", "Just My Luck", 1.5);
    data.AddValueForFeatureToCategory("Gene Seymour", "Superman Returns", 5.0);
    data.AddValueForFeatureToCategory("Gene Seymour", "You, Me and Dupree", 3.5);
    data.AddValueForFeatureToCategory("Gene Seymour", "The Night Listener", 3.0);

    data.AddValueForFeatureToCategory("Michael Phillips", "Lady in the Water", 2.5);
    data.AddValueForFeatureToCategory("Michael Phillips", "Snakes on a Plane", 3.0);
    data.AddValueForFeatureToCategory("Michael Phillips", "Superman Returns", 3.5);
    data.AddValueForFeatureToCategory("Michael Phillips", "The Night Listener", 4.0);

    data.AddValueForFeatureToCategory("Claudia Puig", "Snakes on a Plane", 3.5);
    data.AddValueForFeatureToCategory("Claudia Puig", "Just My Luck", 3.0);
    data.AddValueForFeatureToCategory("Claudia Puig", "Superman Returns", 4.0);
    data.AddValueForFeatureToCategory("Claudia Puig", "You, Me and Dupree", 2.5);
    data.AddValueForFeatureToCategory("Claudia Puig", "The Night Listener", 4.5);

    data.AddValueForFeatureToCategory("Mick LaSalle", "Lady in the Water", 3.0);
    data.AddValueForFeatureToCategory("Mick LaSalle", "Snakes on a Plane", 4.0);
    data.AddValueForFeatureToCategory("Mick LaSalle", "Just My Luck", 2.0);
    data.AddValueForFeatureToCategory("Mick LaSalle", "Superman Returns", 3.0);
    data.AddValueForFeatureToCategory("Mick LaSalle", "You, Me and Dupree", 2.0);
    data.AddValueForFeatureToCategory("Mick LaSalle", "The Night Listener", 3.0);

    data.AddValueForFeatureToCategory("Jack Matthews", "Lady in the Water", 3.0);
    data.AddValueForFeatureToCategory("Jack Matthews", "Snakes on a Plane", 4.0);
    data.AddValueForFeatureToCategory("Jack Matthews", "Superman Returns", 5.0);
    data.AddValueForFeatureToCategory("Jack Matthews", "You, Me and Dupree", 3.5);
    data.AddValueForFeatureToCategory("Jack Matthews", "The Night Listener", 3.0);

    data.AddValueForFeatureToCategory("Toby", "Snakes on a Plane", 4.5);
    data.AddValueForFeatureToCategory("Toby", "Superman Returns", 4.0);
    data.AddValueForFeatureToCategory("Toby", "You, Me and Dupree", 1.0);
}

static void TestCategoryRecommendation(Recommendations data, string category, SimilarityScore scoringFunction, string header)
{
    List<CategoryScore> matches = data.TopNCategoryRecommendations(category, 3, scoringFunction);

    Console.WriteLine("\n\n" + header);

    foreach (CategoryScore match in matches)
    {
        Console.WriteLine(match.Name + " " + match.Value);
    }
}

static void TestFeatureRecommendation(Recommendations data, string category, SimilarityScore scoringFunction, string header)
{
    List<CategoryScore> matches = data.TopNCategoryRecommendations(category, 3, scoringFunction);

    Console.WriteLine("\n\n" + header);

    foreach (FeatureScore score in data.TopNFeatureRecommendations("Toby", 3, scoringFunction))
    {
        Console.WriteLine(score.Name + " " + score.Value);
    }
}


static void TestCategoriesForFeatureRecommendation(Recommendations data, string category, SimilarityScore scoringFunction, string header)
{
    List<CategoryScore> matches = data.TopNCategoriesForFeature(category, 100, scoringFunction, true);

    Console.WriteLine("\n\n" + header);

    foreach (CategoryScore match in matches)
    {
        Console.WriteLine(match.Name + " " + match.Value);
    }
}

Recommendations data = new();
AddTestData(data);

SimilarityScore pearsonScoring = new(data.PearsonCorrelationScore);
SimilarityScore euclideanScoring = new(data.EuclideanDistanceScore);
SimilarityScore tanimotoScoring = new(data.TanimotoSimilarityScore);

TestCategoryRecommendation(data, "Toby", pearsonScoring, "==== Top 3 Pearson Category Matches ====");
TestCategoryRecommendation(data, "Toby", euclideanScoring, "==== Top 3 Category Euclidean Matches ====");
TestCategoryRecommendation(data, "Toby", tanimotoScoring, "==== Top 3 Category Tanimoto Matches ====");

TestFeatureRecommendation(data, "Toby", pearsonScoring, "==== Top 3 Feature Pearson Matches ====");
TestFeatureRecommendation(data, "Toby", euclideanScoring, "==== Top 3 Feature Euclidean Matches ====");
TestFeatureRecommendation(data, "Toby", tanimotoScoring, "==== Top 3 Feature Tanimoto Matches ====");

TestCategoriesForFeatureRecommendation(data, "The Night Listener", pearsonScoring, "==== Top Pearson Category for feature Matches ====");
TestCategoriesForFeatureRecommendation(data, "The Night Listener", euclideanScoring, "==== Top Category for feature Euclidean Matches ====");
TestCategoriesForFeatureRecommendation(data, "The Night Listener", tanimotoScoring, "==== Top Category for feature Tanimoto Matches ====");
