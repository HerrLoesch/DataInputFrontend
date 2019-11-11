using DataInputt.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Reflection;
using DataInputt.Logging;
using Path = System.IO.Path;

namespace DataInputt
{
    /// <summary>
    /// Interaktionslogik für Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        private Delete delete;
        int zahl = 1;
        TextBox tb1 = new TextBox();
        ListView listViewKind; ComboBox comboBoxEnkel;
        TextBox tb2;
        Button clickadd = new Button();

        public Page1()
        {
            InitializeComponent();
            MediaRepo.MediaCollectionImported += MediaRepo_MediaCollectionImported;
            this.delete = Delete.GetInstance();
            delete.SomethingDeleted += Delete_SomethingDeleted;
            GridViewColumn gridViewColumnUrenkel1 = new GridViewColumn();
            gridViewColumnUrenkel1.Header = "Id";
            gridViewColumnUrenkel1.DisplayMemberBinding = new Binding("Id");

            GridViewColumn gridViewColumnUrenkel2 = new GridViewColumn();
            gridViewColumnUrenkel2.Header = "Bezeichnung";
            gridViewColumnUrenkel2.DisplayMemberBinding = new Binding("Name");

            StackPanel stackPanelKind2 = new StackPanel();
            GridViewColumn gridViewColumnUrenkel3 = new GridViewColumn();
            gridViewColumnUrenkel3.Header = "Url";
            gridViewColumnUrenkel3.DisplayMemberBinding = new Binding("Link");

            GridViewColumn gridViewColumnUrenkel4 = new GridViewColumn();
            gridViewColumnUrenkel4.Header = "PublisherId";
            gridViewColumnUrenkel4.DisplayMemberBinding = new Binding("PublisherId");

            GridViewColumn gridViewColumnUrenkel5 = new GridViewColumn();
            gridViewColumnUrenkel5.Header = "";
            gridViewColumnUrenkel5.CellTemplate = (DataTemplate)this.Resources["tempplate1"];

            GridViewColumn gridViewColumnUrenkel6 = new GridViewColumn();
            gridViewColumnUrenkel6.Header = "";
            gridViewColumnUrenkel6.CellTemplate = (DataTemplate)this.Resources["template2"];

            GridView gridViewEnkel = new GridView();
            gridViewEnkel.Columns.Add(gridViewColumnUrenkel1);
            gridViewEnkel.Columns.Add(gridViewColumnUrenkel2);
            gridViewEnkel.Columns.Add(gridViewColumnUrenkel3);
            gridViewEnkel.Columns.Add(gridViewColumnUrenkel4);
            gridViewEnkel.Columns.Add(gridViewColumnUrenkel5);
            gridViewEnkel.Columns.Add(gridViewColumnUrenkel6);

            listViewKind = new ListView()
            {
                View = gridViewEnkel,
                Height = (double) Application.Current.Resources["RandomDouble"],
                Margin = new Thickness(0, 23, 0, 0)
            };
            //var y = new List<Medium>
            //    {
            //        new Medium()
            //        {
            //            Id = zahl++, Name = "Medium1", Link = "abcdef.de", PublisherId=2
            //        },
            //        new Medium()
            //        {
            //            Id = zahl++, Name = "Medium2", Link = "ertz.de", PublisherId=2
            //        }
            //    };
            //foreach (var item in y)
            //{
            //    listViewKind.Items.Add(item);
            //}
            listViewKind.Resources.Add(typeof(ListViewItem), this.Resources["listItem"]);
            List<Medium> a = new List<Medium>();
            foreach (var item in listViewKind.Items)
            {
                a.Add((Medium)item);
            }
            MediaRepo.Media = a;

            StackPanel stackPanelMama = new StackPanel();
            stackPanelMama.Children.Add(listViewKind);

            StackPanel stackPanelKind1 = new StackPanel();
            StackPanel stackPanelKind3 = new StackPanel();
            stackPanelKind1.Orientation = Orientation.Horizontal;
            stackPanelKind2.Orientation = Orientation.Horizontal;
            stackPanelKind3.Orientation = Orientation.Horizontal;
            stackPanelKind1.Margin = new Thickness(31, 10, 0, 0);
            stackPanelKind2.Margin = new Thickness(31, 10, 0, 0);
            stackPanelKind3.Margin = new Thickness(31, 10, 0, 0);

            if (new Data().TryImportMedia(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Medium.csv"),
                out var media))
            {
                foreach (var _ in media)
                {
                    Trace.WriteLine(_);
                }
            }

            TextBlock textBlockEnkel1 = new TextBlock();
            textBlockEnkel1.Text = "Bezeichnung";
            textBlockEnkel1.Width = 101;
            stackPanelKind1.Children.Add(textBlockEnkel1);
            stackPanelKind1.Children.Add(tb1);
            TextBlock textBlockEnkel2 = new TextBlock();
            textBlockEnkel2.Text = "Url";
            textBlockEnkel2.Width = 101;
            TextBlock textBlockEnkel3 = new TextBlock();
            textBlockEnkel3.Text = "Publisher";
            textBlockEnkel3.Width = 101;
            stackPanelKind2.Children.Add(textBlockEnkel2);
            stackPanelKind3.Children.Add(textBlockEnkel3);
            tb2 = new TextBox();
            stackPanelKind2.Children.Add(tb2);
            comboBoxEnkel = new ComboBox();
            comboBoxEnkel.DisplayMemberPath = "Name";
            tb1.Width = 200;
            tb2.Width = 200;
            comboBoxEnkel.Width = 200;
            stackPanelKind3.Children.Add(comboBoxEnkel);

            stackPanelMama.Children.Add(stackPanelKind1);
            stackPanelMama.Children.Add(stackPanelKind2);
            stackPanelMama.Children.Add(stackPanelKind3);

            StackPanel clickmes = new StackPanel();
            clickadd.Content = "Speichern";
            clickadd.Click += Add;
            clickadd.Margin = new Thickness(31, 10, 0, 0);
            clickadd.Width = 301;

            Button clickedit = new Button();
            clickedit.Content = "Zurücksetzen";
            clickedit.Click += Button_Click_3;
            clickedit.Margin = new Thickness(31, 10, 0, 0);
            clickedit.Width = 301;
            clickmes.Children.Add(clickadd);
            clickmes.Children.Add(clickedit);
            clickadd.HorizontalAlignment = HorizontalAlignment.Left;
            clickedit.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanelMama.Children.Add(clickmes);

            this.Content = stackPanelMama;

            this.logger = new CsvLogger(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logfile.log"));
        }

        private void MediaRepo_MediaCollectionImported(object sender, EventArgs e)
        {
            listViewKind.Items.Clear();
            foreach (var item in (List<Medium>)sender)
            {
                listViewKind.Items.Add(item);
            }
        }

        private void Delete_SomethingDeleted(object sender, DeleteEventArgs e)
        {
            this.logger.Log($"{nameof(Delete_SomethingDeleted)} called");

            if(e.Object.GetType() == typeof(Publisher))
            {
                for (int i = 0; i < listViewKind.Items.Count; i++)
                {
                    Publisher pub = e.Object as Publisher;
                    if (pub != null)
                    {
                        object obj = listViewKind.Items[i];
                        Medium med = obj as Medium;
                        if (med != null)
                        {
                            if (med.PublisherId == pub.Id)
                            {
                                Medium med2 = new Medium();
                                med2.Id = med.Id;
                                med2.Name = med.Name;
                                med2.Link = med.Link;
                                var j = listViewKind.Items.IndexOf(med);
                                listViewKind.Items.RemoveAt(j);
                                listViewKind.Items.Insert(j, med2);
                                List<Medium> a = new List<Medium>();
                                foreach(var item in listViewKind.Items)
                                {
                                    a.Add((Medium)item);
                                }
                                MediaRepo.Media = a;
                            }
                        }
                    }
                }
            }
        }

        int qw = -1;
        int itemid = -1;
        private CsvLogger logger;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.logger.Log($"{nameof(Button_Click)} called");

            string bezeichnung = "";
            foreach (var item in listViewKind.Items)
            {
                if(item is Medium medium)
                {
                    if(medium.Id == (int)((Button)sender).CommandParameter)
                    {
                        bezeichnung = medium.Name;
                    }
                }
            }
            string url = "";
            foreach (var item in listViewKind.Items)
            {
                if (item is Medium medium)
                {
                    if (medium.Id == (int)((Button)sender).CommandParameter)
                    {
                        url = medium.Link;
                    }
                }
            }
            int id = -1;
            foreach (var item in listViewKind.Items)
            {
                if (item is Medium medium)
                {
                    if (medium.Id == (int)((Button)sender).CommandParameter)
                    {
                        id = medium.PublisherId;
                    }
                }
            }
            int i = 0;
            bool hasFoundElement = false;
            foreach (var item in comboBoxEnkel.Items)
            {
                if (((Publisher)item).Id == id)
                {
                    comboBoxEnkel.SelectedIndex = i;
                    hasFoundElement = true;
                    break;
                }
                i++;
            }
            if(!hasFoundElement)
                comboBoxEnkel.SelectedIndex = -1;
            tb1.Text = bezeichnung;
            tb2.Text = url;
            clickadd.Click -= Add;
            clickadd.Click += Edit;
            foreach (var item in listViewKind.Items)
            {
                if (item is Medium medium)
                {
                    if (medium.Id == (int)((Button)sender).CommandParameter)
                    {
                        qw = listViewKind.Items.IndexOf(medium);
                        itemid = medium.Id;
                    }
                }
            }
        }
        
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            this.logger.Log($"{nameof(Button_Click1)} called");

