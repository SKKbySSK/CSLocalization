using System;
using CSharpSyntax;
using System.IO;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Globalization;

namespace Localization.Common
{
    public class SyntaxGenerator
    {
        public SyntaxGenerator(GeneratorConfig config)
        {
            Config = config;
        }

        public SyntaxGenerator() : this(new GeneratorConfig()) { }

        public GeneratorConfig Config { get; }

        public string Generate(LocalizationDictionary localization)
        {
            var cs = new ClassDeclarationSyntax();
            cs.Identifier = Config.ClassName;
            cs.Modifiers = Config.Modifiers;
            cs.BaseList = new BaseListSyntax();

            var ns = Syntax.NamespaceDeclaration(Config.Namespace);
            ns.Members.Add(cs);

            List<UsingDirectiveSyntax> usings = new List<UsingDirectiveSyntax>();

            CreateConstructor(cs);
            CreateUsings(usings);
            CreatePrivateFields(cs);
            CreateProperties(cs, localization);

            foreach (var pair in localization)
            {
                cs.Members.Add(CreateLocalizationProperty(pair.Key, pair.Value));
            }

            using (var sw = new StringWriter())
            using (var printer = new CSharpSyntax.Printer.SyntaxPrinter(new CSharpSyntax.Printer.SyntaxWriter(sw)))
            {
                if (Config.ImplementPropertyChanging)
                {
                    ResolvePropertyChanging(cs);
                }

                if (Config.ImplementPropertyChanged)
                {
                    ResolvePropertyChanged(cs);
                }

                foreach (var u in usings)
                {
                    printer.VisitUsingDirective(u);
                }

                printer.VisitNamespaceDeclaration(ns);

                return sw.GetStringBuilder().ToString();
            }
        }

        private void CreateConstructor(ClassDeclarationSyntax cs)
        {
            var currentc = Syntax.ObjectCreationExpression(Config.ClassName, new ArgumentListSyntax());
            var current = Syntax.BinaryExpression(BinaryOperator.Equals, Syntax.ParseName("Current"), currentc);
            var constructor = Syntax.ConstructorDeclaration(Config.ClassName, Syntax.ParameterList(),
                Syntax.Block(
                    Syntax.ExpressionStatement(current)
                ));
            constructor.Modifiers |= Modifiers.Static;

            cs.Members.Add(constructor);
        }

        private void ResolvePropertyChanged(ClassDeclarationSyntax classDeclaration)
        {
            const string name = "PropertyChanged";
            classDeclaration.BaseList.Types.Add(Syntax.ParseName("INotifyPropertyChanged"));
            classDeclaration.Members.Add(CreateEvent("PropertyChangedEventHandler", "PropertyChanged"));

            var invoker = Syntax.MethodDeclaration("void", "On" + name,
                Syntax.ParameterList(CreateCallerMemberNameParam("string", "propertyName")));
            invoker.Modifiers |= Modifiers.Private;

            var access = Syntax.MemberAccessExpression(Syntax.ParseName(name + "?"), $"Invoke");

            var e = Syntax.ObjectCreationExpression(name + "EventArgs",
             Syntax.ArgumentList(Syntax.Argument(Syntax.ParseName("propertyName"))));
            var args = Syntax.ArgumentList(Syntax.Argument(Syntax.ThisExpression()), Syntax.Argument(e));

            var exp = Syntax.InvocationExpression(access, args);
            invoker.Body = Syntax.Block(Syntax.ExpressionStatement(exp));
            classDeclaration.Members.Add(invoker);
        }

