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

namespace CSLocalization.Windows.Views
{
    /// <summary>
    /// StartWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            VM.CloseCommand.Subscribe(() => Close());
        }

        private ViewModels.StartViewModel VM => (ViewModels.StartViewModel)DataContext;

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VM.OpenSelectedProject();
        }
    }
}
