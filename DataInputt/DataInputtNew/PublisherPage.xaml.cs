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
    /// Interaktionslogik für PublisherPage.xaml
    /// </summary>
    public partial class PublisherPage : Page, INotifyPropertyChanged
    {
        private Delete delete;
        private int i = 1;
        public List<Publisher> publisherList;
        private Tuple<bool, int> bearbeiten = new Tuple<bool, int>(false, -1);

        public event PropertyChangedEventHandler PropertyChanged;

        public PublisherPage()
        {
            InitializeComponent();
            delete = Delete.GetInstance();
            publisherList = new List<Publisher>();
            //{
            //    new Publisher
            //    {
            //        Id = i++, Name = "Publisher1", Link = "abc.de"
            //    },
            //    new Publisher {Id=i++, Name="Publisher2", Link="def.de"}
            //};
            //foreach (var item in publisherList)
            //{
            //    publisherListView.Items.Add(item);
            //}
            PublisherRepo.PublisherCollectionImported += PublisherRepo_PublisherCollectionImported;
            PublisherRepo.Publisher = publisherList;
        }

        private void PublisherRepo_PublisherCollectionImported(object sender, EventArgs e)
        {
            publisherList = (List<Publisher>)sender;
            publisherListView.Items.Clear();
            int i = 0;
            foreach (var item in publisherList)
            {
                publisherListView.Items.Add(item);
                i++;
            }
        }

        private void Delete_SomethingDeleted(object sender, DeleteEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bearbeiten = new Tuple<bool, int>(true, publisherList.Find(p => p.Id == (int)((Button)sender).CommandParameter).Id);
            tb1.Text = publisherList.Find(p => p.Id == (int)((Button)sender).CommandParameter).Name;
            tb2.Text = publisherList.Find(p => p.Id == (int)((Button)sender).CommandParameter).Link;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            publisherList.Remove(publisherList.Find(p => p.Id == (int)((Button)sender).CommandParameter));
            foreach (var item in publisherListView.Items)
            {
                foreach (var item2 in publisherList)
                {
                    if (!publisherList.Contains((Publisher)item))
                    {
                        delete.OnDeleteSomething(this, (Publisher)item);
                    }
                }
            }
            publisherListView.Items.Clear();
            int i = 0;
            foreach (var item in publisherList)
            {
                publisherListView.Items.Add(item);
                i++;
            }
            PublisherRepo.Publisher = publisherList;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(bearbeiten.Item1 == true)
            {
                
                publisherList.RemoveAt(publisherList.FindIndex(p => p.Id == bearbeiten.Item2));
                publisherList.Add(new Publisher() { Id = bearbeiten.Item2, Name = tb1.Text, Link = tb2.Text });
                publisherList.Sort(new PublisherComparer());
                bearbeiten = new Tuple<bool, int>(false, -1);
            } else
            {
                publisherList.Add(new Publisher() { Id = i++, Name = tb1.Text, Link = tb2.Text });
            }
            publisherListView.Items.Clear();
            foreach (var item in publisherList)
            {
                publisherListView.Items.Add(item);
            }
            PublisherRepo.Publisher = publisherList;
            OnPropertyChanged("publisher");
            tb1.Text = String.Empty;
            tb2.Text = tb1.Text;
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            tb1.Text = "";
            tb2.Text = "";
        }
    }

    class PublisherComparer : IComparer<Publisher>
    {
        public int Compare(Publisher x, Publisher y)
        {
            if (x.Id != y.Id)
                return x.Id > y.Id ? 1 : -1;
            if (x.Name != y.Name)
                return StringComparer.Create(CultureInfo.CurrentCulture, true).Compare(x.Name, y.Name);
            if (x.Link != y.Link)
                return StringComparer.Create(CultureInfo.CurrentCulture, true).Compare(x.Link, y.Link);
            return 0;
        }
    }
}
