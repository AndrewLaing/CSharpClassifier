using static System.Math;

namespace CollaborativeFiltering
{
    public delegate double SimilarityScore(string categoryA, string categoryB);

    public struct CategoryScore
    {
        public string Name;
        public double Value;
    }

    public struct FeatureScore
    {
        public string Name;
        public double Value;
    }

    public class Recommendations
    {
        private Dictionary<string, Dictionary<string, double>> _dataset;     // category(feature: value, ...), category(feature ... 

        public Recommendations()
        {
            _dataset = new Dictionary<string, Dictionary<string, double>>();
        }

        #region "Dataset management functions"

        public bool ContainsCategory(string categoryName)
        {
            return _dataset.ContainsKey(categoryName);
        }

        public bool ContainsFeatureInCategory(string categoryName, string featureName)
        {
            return _dataset.ContainsKey(categoryName) && _dataset[categoryName].ContainsKey(featureName);
        }

        public void AddCategory(string categoryName)
        {
            if (!ContainsCategory(categoryName))
            {
                _dataset.Add(categoryName, new Dictionary<string, double>());
            }
        }

        public Dictionary<string, Dictionary<string, double>>.KeyCollection GetCategoryNames()
        {
            return _dataset.Keys;
        }

        public Dictionary<string, double>.KeyCollection GetFeatureNamesInCategory(string categoryName)
        {
            if (ContainsCategory(categoryName))
            {
                return _dataset[categoryName].Keys;
            }
            else
            {
                return new Dictionary<string, double>().Keys;
            }
        }

        public void AddValueForFeatureToCategory(string categoryName, string featureName, double rating)
        {
            AddCategory(categoryName);

            if (!_dataset[categoryName].ContainsKey(featureName))
            {
                _dataset[categoryName].Add(featureName, rating);
            }
            else
            {
                _dataset[categoryName][featureName] = rating;
            }
        }

        public double GetValueForFeatureInCategory(string categoryName, string featureName)
        {
            if (ContainsFeatureInCategory(categoryName, featureName))
            {
                return _dataset[categoryName][featureName];
            }
            else
            {
                return 0.0;
            }
        }

        #endregion

        #region "Distance Scoring functions"
        /** 
         * Choosing a distance metric.
         * 
         * Euclidean - Observations with high/low values of features will be grouped together; Groupings are dependant on magnitude.
         * Pearson   - Useful to correct for grade inflation.
         * Tanimoto  - Useful when looking for similarity of feature occurence, or those woth binary values.
         * 
         *        e.g, A bought 1 egg, 1 flour and 1 sugar.
         *             B bought 100 egg, 100 flour and 100 sugar.
         *             C bought 1 egg, 1 Vodka and 1 Red Bull.
         *             Whilst using Euclidean will find that C is more similar to A, Pearson and Tanimoto show B and A as more similar.
         */


        public double EuclideanDistanceScore(string categoryA, string categoryB)
        {
            if (!ContainsCategory(categoryA) || !ContainsCategory(categoryB))
            {
                return 0.0;
            }

            double sumOfSquares = 0.0;
            Dictionary<string, double>.KeyCollection categoryAFeatures = GetFeatureNamesInCategory(categoryA);
            Dictionary<string, double>.KeyCollection categoryBFeatures = GetFeatureNamesInCategory(categoryB);

            foreach (string featureInA in categoryAFeatures)
            {
                foreach (string featureInB in categoryBFeatures)
                {
                    if (featureInA == featureInB)
                    {
                        double valueA = GetValueForFeatureInCategory(categoryA, featureInA);
                        double valueB = GetValueForFeatureInCategory(categoryB, featureInB);
                        sumOfSquares += Pow((valueA - valueB), 2);
                    }
                }
            }

            if (sumOfSquares == 0.0)            // if the 2 categories have no common features
            {
                return 0.0;
            }

            return 1 / (1 + sumOfSquares);
        }

        public double PearsonCorrelationScore(string categoryA, string categoryB)
        {
            if (!ContainsCategory(categoryA) || !ContainsCategory(categoryB))
            {
                return 0.0;
            }

            double sumA = 0.0;                  // sum of mutual item ratings for crategoryA 
            double sumB = 0.0;
            double sumOfSquaresA = 0.0;         // sum of squares of mutual item ratings for critic_1 
            double sumOfSquaresB = 0.0;
            double sumOfProducts = 0.0;         // sum of products of rating by critic_1 for item * those of critic_2 
            int n = 0;                          // number of mutually rated items
            Dictionary<string, double>.KeyCollection categoryAFeatures = GetFeatureNamesInCategory(categoryA);
            Dictionary<string, double>.KeyCollection categoryBFeatures = GetFeatureNamesInCategory(categoryB);

            foreach (string featureInA in categoryAFeatures)
            {
                foreach (string featureInB in categoryBFeatures)
                {
                    if (featureInA == featureInB)
                    {
                        double valueA = GetValueForFeatureInCategory(categoryA, featureInA);
                        double valueB = GetValueForFeatureInCategory(categoryB, featureInB);
                        sumA += valueA;
                        sumB += valueB;
                        sumOfSquaresA += Pow(valueA, 2);
                        sumOfSquaresB += Pow(valueB, 2);
                        sumOfProducts += (valueA * valueB);
                        n++;
                    }
                }
            }

            // if they have no ratings in common
            if (n == 0)
            {
                return 0.0;
            }

            // Calculate PearsonScore
            double numerator = sumOfProducts - (sumA * sumB / n);
            double denominator = Sqrt((sumOfSquaresA - Pow(sumA, 2) / n) * (sumOfSquaresB - Pow(sumB, 2) / n));
            if (denominator == 0)
            {
                return 0.0;
            }
            else
            {
                return numerator / denominator;
            }
        }

