namespace Localization.Common
{
    public class Text
    {
        public Text()
        {
        }

        public Text(int LCID, string value)
        {
            this.LCID = LCID;
            Value = value;
        }

        public int LCID { get; set; }

        public string Value { get; set; }
    }
}
