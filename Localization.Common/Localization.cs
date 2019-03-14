using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Localization.Common
{
    public class LocalizationDictionary : ObservableCollection<KeyValuePair<string, LocalizedText>>
    {
        public LocalizedText this[string key]
        {
            get
            {
                foreach (var t in this)
                {
                    if (key == t.Key)
                        return t.Value;
                }

                throw new KeyNotFoundException();
            }
        }

        public void Add(string key)
        {
            Add(new KeyValuePair<string, LocalizedText>(key, new LocalizedText()));
        }

        public bool ContainsKey(string key)
        {
            foreach (var t in this)
            {
                if (t.Key == key)
                    return true;
            }

            return false;
        }

        public List<int> GetLCIDs()
        {
            List<int> lcids = new List<int>();

            foreach (var p in this)
            {
                foreach (var t in p.Value)
                {
                    if (!lcids.Contains(t.LCID))
                        lcids.Add(t.LCID);
                }
            }

            return lcids;
        }

        public void AddLCID(int lcid)
        {
            foreach (var p in this)
            {
                bool add = true;
                foreach (var t in p.Value)
                {
                    if (t.LCID == lcid)
                    {
                        add = false;
                        break;
                    }
                }

                if (add)
                {
                    p.Value.Add(new Text(lcid, null));
                }
            }
        }
    }
}
