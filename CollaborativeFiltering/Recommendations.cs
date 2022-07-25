using static System.Math;

namespace CollaborativeFiltering
{
    public delegate double SimilarityScore(string category_1, string category_2);

    public struct CategoryScore
    {
        public string name;
        public double value;
    }

    public struct FeatureScore
    {
        public string name;
        public double value;
    }

    public class Recommendations
    {
        private Dictionary<string, Dictionary<string, double>> dataset;     // category(feature: value, ...), category(feature ... 

        public Recommendations()
        {
            dataset = new Dictionary<string, Dictionary<string, double>>();
        }

        #region "Dataset management functions"

        public bool ContainsCategory(string category_name)
        {
            return dataset.ContainsKey(category_name);
        }

        public bool ContainsFeatureInCategory(string category_name, string feature_name)
        {
            return dataset.ContainsKey(category_name) && dataset[category_name].ContainsKey(feature_name);
        }

        public void AddCategory(string category_name)
        {
            if (!ContainsCategory(category_name))
            {
                dataset.Add(category_name, new Dictionary<string, double>());
            }
        }

        public Dictionary<string, Dictionary<string, double>>.KeyCollection GetCategoryNames()
        {
            return dataset.Keys;
        }

        public Dictionary<string, double>.KeyCollection GetFeatureNamesInCategory(string category_name)
        {
            if (ContainsCategory(category_name))
            {
                return dataset[category_name].Keys;
            }
            else
            {
                return new Dictionary<string, double>().Keys;
            }
        }

        public void AddValueForFeatureToCategory(string category_name, string feature_name, double rating)
        {
            AddCategory(category_name);

            if (!dataset[category_name].ContainsKey(feature_name))
            {
                dataset[category_name].Add(feature_name, rating);
            }
            else
            {
                dataset[category_name][feature_name] = rating;
            }
        }

