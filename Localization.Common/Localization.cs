using System.Collections.Generic;

namespace Localization.Common
{
    public class LocalizationDictionary : Dictionary<string, LocalizedText>
    {
        public LocalizedText GetText(string key)
        {
            if (TryGetValue(key, out var text))
            {
                return text;
            }

            return null;
        }
    }
}
