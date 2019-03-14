using CSLocalization.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSLocalization.Windows.Views
{
    class EditorGrid : Grid
    {
        private Grid editor = new Grid()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        private static void HandlePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ed = (EditorGrid)sender;

            switch (e.Property.Name)
            {
                case nameof(RowHeight):
                    ed.RowHeightChanged((double)e.OldValue, (double)e.NewValue);
                    break;
                case nameof(Project):
                    ed.ProjectChanged((Project)e.OldValue, (Project)e.NewValue);
                    break;
            }
        }

        private void RowHeightChanged(double o, double n)
        {
            foreach(var row in editor.RowDefinitions)
            {
                row.Height = CreateRowHeight();
            }
        }

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double), typeof(EditorGrid), new PropertyMetadata(30d, HandlePropertyChanged));


        private void ProjectChanged(Project o, Project n)
        {
            if (o != null)
            {
                o.Localization.CollectionChanged -= Localization_CollectionChanged;
            }

            if (n != null)
            {
                n.Localization.CollectionChanged += Localization_CollectionChanged;
            }

            RefreshView();
        }

        private void Localization_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshView();
        }

        public Project Project
        {
            get { return (Project)GetValue(ProjectProperty); }
            set { SetValue(ProjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Project.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProjectProperty =
            DependencyProperty.Register("Project", typeof(Project), typeof(EditorGrid), new PropertyMetadata(null, HandlePropertyChanged));



        public bool ShouldExport
        {
            get { return (bool)GetValue(ShouldExportProperty); }
            private set { SetValue(ShouldExportProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShouldExport.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShouldExportProperty =
            DependencyProperty.Register("ShouldExport", typeof(bool), typeof(EditorGrid), new PropertyMetadata(false));



        public void RefreshView()
        {

            editor.Children.Clear();
            editor.RowDefinitions.Clear();
            editor.ColumnDefinitions.Clear();
            int column = 0;

            if (Project != null)
            {
                editor.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150, GridUnitType.Pixel) });

                foreach (var (pair, i) in Project.Localization.Select((item, index) => (item, index)))
                {
                    editor.RowDefinitions.Add(new RowDefinition() { Height = CreateRowHeight() });
                    editor.Children.Add(CreateTextBox(pair.Key, i, column, v => !Project.Localization.ContainsKey(v), v =>
                    {
                        Project.Localization[i] = new KeyValuePair<string, Localization.Common.LocalizedText>(v, pair.Value);
                        ShouldExport = true;
                    }));
                }

                foreach(var lcid in Project.Localization.GetLCIDs())
                {
                    AddColumnWithSplitter();

                    foreach (var (key, i) in Project.Localization.Select((pair, i) => (pair.Key, i)))
                    {
                        var loc = Project.Localization[key];
                        var text = loc.GetText(lcid);
                        if (text == null)
                        {
                            text = new Localization.Common.Text(lcid, null);
                            loc.Add(text);
                        }

                        editor.Children.Add(CreateTextBox(text.Value, i, column, _ => true, v =>
                        {
                            text.Value = v;
                            ShouldExport = true;
                        }));
                    }
                }

                AddColumnWithSplitter();
            }

            void AddColumnWithSplitter()
            {
                editor.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                column++;

                var spl = new GridSplitter();
                spl.HorizontalAlignment = HorizontalAlignment.Stretch;
                spl.Width = 5;
                SetColumn(spl, column);
                SetRowSpan(spl, editor.RowDefinitions.Count);
                editor.Children.Add(spl);

                editor.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150, GridUnitType.Pixel) });
                column++;
            }
        }

        private GridLength CreateRowHeight()
        {
            return RowHeight < 0 ? new GridLength(1, GridUnitType.Auto) : new GridLength(RowHeight);
        }

        private TextBox CreateTextBox(string value, int row, int column, Func<string, bool> validation, Action<string> TextChanged)
        {
            var tb = new TextBox()
            {
                Text = value,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
            };

            tb.KeyDown += (sender, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Return)
                {
                    update();
                }
            };

            tb.LostFocus += (sender, e) =>
            {
                update();
            };

            void update()
            {
                if (validation(tb.Text))
                {
                    TextChanged(tb.Text);
                    value = tb.Text;
                }
                else
                {
                    tb.Text = value;
                }
            }

            SetRow(tb, row);
            SetColumn(tb, column);

            return tb;
        }

        public EditorGrid()
        {
            Children.Add(editor);
        }
    }
}
