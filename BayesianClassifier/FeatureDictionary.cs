namespace BayesianClassifier
{
    [System.Serializable]
    internal class FeatureDictionary<T> where T : notnull
    {
        private Dictionary<T, CategoryDictionary> dict;
        private const int DEFAULT_VALUE = 0;

        public FeatureDictionary()
        {
            dict = new Dictionary<T, CategoryDictionary>();
        }

        public void AddElement(T feature, string category, int value)
        {
            if (!dict.ContainsKey(feature))
            {
                dict.Add(feature, new CategoryDictionary(category, value));
            }
            else
            {
                dict[feature].AddKeyValuePair(category, value);
            }
        }

        public void IncrementValue(T feature, string category)
        {
            if (!dict.ContainsKey(feature))
            {
                dict.Add(feature, new CategoryDictionary(category, DEFAULT_VALUE));
            }
            dict[feature].IncrementValue(category);
        }

        public List<T> GetFeatures()
        {
            return dict.Keys.ToList();
        }

        public List<string> GetCategoriesForFeature(T feature)
        {
            return dict[feature].GetCategories();
        }

        public int GetCategoryValue(T feature, string category)
        {
            if (!dict.ContainsKey(feature))
            {
                dict.Add(feature, new CategoryDictionary(category, DEFAULT_VALUE));
            }

            return dict[feature].GetValue(category);
        }

        public int GetSumOfAllValues(T feature)
        {
            int sum = 0;
            if (dict.ContainsKey(feature))
            {
                List<string> categories = GetCategoriesForFeature(feature);
                foreach (var category in categories)
                {
                    sum += dict[feature].GetValue(category);
                }
            }

            return sum;
        }
    }
}
