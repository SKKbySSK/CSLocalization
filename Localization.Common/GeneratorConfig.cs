using CSharpSyntax;

namespace Localization.Common
{
    public class GeneratorConfig
    {
        public string ClassName { get; set; } = "Localization";

        public bool ImplementPropertyChanged { get; set; } = true;

        public bool ImplementPropertyChanging { get; set; } = true;

        public string Namespace { get; set; } = "App";

        public Modifiers Modifiers { get; set; } = Modifiers.Public;
    }
}