        private void ResolvePropertyChanging(ClassDeclarationSyntax classDeclaration)
        {
            const string name = "PropertyChanging";
            classDeclaration.BaseList.Types.Add(Syntax.ParseName("INotifyPropertyChanging"));
            classDeclaration.Members.Add(CreateEvent(name + "EventHandler", name));

            var invoker = Syntax.MethodDeclaration("void", "On" + name,
                Syntax.ParameterList(CreateCallerMemberNameParam("string", "propertyName")));
            invoker.Modifiers |= Modifiers.Private;

            var access = Syntax.MemberAccessExpression(Syntax.ParseName(name + "?"), $"Invoke");

            var e = Syntax.ObjectCreationExpression(name + "EventArgs",
             Syntax.ArgumentList(Syntax.Argument(Syntax.ParseName("propertyName"))));
            var args = Syntax.ArgumentList(Syntax.Argument(Syntax.ThisExpression()), Syntax.Argument(e));

            var exp = Syntax.InvocationExpression(access, args);
            invoker.Body = Syntax.Block(Syntax.ExpressionStatement(exp));
            classDeclaration.Members.Add(invoker);
        }

        private void CreatePrivateFields(ClassDeclarationSyntax classDeclaration)
        {
            FieldDeclarationSyntax cultureF = new FieldDeclarationSyntax();
            cultureF.Modifiers = Modifiers.Private;
            cultureF.Declaration = new VariableDeclarationSyntax()
            {
                Type = Syntax.ParseName(nameof(CultureInfo))
            };

            cultureF.Declaration.Variables.Add(new VariableDeclaratorSyntax()
            {
                Identifier = "culture",
                Initializer = new EqualsValueClauseSyntax()
                {
                    Value = Syntax.ParseName("CultureInfo.CurrentCulture")
                }
            });

            classDeclaration.Members.Add(cultureF);
        }

        private void CreateUsings(List<UsingDirectiveSyntax> syntaxes)
        {
            syntaxes.Add(Syntax.UsingDirective("System"));
            syntaxes.Add(Syntax.UsingDirective("System.Globalization"));

            if (Config.ImplementPropertyChanged || Config.ImplementPropertyChanging)
            {
                syntaxes.Add(Syntax.UsingDirective("System.ComponentModel"));
            }
        }

        private EventFieldDeclarationSyntax CreateEvent(string type, string name)
        {
            var ev = Syntax.EventFieldDeclaration(modifiers: Modifiers.Public, declaration: new VariableDeclarationSyntax()
            {
                Type = Syntax.ParseName(type)
            });
            ev.Declaration.Variables.Add(new VariableDeclaratorSyntax()
            {
                Identifier = name
            });

            return ev;
        }

        private void CreateProperties(ClassDeclarationSyntax classDeclaration, LocalizationDictionary localization)
        {
            classDeclaration.Members.Add(CreateCurrentProperty());
            classDeclaration.Members.Add(CreateCultureProperty(localization));
        }

        private PropertyDeclarationSyntax CreateCurrentProperty()
        {
            var prop = Syntax.PropertyDeclaration(null, Modifiers.Public | Modifiers.Static, Config.ClassName, null, "Current");
            prop.AccessorList = Syntax.AccessorList(Syntax.AccessorDeclaration(AccessorDeclarationKind.Get, null));

            return prop;
        }

        private PropertyDeclarationSyntax CreateCultureProperty(LocalizationDictionary localization)
        {
            var getter = Syntax.Block(Syntax.ReturnStatement(Syntax.ParseName("culture")));

            var condition = Syntax.BinaryExpression(BinaryOperator.EqualsEquals, Syntax.ParseName("culture"), Syntax.ParseName("value"));

            List<StatementSyntax> statements = new List<StatementSyntax>();
            statements.Add(Syntax.IfStatement(condition, Syntax.ReturnStatement()));

            if (Config.ImplementPropertyChanging)
            {
                statements.Add(Syntax.ExpressionStatement(CreateChangingMethodInvocator("Culture")));
            }

            statements.Add(Syntax.ExpressionStatement(Syntax.BinaryExpression(BinaryOperator.Equals, Syntax.ParseName("culture"), Syntax.ParseName("value"))));

            if (Config.ImplementPropertyChanged)
            {
                statements.Add(Syntax.ExpressionStatement(CreateChangedMethodInvocator("Culture")));
            }

            foreach (var loc in localization)
            {
                if (Config.ImplementPropertyChanging)
                {
                    statements.Add(Syntax.ExpressionStatement(CreateChangingMethodInvocator(loc.Key)));
                }

                if (Config.ImplementPropertyChanged)
                {
                    statements.Add(Syntax.ExpressionStatement(CreateChangedMethodInvocator(loc.Key)));
                }
            }

            var setter = Syntax.Block(statements);

            var prop = Syntax.PropertyDeclaration(null, Modifiers.Public, nameof(CultureInfo), null, "Culture");
            prop.AccessorList = Syntax.AccessorList(Syntax.AccessorDeclaration(AccessorDeclarationKind.Get, getter),
                Syntax.AccessorDeclaration(AccessorDeclarationKind.Set, setter));

            return prop;
        }

