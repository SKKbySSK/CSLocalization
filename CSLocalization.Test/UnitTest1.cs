using System;
using Xunit;
using Localization.Common;
using System.Globalization;

namespace CSLocalization.Test
{
    public class UnitTest1
    {
        [Fact]
        public void GenerationTest()
        {
            var gen = new SyntaxGenerator();
            gen.ClassName = "TestClass";
            gen.Namespace = "CSLocalization.Test";
            gen.ImplementPropertyChanged = true;
            gen.ImplementPropertyChanging = false;

            var dict = new LocalizationDictionary();

            for (int i = 0; 7 > i; i++)
            {
                var week = (DayOfWeek)i;
                var text = new LocalizedText();
                text.Add(new Text(CultureInfo.CurrentCulture.LCID, "Hello " + week));
                dict[week.ToString()] = text;
            }

            var syntax = gen.Generate(dict);
            Console.WriteLine(syntax);
        }
    }
}
