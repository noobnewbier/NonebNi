using System.Collections.Generic;

namespace NonebNi.Ui.Entities
{
    public class Context
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public T? Get<T>(string key) where T : class => _data[key] as T;

        public void Set<T>(string key, T value) where T : class
        {
            _data[key] = value;
        }
    }
}