        private InvocationExpressionSyntax CreateChangingMethodInvocator(string name)
        {
            var access = Syntax.MemberAccessExpression(Syntax.ThisExpression(), "OnPropertyChanging");
            return Syntax.InvocationExpression(access, Syntax.ArgumentList(Syntax.Argument(Syntax.LiteralExpression(name))));
        }

        private InvocationExpressionSyntax CreateChangedMethodInvocator(string name)
        {
            var access = Syntax.MemberAccessExpression(Syntax.ThisExpression(), "OnPropertyChanged");
            return Syntax.InvocationExpression(access, Syntax.ArgumentList(Syntax.Argument(Syntax.LiteralExpression(name))));
        }

        private PropertyDeclarationSyntax CreateLocalizationProperty(string key, LocalizedText text)
        {
            List<StatementSyntax> statements = new List<StatementSyntax>();
            List<SwitchSectionSyntax> switchSections = new List<SwitchSectionSyntax>();

            foreach (var t in text)
            {
                var sec = new SwitchSectionSyntax();
                var eq = Syntax.BinaryExpression(BinaryOperator.Equals, Syntax.ParseName("text"), Syntax.LiteralExpression(t.Value));
                sec.Labels.Add(Syntax.SwitchLabel(CaseOrDefault.Case, Syntax.LiteralExpression(t.LCID)));
                sec.Statements.Add(Syntax.ExpressionStatement(eq));
                sec.Statements.Add(Syntax.BreakStatement());
                switchSections.Add(sec);
            }

            var def = new SwitchSectionSyntax();
            var eqdef = Syntax.BinaryExpression(BinaryOperator.Equals, Syntax.ParseName("text"), Syntax.LiteralExpression(key));
            def.Labels.Add(Syntax.SwitchLabel(CaseOrDefault.Default));
            def.Statements.Add(Syntax.ExpressionStatement(eqdef));
            def.Statements.Add(Syntax.BreakStatement());
            switchSections.Add(def);

            var sw = Syntax.SwitchStatement(Syntax.ParseName("Culture.LCID"), switchSections);
            var dec = Syntax.VariableDeclarator("text");

            statements.Add(Syntax.LocalDeclarationStatement(Syntax.VariableDeclaration("string", new[] { dec })));
            statements.Add(sw);
            statements.Add(Syntax.ReturnStatement(Syntax.ParseName("text")));

            var getter = Syntax.Block(statements);
            var prop = Syntax.PropertyDeclaration(null, Modifiers.Public, "string", null, key);
            prop.AccessorList = Syntax.AccessorList(Syntax.AccessorDeclaration(AccessorDeclarationKind.Get, getter));
            return prop;
        }

        private ParameterSyntax CreateCallerMemberNameParam(string type, string name)
        {
            return Syntax.Parameter(new AttributeListSyntax[] { Syntax.AttributeList(Syntax.Attribute("CallerMemberName")) },
                ParameterModifier.None, Syntax.ParseName(type), name, new EqualsValueClauseSyntax() { Value = Syntax.ParseName("null") });
        }

        private bool UseStatic(Modifiers modifiers)
        {
            return modifiers.HasFlag(Modifiers.Static);
        }

        private ExpressionSyntax GetNullExpression()
        {
            return Syntax.ParseName("null");
        }
    }
}
