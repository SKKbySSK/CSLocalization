using System;
using System.Collections.Generic;
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
using System.Globalization;

namespace CSLocalization.Windows.Views
{
    /// <summary>
    /// LanguageWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LanguageWindow : Window
    {
        public LanguageWindow()
        {
            InitializeComponent();

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            langs.ItemsSource = cultures;
        }

        public CultureInfo SelectedCulture { get; private set; } = null;

        private void DoneB_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Langs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCulture = (CultureInfo)langs.SelectedItem;
        }
    }
}