            int index = (int)((Button)sender).CommandParameter;
            int i = 0;
            while(i < listViewKind.Items.Count)
            {
                if (((Medium)listViewKind.Items[i]).Id == index)
                    break;

                i++;
            }
            delete.OnDeleteSomething(this, listViewKind.Items[i]);
            listViewKind.Items.RemoveAt(i);

            List<Medium> a = new List<Medium>();
            foreach (var item in listViewKind.Items)
            {
                a.Add((Medium)item);
            }
            MediaRepo.Media = a;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            this.logger.Log($"{nameof(Add)} called");

            Medium m = new Medium();
            m.Id = zahl++;
            m.Name = tb1.Text;
            m.Link = tb2.Text;
            try
            {
                m.PublisherId = ((Publisher)comboBoxEnkel.SelectedItem).Id;
            }
            catch(NullReferenceException ex)
            {
                return;
            }
            listViewKind.Items.Add(m);
            List<Medium> a = new List<Medium>();
            foreach (var item in listViewKind.Items)
            {
                a.Add((Medium)item);
            }
            MediaRepo.Media = a;
            tb1.Text = String.Empty;
            tb2.Text = tb1.Text;
            comboBoxEnkel.SelectedIndex = -1;
        }
        private void Edit(object sender, RoutedEventArgs e)
        {
            int id = qw;
            Medium item = new Medium();
                    ((Medium)item).Name = tb1.Text;
                    ((Medium)item).Link = tb2.Text;
                    ((Medium)item).PublisherId = ((Publisher)comboBoxEnkel.SelectedItem).Id;
            listViewKind.Items.RemoveAt(id);
            item.Id = itemid;
            listViewKind.Items.Insert(id, item);
                    clickadd.Click += Add;
                    clickadd.Click -= Edit;
            List<Medium> a = new List<Medium>();
            foreach (var x in listViewKind.Items)
            {
                a.Add((Medium)x);
            }
            MediaRepo.Media = a;
            tb1.Text = "";
            tb2.Text = "";
            comboBoxEnkel.SelectedIndex = -1;
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            tb1.Text = "";
            tb2.Text = "";
            comboBoxEnkel.SelectedIndex = -1;
        }
    }
}
