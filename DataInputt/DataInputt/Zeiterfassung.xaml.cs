using DataInputt.DataInputServiceReference;
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
using System.ServiceModel;
using System.Windows.Threading;

namespace DataInputt
{
    /// <summary>
    /// Interaktionslogik für Projects.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
    public partial class Zeiterfassung : Page, IDataInputServiceCallback
    {
        private DataInputServiceClient client;
        private Delete delete;
        private int i = 1;
        private int userId = -1;
        private List<Time> timesList;
        private Tuple<bool, int> bearbeiten = new Tuple<bool, int>(false, -1);

        public event PropertyChangedEventHandler PropertyChanged;

        public Zeiterfassung()
        {
            InitializeComponent();
            client = new DataInputServiceClient(new InstanceContext(this));
            delete = Delete.GetInstance();
            projectsCombo.ItemsSource = client.Projects();
            projectsCombo.SelectedIndex = 0;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bearbeiten = new Tuple<bool, int>(true, timesList.Find(p => p.Id == (int)((Button)sender).CommandParameter).Id);
            Time time = timesList.Find(p => p.Id == (int)((Button)sender).CommandParameter);
            tb1.Text = time.Start.ToString();
            tb5.Text = time.End.ToString();
            projectsCombo.SelectedItem = time.Project;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            timesList.Remove(timesList.Find(p => p.Id == (int)((Button)sender).CommandParameter));
            foreach (var item in timesListView.Items)
            {
                    if (!timesList.Contains((Time)item))
                    {
                        delete.OnDeleteSomething(this, (Time)item);
                        client.AddTime((Time) item, -1);
                    }
            }
            timesListView.Items.Clear();
            foreach (var item in timesList)
            {
                timesListView.Items.Add(item);
            }
            TimeRepo.Times = timesList;
        }

                                        private void Button_Click_2(object sender, RoutedEventArgs e)
                                        {
                                            if (bearbeiten.Item1 == true)
                                            {
                                                timesList.RemoveAt(timesList.FindIndex(p => p.Id == bearbeiten.Item2));
                                                Time a = new Time();
                                                a.Id = bearbeiten.Item2;
                                                a.Start = DateTime.Parse(tb1.Text);
                                                a.End = DateTime.Parse(tb5.Text);
                                                a.Project = (string) projectsCombo.SelectedItem;
                                                a.uId = userId;                                        
                                                timesList.Add(a);
                                                timesList.Sort(new TimesComparer());
                                                bearbeiten = new Tuple<bool, int>(false, -1);
                                                client.AddTime(a, userId);
                                            }
                                            else
                                            {
                                                Time a = new Time();
                                                a.Id = a.GetHashCode();
                                                a.Start = DateTime.Parse(tb1.Text);
                                                a.End = DateTime.Parse(tb5.Text);
                                                a.Project = (string)projectsCombo.SelectedItem;
                                                a.uId = userId;

                                                timesList.Add(a);
                                                timesList.Sort(new TimesComparer());
                                                client.AddTime(a, userId);
                                            }

                                            timesListView.Items.Clear();
                                            foreach (var item in timesList)
                                            {
                                                timesListView.Items.Add(item);
                                            }
                                            TimeRepo.Times = timesList;
                                            tb1.Text = String.Empty;
                                            projectsCombo.SelectedIndex = 0;
                                            tb5.Text = tb1.Text;
                                        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            tb1.Text = "";
            tb5.Text = "";
            projectsCombo.SelectedIndex = 0;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var user = new User();
            user.Name = name.Text;
            user.Passwort = passwort.Text;
            userId = client.Login(user);

            timesListView.Items.Clear();
            timesList = client.GetTimes(userId).ToList();
            timesList.Sort(new TimesComparer());
            foreach(var item in timesList)
            {
                timesListView.Items.Add(item);
            }
            TimeRepo.Times = timesList;
        }

        public void EarningsCalculated(Dictionary<int, decimal> earnings)
        {
            var e = earnings.ContainsKey(userId) == true ? earnings[userId].ToString("C") : "0 €";
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.earnings.Text = e));
        }
    }

    class TimesComparer : IComparer<Time>
    {
        public int Compare(Time x, Time y)
        {
            if (x.Id != y.Id)
                return x.Id > y.Id ? 1 : -1;
            if (x.Project != y.Project)
                return StringComparer.Create(CultureInfo.CurrentCulture, true).Compare(x.Project, y.Project);
            return 0;
        }
    }
}
