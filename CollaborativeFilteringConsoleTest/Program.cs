// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using CollaborativeFiltering;
using static System.Math;

static void addTestData(Recommendations data)
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

static void testCategoryRecommendation(Recommendations data, string category, SimilarityScore scoringFunction, string header)
{
    List<CategoryScore> matches = data.TopNCategoryRecommendations(category, 3, scoringFunction);

    Console.WriteLine(" ");
    Console.WriteLine(" ");
    Console.WriteLine(header);

    foreach (CategoryScore match in matches)
    {
        Console.WriteLine(match.name + " " + match.value);
    }
}

static void testFeatureRecommendation(Recommendations data, string category, SimilarityScore scoringFunction, string header)
{
    List<CategoryScore> matches = data.TopNCategoryRecommendations(category, 3, scoringFunction);

    Console.WriteLine(" ");
    Console.WriteLine(" ");
    Console.WriteLine(header);

    foreach (FeatureScore score in data.TopNFeatureRecommendations("Toby", 3, scoringFunction))
    {
        Console.WriteLine(score.name + " " + score.value);
    }
}


static void testCategoriesForFeatureRecommendation(Recommendations data, string category, SimilarityScore scoringFunction, string header)
{
    List<CategoryScore> matches = data.TopNCategoriesForFeature(category, 100, scoringFunction, true);

    Console.WriteLine(" ");
    Console.WriteLine(" ");
    Console.WriteLine(header);

    foreach (CategoryScore match in matches)
    {
        Console.WriteLine(match.name + " " + match.value);
    }
}

Recommendations data = new();
addTestData(data);

SimilarityScore scoringFunction = new SimilarityScore(data.PearsonCorrelationScore);
SimilarityScore scoringFunction2 = new SimilarityScore(data.EuclideanDistanceScore);
SimilarityScore scoringFunction3 = new SimilarityScore(data.TanimotoSimilarityScore);

testCategoryRecommendation(data, "Toby", scoringFunction, "==== Top 3 Pearson Category Matches ====");
testCategoryRecommendation(data, "Toby", scoringFunction2, "==== Top 3 Category Euclidean Matches ====");
testCategoryRecommendation(data, "Toby", scoringFunction3, "==== Top 3 Category Tanimoto Matches ====");

testFeatureRecommendation(data, "Toby", scoringFunction, "==== Top 3 Feature Pearson Matches ====");
testFeatureRecommendation(data, "Toby", scoringFunction2, "==== Top 3 Feature Euclidean Matches ====");
testFeatureRecommendation(data, "Toby", scoringFunction3, "==== Top 3 Feature Tanimoto Matches ====");

testCategoriesForFeatureRecommendation(data, "The Night Listener", scoringFunction, "==== Top Pearson Category for feature Matches ====");
testCategoriesForFeatureRecommendation(data, "The Night Listener", scoringFunction2, "==== Top Category for feature Euclidean Matches ====");
testCategoriesForFeatureRecommendation(data, "The Night Listener", scoringFunction3, "==== Top Category for feature Tanimoto Matches ====");
