using System.Collections.ObjectModel;
using System.Globalization;

namespace Localization.Common
{
    public class LocalizedText : ObservableCollection<Text>
    {
        public Text GetText(CultureInfo culture)
        {
            return GetText(culture.LCID);
        }

        public Text GetText(int LCID)
        {
            foreach (var t in this)
            {
                if (t.LCID == LCID)
                    return t;
            }

            return null;
        }

        public bool ContainsLCID(int LCID)
        {
            foreach (var t in this)
            {
                if (t.LCID == LCID)
                    return true;
            }

            return false;
        }
    }
}