        public double GetValueForFeatureInCategory(string category_name, string feature_name)
        {
            if (ContainsFeatureInCategory(category_name, feature_name))
            {
                return dataset[category_name][feature_name];
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
         *        e.g, A bought 1x eggs, 1x flour and 1x sugar.
         *             B bought 100x eggs, 100x flour and 100x sugar.
         *             C bought 1x eggs, 1x Vodka and 1x Red Bull.
         *             Whilst using Euclidean will find that C is more similar to A, Pearson and Tanimoto show B and A as more similar.
         */


        public double EuclideanDistanceScore(string category_1, string category_2)
        {
            if (!ContainsCategory(category_1) || !ContainsCategory(category_2))
            {
                return 0.0;
            }

            double sum_of_squares = 0.0;
            Dictionary<string, double>.KeyCollection critic_1_movies = GetFeatureNamesInCategory(category_1);
            Dictionary<string, double>.KeyCollection critic_2_movies = GetFeatureNamesInCategory(category_2);

            foreach (string key1 in critic_1_movies)
            {
                foreach (string key2 in critic_2_movies)
                {
                    if (key1 == key2)
                    {
                        double rating_c1 = GetValueForFeatureInCategory(category_1, key1);
                        double rating_c2 = GetValueForFeatureInCategory(category_2, key2);
                        sum_of_squares += Pow((rating_c1 - rating_c2), 2);
                    }
                }
            }

            // if they have no ratings in common
            if (sum_of_squares == 0.0)
            {
                return 0.0;
            }

            return 1 / (1 + sum_of_squares);
        }

        public double PearsonCorrelationScore(string category_1, string category_2)
        {
            if (!ContainsCategory(category_1) || !ContainsCategory(category_2))
            {
                return 0.0;
            }

            double sum_c1 = 0.0;                // sum of mutual item ratings for critic_1 
            double sum_c2 = 0.0;
            double sum_of_squares_c1 = 0.0;     // sum of squares of mutual item ratings for critic_1 
            double sum_of_squares_c2 = 0.0;
            double sum_of_products = 0.0;       // sum of products of rating by critic_1 for item * those of critic_2 
            int n = 0;                          // number of mutually rated items
            Dictionary<string, double>.KeyCollection critic_1_movies = GetFeatureNamesInCategory(category_1);
            Dictionary<string, double>.KeyCollection critic_2_movies = GetFeatureNamesInCategory(category_2);

            foreach (string key1 in critic_1_movies)
            {
                foreach (string key2 in critic_2_movies)
                {
                    if (key1 == key2)
                    {
                        double rating_c1 = GetValueForFeatureInCategory(category_1, key1);
                        double rating_c2 = GetValueForFeatureInCategory(category_2, key2);
                        sum_c1 += rating_c1;
                        sum_c2 += rating_c2;
                        sum_of_squares_c1 += Pow(rating_c1, 2);
                        sum_of_squares_c2 += Pow(rating_c2, 2);
                        sum_of_products += (rating_c1 * rating_c2);
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
            double numerator = sum_of_products - (sum_c1 * sum_c2 / n);
            double denominator = Sqrt((sum_of_squares_c1 - Pow(sum_c1, 2) / n) * (sum_of_squares_c2 - Pow(sum_c2, 2) / n));
            if (denominator == 0)
            {
                return 0.0;
            }
            else
            {
                return numerator / denominator;
            }
        }

        public double TanimotoSimilarityScore(string category_1, string category_2)
        {
            if (!ContainsCategory(category_1) || !ContainsCategory(category_2))
            {
                return 0.0;
            }

            Dictionary<string, double>.KeyCollection critic_1_movies = GetFeatureNamesInCategory(category_1);
            Dictionary<string, double>.KeyCollection critic_2_movies = GetFeatureNamesInCategory(category_2);

            int Na = critic_1_movies.Count;
            int Nb = critic_2_movies.Count;
            int Nc = 0;

            foreach (string key1 in critic_1_movies)
            {
                foreach (string key2 in critic_2_movies)
                {
                    if (key1 == key2)
                    {
                        Nc++;
                    }
                }
            }

            // Calculate TanimotoScore
            double numerator = Nc;
            double denominator = Na + Nb - Nc;
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
        public List<CategoryScore> TopNCategoryRecommendations(string category_name, int n, SimilarityScore ScoringFunction)
        {
            List<CategoryScore> criticScores = new List<CategoryScore>();
            foreach (string key in GetCategoryNames())
            {
                if (!key.Equals(category_name))
                {
                    criticScores.Add(new CategoryScore()
                    {
                        name = key,
                        value = ScoringFunction(category_name, key)
                    });
                }
            }

            criticScores.Sort((x, y) => x.value.CompareTo(y.value));
            criticScores.Reverse();

            if (n < criticScores.Count)
            {
                return criticScores.GetRange(0, n);
            }
            else
            {
                return criticScores;
            }
        }

        // Recommend n features not in category
        public List<FeatureScore> TopNFeatureRecommendations(string category_name, int n, SimilarityScore ScoringFunction)
        {
            List<FeatureScore> rankings = new();
            Dictionary<string, double> totals = new();
            Dictionary<string, double> sim_sums = new();

            Dictionary<string, double>.KeyCollection features = GetFeatureNamesInCategory(category_name);
            double sim_score = 0.0;

            foreach (string key in GetCategoryNames())
            {
                if (!key.Equals(category_name))
                {
                    sim_score = ScoringFunction(category_name, key);
                    if (sim_score <= 0.0)   // ignore similarity scores of 0 or lower
                    {
                        continue;
                    }

                    foreach (string candidate in GetFeatureNamesInCategory(key))
                    {
                        if (!features.Contains(candidate))
                        {
                            double value_for_feature = GetValueForFeatureInCategory(key, candidate);
                            if (!totals.ContainsKey(candidate))
                            {
                                totals[candidate] = (value_for_feature * sim_score);
                            }
                            else
                            {
                                totals[candidate] += (value_for_feature * sim_score);
                            }
                            if (!sim_sums.ContainsKey(candidate))
                            {
                                sim_sums[candidate] = sim_score;
                            }
                            else
                            {
                                sim_sums[candidate] += sim_score;
                            }
                        }
                    }
                }
            }

            foreach (string fName in totals.Keys)
            {
                rankings.Add(new FeatureScore()
                {
                    name = fName,
                    value = totals[fName] / sim_sums[fName]
                });
            }

            rankings.Sort((x, y) => x.value.CompareTo(y.value));
            rankings.Reverse();

            if (n < rankings.Count)
            {
                return rankings.GetRange(0, n);
            }
            else
            {
                return rankings;
            }
        }

        #endregion
    }
}