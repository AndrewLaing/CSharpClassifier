namespace BayesianClassifier
{
    public class NaiveBayes<T> where T : notnull
    {
        private FeatureDictionary<T> featureCount;
        private CategoryDictionary categoryCount;

        private double weight;
        private double assumedProb;

        private string defaultCategory;

        public NaiveBayes()
        {
            featureCount = new FeatureDictionary<T>();
            categoryCount = new CategoryDictionary();
            weight = 1.0;
            assumedProb = 1.0;
            defaultCategory = "Unclassified";
        }

        public NaiveBayes(double weight, double assumedProb)
        {
            featureCount = new FeatureDictionary<T>();
            categoryCount = new CategoryDictionary();
            this.weight = weight;
            this.assumedProb = assumedProb;
            defaultCategory = "Unclassified";
        }

        public NaiveBayes(string defaultCategory)
        {
            featureCount = new FeatureDictionary<T>();
            categoryCount = new CategoryDictionary();
            weight = 1.0;
            assumedProb = 1.0;
            this.defaultCategory = defaultCategory;
        }

        public NaiveBayes(double weight, double assumedProb, string defaultCategory)
        {
            featureCount = new FeatureDictionary<T>();
            categoryCount = new CategoryDictionary();
            this.weight = weight;
            this.assumedProb = assumedProb;
            this.defaultCategory = defaultCategory;
        }

        /// <summary>
        /// Loads the featureCount and categoryCount dictionaries\n
        /// <para>Name of classifier should be passed without extension (e.g, "nb_foodclassifier")</para>
        /// </summary>
        /// <param name="filename">Path to classifier without extension - e.g, "path/nb" will load both the nb.feature and nb.category files.</param>
        public void LoadClassifier(string filename)
        {
            JSONSerialise.LoadObject(filename + ".feature", ref featureCount);
            JSONSerialise.LoadObject(filename + ".category", ref categoryCount);
        }

        /// <summary>
        /// Saves the featureCount and categoryCount dictionaries
        /// <para> Name of classifier should be passed without extension (e.g, "nb_foodclassifier")</para>
        /// </summary>
        /// <param name="filename">Save path for classifier without extension - e.g, "path/nb" will create both the nb.feature and nb.category files.</param>
        public void SaveClassifier(string filename)
        {
            JSONSerialise.SaveObject(filename + ".feature", featureCount);
            JSONSerialise.SaveObject(filename + ".category", categoryCount);
        }

        public void SetAssumedProb(double assumedProb)
        {
            this.assumedProb = assumedProb;
        }

        public void SetWeight(double weight)
        {
            this.weight = weight;
        }

        public void SetDefaultCategory(string defaultCategory)
        {
            this.defaultCategory = defaultCategory;
        }

        private void IncrementFC(T feature, string category)
        {
            featureCount.IncrementValue(feature, category);
        }

        private void IncrementCC(string category)
        {
            categoryCount.IncrementValue(category);
        }

        private int GetFeaturesCategoryValue(T feature, string category)
        {
            return featureCount.GetCategoryValue(feature, category);
        }

        private int GetCategoryValue(string category)
        {
            return categoryCount.GetValue(category);
        }

        private int GetCategoriesSum()
        {
            return categoryCount.GetSumOfValues();
        }

        private List<string> GetCategoryKeys()
        {
            return categoryCount.GetCategories();
        }

        public void Train(List<T> features, string category)
        {
            foreach (T feature in features)
            {
                IncrementFC(feature, category);
            }

            IncrementCC(category);
        }

        public void Train(T feature, string category)
        {
            IncrementFC(feature, category);
            IncrementCC(category);
        }

        /// <summary>
        /// Returns the probability of a feature appearing in a category
        /// </summary>
        private double FeatureProbability(T feature, string category)
        {
            if (GetCategoryValue(category) == 0)
            {
                return 0.0;
            }
            else
            {
                return GetFeaturesCategoryValue(feature, category) / (double)GetCategoryValue(category);
            }
        }

        /// <summary>
        /// Returns the weighted probability of a feature appearing in a category
        /// </summary>
        private double WeightedProbability(T feature, string category)
        {
            double basicProbability = FeatureProbability(feature, category);
            double total = featureCount.GetSumOfAllValues(feature);

            return ((weight * assumedProb) * (total * basicProbability)) / (weight + total);
        }

        /// <summary>
        /// Returns the probabilty of the category existing in all features
        /// </summary>
        private double CommonAppearanceProbability(List<T> features, string category)
        {
            double probability = 1.0;

            foreach (T feature in features)
            {
                probability *= WeightedProbability(feature, category);
            }

            return probability;
        }

        /// <summary>
        /// Returns the probability for the category passed
        /// </summary>
        private double Probability(List<T> features, string category)
        {

            double categoryProbability = GetCategoryValue(category) / (double)GetCategoriesSum();
            double commonAppearanceProbabilty = CommonAppearanceProbability(features, category);

            return categoryProbability * commonAppearanceProbabilty;
        }

        public string Classify(List<T> features)
        {
            double max = 0.0;
            double categoryProbability = 0.0;
            string best = defaultCategory;

            List<string> categories = GetCategoryKeys();
            foreach (string category in categories)
            {
                categoryProbability = Probability(features, category);
                if (categoryProbability > max)
                {
                    max = categoryProbability;
                    best = category;
                }
            }

            return best;
        }
    }
}
