using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataInputt.Annotations;
using DataInputt.Models;

namespace DataInputt
{
    public class Publication : INotifyPropertyChanged
    {
        private string date;
        private string name;
        private string description;
        private string link;
        private string type;
        private int mediumId;
        private int id;
        private bool reviewed;

        public int Id
        {
            get { return id; }
            set
            {
                if (value == id) return;
                id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (value == name) return;
                name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                if (value == description) return;
                description = value;
                OnPropertyChanged();
            }
        }

        public string Date
        {
            get { return date; }
            set
            {
                if (value == date) return;
                date = value;
                OnPropertyChanged();
            }
        }

        public string Link
        {
            get { return link; }
            set
            {
                if (value == link) return;
                link = value;
                OnPropertyChanged();
            }
        }

        public string Type
        {
            get { return type; }
            set
            {
                if (value == type) return;
                type = value;
                OnPropertyChanged();
            }
        }

        public int MediumId
        {
            get { return mediumId; }
            set
            {
                if (value == mediumId) return;
                mediumId = value;
                OnPropertyChanged();
            }
        }

        public bool Reviewed
        {
            get { return reviewed; }
            set
            {
                if (value == reviewed) return;
                reviewed = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}