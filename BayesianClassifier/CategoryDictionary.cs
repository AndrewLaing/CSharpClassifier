namespace BayesianClassifier
{
    [System.Serializable]
    internal class CategoryDictionary
    {
        private Dictionary<string, int> _dict;
        private const int DEFAULT_VALUE = 0;

        public CategoryDictionary()
        {
            _dict = new Dictionary<string, int>();
        }

        public CategoryDictionary(string category, int value)
        {
            _dict = new Dictionary<string, int>();
            AddKeyValuePair(category, value);
        }

        public void AddKeyValuePair(string category, int value)
        {
            if (!_dict.ContainsKey(category))
            {
                _dict.Add(category, value);
            }
            else
            {
                _dict[category] = value;
            }
        }

        public void IncrementValue(string category)
        {
            if (!_dict.ContainsKey(category))
            {
                AddKeyValuePair(category, DEFAULT_VALUE);
            }

            int value = _dict[category] + 1;
            AddKeyValuePair(category, value);
        }

        public List<String> GetCategories()
        {
            return _dict.Keys.ToList();
        }

        public int GetValue(string category)
        {
            if (!_dict.ContainsKey(category))
            {
                _dict.Add(category, DEFAULT_VALUE);
            }

            return _dict[category];
        }

        public int GetSumOfValues()
        {
            if (_dict.Count > 0)
            {
                return _dict.Sum(x => x.Value);
            }
            else
            {
                return 0;
            }
        }
    }
}
