using CSLocalization.Shared;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CSLocalization.Windows.Views
{
    /// <summary>
    /// EditorWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditorWindow : Window
    {
        private Project Project { get; }

        public EditorWindow(Project project)
        {
            Project = project;

            InitializeComponent();
            editor.Project = project;
        }

        private void AddLang_Click(object sender, RoutedEventArgs e)
        {
            var d = new LanguageWindow();
            d.ShowDialog();

            if (d.SelectedCulture != null && !Project.Localization.GetLCIDs().Contains(d.SelectedCulture.LCID))
            {
                Project.Localization.AddLCID(d.SelectedCulture.LCID);
                Project.Export();

                editor.RefreshView();
            }
        }

        private void AddKey_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(keyBox.Text) && !Project.Localization.ContainsKey(keyBox.Text))
            {
                Project.Localization.Add(keyBox.Text);
                Project.Export();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Project.Export();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "C# Source File|*.cs";
            if (sfd.ShowDialog() ?? false)
            {
                var syntax = new Localization.Common.SyntaxGenerator(Project.GeneratorConfig);
                var text = syntax.Generate(Project.Localization);

                System.IO.File.WriteAllText(sfd.FileName, text);
            }
        }
    }
}
