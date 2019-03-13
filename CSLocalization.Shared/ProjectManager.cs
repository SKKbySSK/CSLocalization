using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace CSLocalization.Shared
{
    public class ProjectManager
    {
        public ProjectManager(string rootDirectory)
        {
            RootDirectory = rootDirectory;
            Projects = new ReadOnlyObservableCollection<Project>(projects);
            RefreshProjects();
        }

        private void RefreshProjects()
        {
            projects.Clear();

            if (Directory.Exists(RootDirectory))
            {
                var files = Directory.GetFiles(RootDirectory, "*.json");

                foreach (var f in files)
                {
                    var project = JsonConvert.DeserializeObject<Project>(File.ReadAllText(f));
                    project.Path = f;
                    project.Manager = this;
                    projects.Add(project);
                }
            }
        }

        public Project AddProject(string title)
        {
            var project = new Project();
            project.Manager = this;
            project.Title = title;
            project.Path = Path.Combine(RootDirectory, Guid.NewGuid() + ".json");
            project.Export();

            projects.Add(project);

            return project;
        }

        public void DeleteProject(Project project)
        {
            if (File.Exists(project.Path)) File.Delete(project.Path);

            project.Manager = null;
            project.Path = null;

            projects.Remove(project);
        }

        public string RootDirectory { get; }

        private ObservableCollection<Project> projects = new ObservableCollection<Project>();
        public ReadOnlyObservableCollection<Project> Projects { get; }
    }
}
