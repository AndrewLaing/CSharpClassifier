namespace BayesianClassifier
{
    [System.Serializable]
    internal class FeatureDictionary<T> where T : notnull
    {
        private Dictionary<T, CategoryDictionary> _dict;
        private const int DEFAULT_VALUE = 0;

        public FeatureDictionary()
        {
            _dict = new Dictionary<T, CategoryDictionary>();
        }

        public void AddElement(T feature, string category, int value)
        {
            if (!_dict.ContainsKey(feature))
            {
                _dict.Add(feature, new CategoryDictionary(category, value));
            }
            else
            {
                _dict[feature].AddKeyValuePair(category, value);
            }
        }

        public void IncrementValue(T feature, string category)
        {
            if (!_dict.ContainsKey(feature))
            {
                _dict.Add(feature, new CategoryDictionary(category, DEFAULT_VALUE));
            }
            _dict[feature].IncrementValue(category);
        }

        public List<T> GetFeatures()
        {
            return _dict.Keys.ToList();
        }

        public List<string> GetCategoriesForFeature(T feature)
        {
            return _dict[feature].GetCategories();
        }

        public int GetCategoryValue(T feature, string category)
        {
            if (!_dict.ContainsKey(feature))
            {
                _dict.Add(feature, new CategoryDictionary(category, DEFAULT_VALUE));
            }

            return _dict[feature].GetValue(category);
        }

        public int GetSumOfAllValues(T feature)
        {
            int sum = 0;
            if (_dict.ContainsKey(feature))
            {
                List<string> categories = GetCategoriesForFeature(feature);
                foreach (var category in categories)
                {
                    sum += _dict[feature].GetValue(category);
                }
            }

            return sum;
        }
    }
}