        public double TanimotoSimilarityScore(string categoryA, string categoryB)
        {
            if (!ContainsCategory(categoryA) || !ContainsCategory(categoryB))
            {
                return 0.0;
            }

            Dictionary<string, double>.KeyCollection categoryAFeatures = GetFeatureNamesInCategory(categoryA);
            Dictionary<string, double>.KeyCollection categoryBFeatures = GetFeatureNamesInCategory(categoryB);

            int sharedFeaturesCount = 0;
            foreach (string featureInA in categoryAFeatures)
            {
                foreach (string featureInB in categoryBFeatures)
                {
                    if (featureInA == featureInB)
                    {
                        sharedFeaturesCount++;
                    }
                }
            }

            // Calculate TanimotoScore
            double numerator = sharedFeaturesCount;
            double denominator = categoryAFeatures.Count + categoryBFeatures.Count - sharedFeaturesCount;
            if (denominator == 0)
            {
                return 0.0;
            }
            else
            {
                return numerator / denominator;
            }
        }

        #endregion

        #region "Categorisation functions"

        // Recommend n similar categories.
        public List<CategoryScore> TopNCategoryRecommendations(string categoryName, int n, SimilarityScore scoringFunction)
        {
            List<CategoryScore> scoreList = new();
            foreach (string category in GetCategoryNames())
            {
                if (!category.Equals(categoryName))
                {
                    scoreList.Add(new CategoryScore()
                    {
                        Name = category,
                        Value = scoringFunction(categoryName, category)
                    });
                }
            }

            scoreList.Sort((x, y) => x.Value.CompareTo(y.Value));
            scoreList.Reverse();

            if (n < scoreList.Count)
            {
                return scoreList.GetRange(0, n);
            }
            else
            {
                return scoreList;
            }
        }

        // Recommend n features not in category
        public List<FeatureScore> TopNFeatureRecommendations(string categoryName, int n, SimilarityScore scoringFunction)
        {
            List<FeatureScore> rankedFeatures = new();
            Dictionary<string, double> totals = new();
            Dictionary<string, double> simSums = new();

            Dictionary<string, double>.KeyCollection features = GetFeatureNamesInCategory(categoryName);
            double simScore = 0.0;

            foreach (string category in GetCategoryNames())
            {
                if (!category.Equals(categoryName))
                {
                    simScore = scoringFunction(categoryName, category);
                    if (simScore <= 0.0)   // ignore similarity scores of 0 or lower
                    {
                        continue;
                    }

                    foreach (string feature in GetFeatureNamesInCategory(category))
                    {
                        if (!features.Contains(feature))
                        {
                            double valueForFeature = GetValueForFeatureInCategory(category, feature);
                            if (!totals.ContainsKey(feature))
                            {
                                totals[feature] = valueForFeature * simScore;
                            }
                            else
                            {
                                totals[feature] += valueForFeature * simScore;
                            }
                            if (!simSums.ContainsKey(feature))
                            {
                                simSums[feature] = simScore;
                            }
                            else
                            {
                                simSums[feature] += simScore;
                            }
                        }
                    }
                }
            }

            foreach (string feature in totals.Keys)
            {
                rankedFeatures.Add(new FeatureScore()
                {
                    Name = feature,
                    Value = totals[feature] / simSums[feature]
                });
            }

            rankedFeatures.Sort((x, y) => x.Value.CompareTo(y.Value));
            rankedFeatures.Reverse();

            if (n < rankedFeatures.Count)
            {
                return rankedFeatures.GetRange(0, n);
            }
            else
            {
                return rankedFeatures;
            }
        }


        // Recommend n most popular categories for feature.
        // If includePredictions is true, predictions for categories which do not have the feature will be added to result
        public List<CategoryScore> 
            TopNCategoriesForFeature(string featureName, int n, SimilarityScore scoringFunction, bool includePredictions=false)
        {
            List<CategoryScore> scoreList = new();
            foreach (string category in GetCategoryNames())
            {
                bool featureFound = false;
                foreach(string feature in GetFeatureNamesInCategory(category))
                {
                    if(feature == featureName)
                    {
                        scoreList.Add(new CategoryScore()
                        {
                            Name = category,
                            Value = GetValueForFeatureInCategory(category, feature)
                        });
                        featureFound = true;
                        break;
                    }
                }
                if(!featureFound && includePredictions)   // get predicted score for feature and add to list
                {
                    scoreList.Add(new CategoryScore()
                    {
                        Name = category,
                        Value = PredictFeatureValueForCategory(category, featureName, scoringFunction)
                    }) ;
                }
            }

            scoreList.Sort((x, y) => x.Value.CompareTo(y.Value));
            scoreList.Reverse();

            if (n < scoreList.Count)
            {
                return scoreList.GetRange(0, n);
            }
            else
            {
                return scoreList;
            }
        }

        // Predicts the value a category would assign to a feature
        public double PredictFeatureValueForCategory(string categoryName, string featureName, SimilarityScore scoringFunction)
        {
            double total = 0.0;
            double simSum = 0.0;
            double simScore;

            foreach (string category in GetCategoryNames())
            {
                if (!category.Equals(categoryName))
                {
                    simScore = scoringFunction(categoryName, category);
                    if (simScore <= 0.0)   // ignore similarity scores of 0 or lower
                    {
                        continue;
                    }

                    foreach (string candidate in GetFeatureNamesInCategory(category))
                    {
                        if (featureName.Equals(candidate))
                        {
                            double value_for_feature = GetValueForFeatureInCategory(category, candidate);
                            total += value_for_feature * simScore;
                            simSum += simScore;
                            break;
                        }
                    }
                }
            }

            return total / simSum;
        }

        #endregion
    }
}
