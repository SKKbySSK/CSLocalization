using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using CSLocalization.Shared;
using Reactive.Bindings;

namespace CSLocalization.Windows.ViewModels
{
    class StartViewModel
    {
        public StartViewModel()
        {
            NewAddCommand.Subscribe(() =>
            {
                Selected.Value = Manager.AddProject(NewTitle.Value);
                OpenSelectedProject();
            });

            DeleteCommand.Subscribe(() =>
            {
                if (Selected.Value != null)
                    Manager.DeleteProject(Selected.Value);
            });

            Manager = new ProjectManager(path);
        }

        readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CSLocalization");
        public ProjectManager Manager { get; }
        
        public ReactiveProperty<string> NewTitle { get; } = new ReactiveProperty<string>();

        public ReactiveCommand NewAddCommand { get; } = new ReactiveCommand();

        public ReactiveCommand DeleteCommand { get; } = new ReactiveCommand();

        public ReactiveProperty<Project> Selected { get; } = new ReactiveProperty<Project>();

        public void OpenSelectedProject()
        {
            if (Selected.Value != null)
            {

            }
        }
    }
}
