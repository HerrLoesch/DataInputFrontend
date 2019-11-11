using DataInputt.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataInputt
{
    /// <summary>
    /// Interaktionslogik für Projects.xaml
    /// </summary>
    public partial class Projects : Page
    {
        private Delete delete;
        private int i = 1;
        private List<Project> projectsList;
        private Tuple<bool, int> bearbeiten = new Tuple<bool, int>(false, -1);

        public event PropertyChangedEventHandler PropertyChanged;

        public Projects()
        {
            InitializeComponent();
            delete = Delete.GetInstance();
            ProjectRepo.ProjectCollectionImported += ProjectRepo_ProjectCollectionImported;
            projectsList = new List<Project>()
            {
                new Project
                {
                    Id = i++,
                    Abstract = "Project1",
                    From = "10.05.2016",
                    To = "10.07.2016",
                    UntilToday = false,
                    Tools = new string[] { "Jetbrains", "Visual Studio" },
                    Description = "I have done a project",
                    Position = "Position1",
                    Tasks = new string[] { "Write code", "Read code", "Delete code"},
                    Sector = "Autos"
                },
                new Project
                {
                    Id = i++,
                    Abstract = "Project2",
                    From = "17.01.2016",
                    To = "25.01.2016",
                    Tools = new string[] { "Visual Studio", "Resharper" },
                    Tasks = new string[] { "Clean Up", "Leadership" },
                    Description = "This was the real shit",
                    Position = "Position2",
                    UntilToday = false,
                    Sector = "Medizin"
                },
                new Project
                {
                    Id = i++,
                    Abstract = "Project3",
                    From = "30.05.2017",
                    UntilToday = false,
                    To = "10.06.2017",
                    Position = "Position1",
                    Tasks = new string[] { "Write code", "Read code", "Delete code"},
                    Description = "A projekt about projects",
                    Tools = new string[] { "Jetbrains", "Visual Studio" },
                    Sector = "Autos"
                }
            };
            foreach (var item in projectsList)
            {
                publisherListView.Items.Add(item);
            }
            ProjectRepo.Projects = projectsList;
        }

        private void ProjectRepo_ProjectCollectionImported(object sender, EventArgs e)
        {
            /*projectsList.Clear();
            foreach (var item in (List<Project>)sender)
            {
                projectsList.Add(item);
                publisherListView.Items.Clear();
            }
            foreach (var item in projectsList)
            {
                publisherListView.Items.Add(item);
            }*/

            publisherListView.Items.Clear();
            foreach (var item in (List<Project>)sender)
            {
                publisherListView.Items.Add(item);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bearbeiten = new Tuple<bool, int>(true, projectsList.Find(p => p.Id == (int)((Button)sender).CommandParameter).Id);
            Project project = projectsList.Find(p => p.Id == (int)((Button)sender).CommandParameter);
            tb1.Text = project.Abstract;
            tb5.Text = project.Position;
            DatePicker1.Text = project.From;
            DatePicker2.Text = project.To;
            tb6.Text = project.Description;
            StringBuilder builder = new StringBuilder();
            foreach (string tool in project.Tools)
            {
                builder.Append(tool + ", ");
            }
            StringBuilder builder2 = new StringBuilder();
            foreach (string tool in project.Tasks)
            {
                builder2.Append(tool + ", ");
            }
            builder.Remove(builder.Length - 2, 2);
            tb3.Text = builder.ToString();
            tb7.Text = builder2.ToString();
            tb4.Text = project.Sector;
            myCheckbox.IsChecked = project.UntilToday;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            projectsList.Remove(projectsList.Find(p => p.Id == (int)((Button)sender).CommandParameter));
            foreach (var item in publisherListView.Items)
            {
                    if (!projectsList.Contains((Project)item))
                    {
                        delete.OnDeleteSomething(this, (Project)item);
                    }
            }
            publisherListView.Items.Clear();
            foreach (var item in projectsList)
            {
                publisherListView.Items.Add(item);
            }
            ProjectRepo.Projects = projectsList;
        }

                                        private void Button_Click_2(object sender, RoutedEventArgs e)
                                        {
                                            if (bearbeiten.Item1 == true)
                                            {
                                                projectsList.RemoveAt(projectsList.FindIndex(p => p.Id == bearbeiten.Item2));
                                                List<string> tools = new List<string>();
                                                Project a = new Project();
                                                a.Id = bearbeiten.Item2;
                                                a.Abstract = tb1.Text;
                                                a.From = DatePicker1.Text;
                                                a.To = DatePicker2.Text;
                                                a.Tools = tb3.Text.Replace(" ", "").Split(',');
                                                a.Tasks = tb7.Text.Replace(" ", "").Split(',');
                                                a.Position = tb5.Text;
                                                a.Description = tb6.Text;
                                                a.Sector = tb4.Text; // Macht das Refactoring Spaß? :D
                                                if (myCheckbox.IsChecked.Value)
                                                {
                                                    a.UntilToday = true;
                                                    a.To = "";
                                                }
                                                a.UntilToday = myCheckbox.IsChecked.Value;
                                                projectsList.Add(a);
                                                projectsList.Sort(new ProjectsComparer());
                                                bearbeiten = new Tuple<bool, int>(false, -1);
                                            }
                                            else
                                            {
                                                Project a = new Project();
                                                a.Id = i++;
                                                a.Abstract = tb1.Text;
                                                a.From = DatePicker1.Text;
                                                a.To = myCheckbox.IsChecked.Value ? "" : DatePicker2.Text;
                                                a.Tools = tb3.Text.Replace(" ", "").Split(',');
                                                a.Sector = tb4.Text;
                                                a.Tasks = tb7.Text.Replace(" ", "").Split(',');
                                                a.Position = tb5.Text;
                                                a.Description = tb6.Text;
                                                a.UntilToday = myCheckbox.IsChecked.Value;

                                                Project b = new Project() { Id = i++, Abstract = tb1.Text, From = DatePicker1.Text, To = myCheckbox.IsChecked.Value ? "" : DatePicker2.Text, Tools = tb3.Text.Replace(" ", "").Split(','), Sector = tb4.Text, Tasks = tb7.Text.Replace(" ", "").Split(','), Position = tb5.Text, Description = tb6.Text, UntilToday = myCheckbox.IsChecked.Value }; var x = new System.Diagnostics.Stopwatch(); x.Start();
                                #if DEBUG
                                                System.Diagnostics.Debug.WriteLine(a == b);
                                #endif
                                                x.Stop();

                                                int randomGenerator = (int)Math.Round(new Random().NextDouble());
                                                if (randomGenerator == 1)
                                                {
                                                    projectsList.Add(a);
                                                }
                                                else
                                                {
                                                    projectsList.Add(b);
                                                }
                                            }

                                            publisherListView.Items.Clear();
                                            foreach (var item in projectsList)
                                            {
                                                publisherListView.Items.Add(item);
                                            }
                                            ProjectRepo.Projects = projectsList;
                                            tb1.Text = String.Empty;
                                            tb3.Text = tb1.Text;
                                            tb4.Text = tb1.Text;
                                            tb6.Text = tb3.Text;
                                            tb3.Text = tb1.Text;
                                            tb5.Text = tb1.Text;
                                            tb6.Text = tb1.Text;
                                            tb7.Text = tb1.Text;
                                            myCheckbox.IsChecked = null;
                                            DatePicker1.SelectedDate = null;
                                            DatePicker2.SelectedDate = null;
                                        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            tb1.Text = "";
            tb3.Text = "";
            tb4.Text = "";
            tb5.Text = "";
            tb6.Text = "";
            tb7.Text = "";
            myCheckbox.IsChecked = null;
            DatePicker1.SelectedDate = null;
            DatePicker2.SelectedDate = null;
        }
    }

    class ProjectsComparer : IComparer<Project>
    {
        public int Compare(Project x, Project y)
        {
            if (x.Id != y.Id)
                return x.Id > y.Id ? 1 : -1;
            if (x.Abstract != y.Abstract)
                return StringComparer.Create(CultureInfo.CurrentCulture, true).Compare(x.Abstract, y.Abstract);
            return 0;
        }
    }
}
