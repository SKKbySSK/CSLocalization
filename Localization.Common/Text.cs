namespace Localization.Common
{
    public class Text
    {
        public Text()
        {
        }

        public Text(int lCID, string value)
        {
            LCID = lCID;
            Value = value;
        }

        public int LCID { get; set; }

        public string Value { get; set; }
    }
}
