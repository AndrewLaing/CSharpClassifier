namespace BayesianClassifier
{
    [System.Serializable]
    internal class CategoryDictionary
    {
        private Dictionary<string, int> dict;
        private const int DEFAULT_VALUE = 0;

        public CategoryDictionary()
        {
            dict = new Dictionary<string, int>();
        }

        public CategoryDictionary(string category, int value)
        {
            dict = new Dictionary<string, int>();
            AddKeyValuePair(category, value);
        }

        public void AddKeyValuePair(string category, int value)
        {
            if (!dict.ContainsKey(category))
            {
                dict.Add(category, value);
            }
            else
            {
                dict[category] = value;
            }
        }

        public void IncrementValue(string category)
        {
            if (!dict.ContainsKey(category))
            {
                AddKeyValuePair(category, DEFAULT_VALUE);
            }

            int value = dict[category] + 1;
            AddKeyValuePair(category, value);
        }

        public List<String> GetCategories()
        {
            return dict.Keys.ToList();
        }

        public int GetValue(string category)
        {
            if (!dict.ContainsKey(category))
            {
                dict.Add(category, DEFAULT_VALUE);
            }

            return dict[category];
        }

        public int GetSumOfValues()
        {
            if (dict.Count > 0)
            {
                return dict.Sum(x => x.Value);
            }
            else
            {
                return 0;
            }
        }
    }
}
