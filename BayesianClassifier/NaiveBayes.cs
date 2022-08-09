namespace BayesianClassifier
{
    public class NaiveBayes<T> where T : notnull
    {
        private FeatureDictionary<T> _featureDict;
        private CategoryDictionary _categoryDict;
        private double _weight;
        private double _assumedProbability;
        private string _defaultCategory;

        public NaiveBayes()
        {
            _featureDict = new FeatureDictionary<T>();
            _categoryDict = new CategoryDictionary();
            _weight = 1.0;
            _assumedProbability = 1.0;
            _defaultCategory = "Unclassified";
        }

        public NaiveBayes(double weight, double assumedProbability)
        {
            _featureDict = new FeatureDictionary<T>();
            _categoryDict = new CategoryDictionary();
            _weight = weight;
            _assumedProbability = assumedProbability;
            _defaultCategory = "Unclassified";
        }

        public NaiveBayes(string defaultCategory)
        {
            _featureDict = new FeatureDictionary<T>();
            _categoryDict = new CategoryDictionary();
            _weight = 1.0;
            _assumedProbability = 1.0;
            _defaultCategory = defaultCategory;
        }

        public NaiveBayes(double weight, double assumedProbability, string defaultCategory)
        {
            _featureDict = new FeatureDictionary<T>();
            _categoryDict = new CategoryDictionary();
            _weight = weight;
            _assumedProbability = assumedProbability;
            _defaultCategory = defaultCategory;
        }

        /// <summary>
        /// Loads the featureCount and categoryCount dictionaries\n
        /// <para>Name of classifier should be passed without extension (e.g, "nb_foodclassifier")</para>
        /// </summary>
        /// <param name="filename">Path to classifier without extension - e.g, "path/nb" will load both the nb.feature and nb.category files.</param>
        public void LoadClassifier(string filename)
        {
            JsonSerialise.LoadObject(filename + ".feature", ref _featureDict);
            JsonSerialise.LoadObject(filename + ".category", ref _categoryDict);
        }

        /// <summary>
        /// Saves the featureCount and categoryCount dictionaries
        /// <para> Name of classifier should be passed without extension (e.g, "nb_foodclassifier")</para>
        /// </summary>
        /// <param name="filename">Save path for classifier without extension - e.g, "path/nb" will create both the nb.feature and nb.category files.</param>
        public void SaveClassifier(string filename)
        {
            JsonSerialise.SaveObject(filename + ".feature", _featureDict);
            JsonSerialise.SaveObject(filename + ".category", _categoryDict);
        }

        public void SetAssumedProb(double assumedProbability)
        {
            _assumedProbability = assumedProbability;
        }

        public void SetWeight(double weight)
        {
            _weight = weight;
        }

        public void SetDefaultCategory(string defaultCategory)
        {
            _defaultCategory = defaultCategory;
        }

        private void IncrementFeatureCount(T feature, string category)
        {
            _featureDict.IncrementValue(feature, category);
        }

        private void IncrementCategoryCount(string category)
        {
            _categoryDict.IncrementValue(category);
        }

        private int GetFeaturesCategoryValue(T feature, string category)
        {
            return _featureDict.GetCategoryValue(feature, category);
        }

        private int GetCategoryValue(string category)
        {
            return _categoryDict.GetValue(category);
        }

        private int GetCategoriesSum()
        {
            return _categoryDict.GetSumOfValues();
        }

        private IEnumerable<string> GetCategoryKeys()
        {
            return _categoryDict.GetCategories();
        }

        public void Train(IEnumerable<T> features, string category)
        {
            foreach (T feature in features)
            {
                IncrementFeatureCount(feature, category);
            }

            IncrementCategoryCount(category);
        }

        public void Train(T feature, string category)
        {
            IncrementFeatureCount(feature, category);
            IncrementCategoryCount(category);
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
            double total = _featureDict.GetSumOfAllValues(feature);

            return ((_weight * _assumedProbability) * (total * basicProbability)) / (_weight + total);
        }

        /// <summary>
        /// Returns the probabilty of the category existing in all features
        /// </summary>
        private double CommonAppearanceProbability(IEnumerable<T> features, string category)
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
        private double Probability(IEnumerable<T> features, string category)
        {
            double categoryProbability = GetCategoryValue(category) / (double)GetCategoriesSum();
            double commonAppearanceProbabilty = CommonAppearanceProbability(features, category);

            return categoryProbability * commonAppearanceProbabilty;
        }

        public string Classify(IEnumerable<T> features)
        {
            double max = 0.0;
            double categoryProbability;
            string best = _defaultCategory;

            IEnumerable<string> categories = GetCategoryKeys();
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
