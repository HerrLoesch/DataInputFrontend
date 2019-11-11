using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Windows.Controls;
using DataInputt.Models;
using System;
using System.Data.SqlClient;
using DataInputt.DB;
using System.Data.SqlTypes;
using System.Globalization;
using System.Reflection;
using DataInputt.Logging;
using DataInputt.Properties;
using NLog;
using System.Linq;

namespace DataInputt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Delete delete;
        ItemCollection x = new ListView().Items;
        ItemCollection j = new ListView().Items;
        PublicationData data = new PublicationData();
        private readonly ILog fileLogger;
        
        private DataContext _context = new DataContext();

        public MainWindow()
        {
            InitializeComponent();
            this.delete = Delete.GetInstance();
            delete.SomethingDeleted += Delete_SomethingDeleted;
            foreach (var dataType in data.types)
            {
                this.typ.Items.Add(dataType);
            }

            
            Publication publication1 = new Publication()
            {
                Id = 1,
                Name = "Publikation1",
                Description = "My first publication",
                Date = "02.08.2017",
                Type = "Workshop",
                MediumId = 1
            };
            Publication publication2 = new Publication()
            {
                Id = 2,
                Name = "Publikation2",
                Description = "My second publication",
                Date = "03.08.2017",
                Type = "Usergroup",
                MediumId = 2
            };
            publikationen.Items.Add(publication1);
            publikationen.Items.Add(publication2);

            switch (Settings.Default.LoggingImpl)
            {
                case nameof(NLogFacade):
                    this.fileLogger = new NLogFacade(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Trace.log"));
                    break;

                case nameof(FileLog):
                    this.fileLogger =
                        new FileLog(
                            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Trace.log"),
                            LogLevel.Trace, true, true);
                    break;

                case nameof(EventLogger):
                    this.fileLogger = new EventLogger(Assembly.GetExecutingAssembly().GetName().Name);
                    break;
            }

            this.fileLogger.Write(new FileLogData
            {
                Message = "Application started",
                Severity = LogLevel.Info,
                Timestamp = DateTime.Now
            });
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            if (args == null)
            {
                return;
            }

            this.fileLogger.Write(new LogData
            {
                Message = $"Unhandled exception occurred: {args.ExceptionObject}",
                Severity = LogLevel.Fatal
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (this.fileLogger is FileLog fileLogger)
            {
                fileLogger.Dispose();
            }
        }

        private void Delete_SomethingDeleted(object sender, DeleteEventArgs e)
        {
            if(sender.GetType() == typeof(Page1))
            {
                if (e.Object is Medium medi)
                {
                    for(int i = 0; i < publikationen.Items.Count; i++)
                    {
                        var item = publikationen.Items[i];
                        if (((Publication)item).MediumId == medi.Id)
                        {
                            int j = publikationen.Items.IndexOf(item);
                            Publication ppub = new Publication();
                            ppub.Id = ((Publication)item).Id;
                            ppub.Name = ((Publication)item).Name;
                            ppub.Description = ((Publication)item).Description;
                            ppub.Date = ((Publication)item).Date;
                            ppub.Link = ((Publication)item).Link;
                            ppub.Reviewed = ((Publication)item).Reviewed;
                            ppub.Type = ((Publication)item).Type;
                            publikationen.Items.RemoveAt(j);
                            publikationen.Items.Insert(j, ppub);
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.id.Text))
            {
                object itemToDelete = null;
                foreach (var item in this.publikationen.Items)
                {
                    var publikation = (Publication) item;
                    if (publikation.Id == int.Parse(this.id.Text))
                    {
                        itemToDelete = publikation;
                    }
                }

                if (itemToDelete != null)
                {
                    this.publikationen.Items.Remove(itemToDelete);
                }
            }

            this.DatePicker.Text = "";
            this.Beschreibung.Text = "";
            this.id.Text = "";
            this.link.Text = "";
            this.name.Text = "";
            this.typ.Text = "";
            this.medium.SelectedIndex = -1;
        }

        protected virtual void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var publication = new Publication();

            if (!string.IsNullOrEmpty(this.id.Text))
            {
                var id = int.Parse(this.id.Text);
                Publication foundPublication = null;
                foreach (Publication item in this.publikationen.Items)
                {
                    if (item.Id == id)
                    {
                        foundPublication = item;
                    }
                }

                foundPublication.Link = this.link.Text;
                foundPublication.Date = this.DatePicker.Text;
                foundPublication.Description = this.Beschreibung.Text;
                foundPublication.Name = this.name.Text;
                foundPublication.Type = this.typ.Text;
                try
                {
                    foundPublication.MediumId = ((Medium)this.medium.SelectedItem).Id;
                }
                catch (NullReferenceException ex)
                {
                    return;
                }

                this.fileLogger.Write(new LogData
                {
                    Message = $"Updated publication '{foundPublication.Name}'",
                    Severity = LogLevel.Info
                });
            }
            else
            {
                publication.Id = GetId();

                publication.Link = this.link.Text;
                publication.Date = this.DatePicker.Text;
                publication.Description = this.Beschreibung.Text;
                publication.Name = this.name.Text;
                publication.Type = this.typ.Text;
                try
                {
                    publication.MediumId = ((Medium)this.medium.SelectedItem).Id;
                }
                catch(NullReferenceException ex)
                {
                    return;
                }

                this.publikationen.Items.Add(publication);
            }
            this.Beschreibung.Text = "";
            this.DatePicker.SelectedDate = null;
            this.link.Text = "";
            this.medium.SelectedIndex = -1;
            this.name.Text = "";
            this.typ.SelectedIndex = -1;
        }

        private int GetId()
        {
            if (!string.IsNullOrEmpty(this.id.Text))
            {
                return int.Parse(this.id.Text);
            }

            int highestId = 0;
            foreach (var item in this.publikationen.Items)
            {
                var publikation = (Publication)item;
                if (highestId < publikation.Id)
                {
                    highestId = publikation.Id;
                }
            }

            return ++highestId;
        }

        private void publikationen_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var publication = (Publication)this.publikationen.SelectedItem;

            if (publication == null)
            {
                return;
            }

            this.link.Text = publication.Link;
            this.DatePicker.Text = publication.Date;
            this.Beschreibung.Text = publication.Description;
            this.name.Text = publication.Name;
            this.typ.Text = publication.Type;
            Medium medi = null;
            foreach(var item in this.medium.Items)
            {
                if (((Medium)item).Id == publication.MediumId)
                {
                    medi = (Medium)item;
                    break;
                }
            }
            this.medium.SelectedItem = medi != null ? medi : null;
            this.id.Text = publication.Id.ToString();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var publicationData = new PublicationData();
            publicationData.Publications = new List<Publication>();

            foreach (var item in this.publikationen.Items)
            {
                var publikation = (Publication) item;

                publicationData.Publications.Add(publikation);
            }
            
            var saveFileDialog = new SaveFileDialog();
            var showDialog = saveFileDialog.ShowDialog(this);
            saveFileDialog.DefaultExt = "";

            if (showDialog == true)
            {
                string json = JsonConvert.SerializeObject(publicationData, Formatting.Indented);
                MediaRepoInstance mri = new MediaRepoInstance();
                mri.Media = MediaRepo.Media;
                string json2 = JsonConvert.SerializeObject(mri, Formatting.Indented);
                PublisherRepoInstance pri = new PublisherRepoInstance();
                pri.Publisher = PublisherRepo.Publisher;
                string json3 = JsonConvert.SerializeObject(pri, Formatting.Indented);
                ProjectRepoInstance prori = new ProjectRepoInstance();
                prori.Projects = ProjectRepo.Projects;
                string json4 = JsonConvert.SerializeObject(prori, Formatting.Indented);
                string directoryPath = Path.GetDirectoryName(saveFileDialog.FileName);
                string folderName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                Directory.CreateDirectory(directoryPath + "\\" + folderName);
                File.WriteAllText(directoryPath + "\\" + folderName + "\\publications.json", json);
                File.WriteAllText(directoryPath + "\\" + folderName + "\\media.json", json2);
                File.WriteAllText(directoryPath + "\\" + folderName + "\\publisher.json", json3);
                File.WriteAllText(directoryPath + "\\" + folderName + "\\projects.json", json4);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            var showDialog = openFileDialog.ShowDialog();
            if (showDialog == true)
            {
                var json = File.ReadAllText(openFileDialog.FileName);
                Do(json);
            }
        }

        private void Do(string json)
        {
            if (json.Contains("\"Publications\": ["))
            {
                this.publikationen.Items.Clear();
                if (json.Contains("\"PublisherUrl\":"))
                {
                    var publicationData = JsonConvert.DeserializeObject<OldPublicationData>(json);

                    MediaRepo.Media.Clear();
                    PublisherRepo.Publisher.Clear();
                    this.publikationen.Items.Clear();

                    foreach (var publication in publicationData.Publications)
                    {
                        Publisher publ = new Publisher();
                        publ.Id = PublisherRepo.Publisher.Count + 1;
                        publ.Name = publication.Publisher;
                        publ.Link = publication.PublisherUrl;
                        PublisherRepo.Publisher.Add(publ);

                        Medium med = new Medium();
                        med.Id = MediaRepo.Media.Count + 1;
                        med.Name = publication.Medium;
                        med.PublisherId = publ.Id;
                        MediaRepo.Media.Add(med);

                        Publication pub = new Publication();
                        pub.Id = publication.Id;
                        pub.Name = publication.Name;
                        pub.Description = publication.Description;
                        pub.Date = publication.Date;
                        pub.Link = publication.Link;
                        pub.Reviewed = false;
                        pub.Type = publication.Type;
                        pub.MediumId = med.Id;

                        this.publikationen.Items.Add(pub);

                        PublisherRepo.OnPublisherCollectionImport();
                        MediaRepo.OnMediaCollectionImport();
                    }
                }
                else
                {
                    //TODO Check if table exist, drop and create new
                    var publicationData = JsonConvert.DeserializeObject<PublicationData>(json);

                    foreach (var publication in publicationData.Publications)
                    {
                        this.publikationen.Items.Add(publication);
                    }
                }
            }
            else if (json.Contains("\"Publisher\": ["))
            {
                PublisherRepo.Publisher.Clear();
                var publicationData = JsonConvert.DeserializeObject<PublisherRepoInstance>(json);
                PublisherRepo.Publisher = publicationData.Publisher;
                PublisherRepo.OnPublisherCollectionImport();
            }
            else if (json.Contains("\"Media\": ["))
            {
                MediaRepo.Media.Clear();
                var publicationData = JsonConvert.DeserializeObject<MediaRepoInstance>(json);
                MediaRepo.Media = publicationData.Media;
                MediaRepo.OnMediaCollectionImport();
            }
            else if (json.Contains("\"Projects\": ["))
            {
                var publicationData = JsonConvert.DeserializeObject<ProjectRepoInstance>(json);
                ProjectRepo.Projects = publicationData.Projects;
                ProjectRepo.OnProjectCollectionImport();
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            this.DatePicker.Text = "";
            this.Beschreibung.Text = "";
            this.id.Text = "";
            this.link.Text = "";
            this.name.Text = "";
            this.typ.Text = "";
            this.medium.SelectedIndex = -1;
        }

        private void tabCtrl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(ComboBox) || e.OriginalSource.GetType() == typeof(ListView))
                return;

            if (e.RemovedItems.Count > 0 && (string)((TabItem)e.RemovedItems[0]).Header == "Publisher")
            {
                x = ((ListView)((Grid)((Grid)((Grid)((PublisherPage)((Frame)((TabItem)e.RemovedItems[0]).Content).Content).Content).Children[1]).Parent).Children[0]).Items;
            }
            if (e.RemovedItems.Count > 0 && (string)((TabItem)e.RemovedItems[0]).Header == "Media")
            {
                j = ((ListView)((StackPanel)((Page1)((Frame)((TabItem)e.RemovedItems[0]).Content).Content).Content).Children[0]).Items;
            }
            if (e.AddedItems.Count > 0 && 5 == 5 && (string)((TabItem) e.AddedItems[0]).Header == "Media")
            {
                ((ComboBox)((StackPanel)((StackPanel)((Page1)((Frame)((TabItem)e.AddedItems[0]).Content).Content).Content).Children[3]).Children[1]).ItemsSource = x;
            }
            if (e.AddedItems.Count > 0 && (string)((TabItem)e.AddedItems[0]).Header == "Publikationen")
            {
                ((ComboBox)((Grid)((TabItem)e.AddedItems[0]).Content).Children[12]).ItemsSource = j;
            }
        }

        private void ImportDB(object sender, EventArgs e)
        {
            inputField.Visibility = Visibility.Visible;
            cStgring1.Text = Properties.Settings.Default.DBUsername ?? "";
            cStgring2.Text = Properties.Settings.Default.DBPassword ?? "";
            cStgring3.Text = Properties.Settings.Default.ConnectionString ?? "";
            this.command = "Import";
        }
        private void ImportDB1()
        {
            // read publications
            string q = @"SELECT Id, Bezeichnung AS 'Name', Beschreibung AS 'Description', Datum AS 'Date', Link, Typ AS 'Type', MediumId, Geprueft AS Reviewed FROM Publikationen FOR JSON PATH;";
            SqlCommand commandX = new SqlCommand(q, MisterDeleteDB.connection);
            SqlDataReader result = commandX.ExecuteReader();
            if(result.Read())
            {
                string json = result.GetFieldValue<string>(0);
                int year = 1990;
                while (year <= DateTime.Today.Year)
                {
                    int index = json.IndexOf("\"Date\":\"" + year + "-");
                    while (index > -1)
                    {
                        int index2 = index + 8;
                        string date = json.Substring(index2, 10);
                        string date2 = FormatDate(date);
                        json = json.Replace(date, date2);
                        index = json.IndexOf("\"Date\":\"" + year + "-");
                    }
                    year++;
                }
                string before = "{\"types\": [\"Workshop\",\"Vortrag\",\"Artikel\",\"Videotraining\",\"Usergroup\",\"Buch\"],\"Publications\": ";
                string after = "}";
                Do(before + json + after);
            }            
            result.Close();

            // read media
            string q2 = @"SELECT Id, Bezeichnung AS 'Name', Link, PublisherId FROM Medien FOR JSON PATH;";
            SqlCommand command2 = new SqlCommand(q2, MisterDeleteDB.connection);
            SqlDataReader result2 = command2.ExecuteReader();
            if (result2.Read())
            {
                string json2 = result2.GetFieldValue<string>(0);
                string before2 = "{\"Media\": ";
                string after2 = "}";
                Do(before2 + json2 + after2);
            }
            result2.Close();

            // read publisher data
            string q3 = @"SELECT Id, Bezeichnung AS 'Name', Link FROM Publisher FOR JSON PATH;";
            SqlCommand command3 = new SqlCommand(q3, MisterDeleteDB.connection);
            SqlDataReader result3 = command3.ExecuteReader();
            if(result3.Read())
            {
            string json3 = result3.GetFieldValue<string>(0);
            string before3 = "{\"Publisher\": ";
            string after3 = "}";
            Do(before3 + json3 + after3);
            }
            result3.Close();

            // read projects
            string q4 = @"SELECT Id, Bezeichnung AS Abstract, Position, Von AS 'From', Bis AS 'To', BisHeute AS UntilToday, Beschreibung AS 'Description', Branche AS Sector FROM Projekte FOR JSON PATH;";
            SqlCommand command4 = new SqlCommand(q4, MisterDeleteDB.connection);
            SqlDataReader result4 = command4.ExecuteReader();
            if (result4.Read())
            {
                List<Project> projects = new List<Project>();
                string json4 = result4.GetFieldValue<string>(0);
                result4.Close();
                var publicationData = JsonConvert.DeserializeObject<ProjectRepoInstanceWithoutCollections>("{\"Projects\": " + json4 + "}");
                foreach (var item in publicationData.Projects)
                {
                    List<string> tasks = new List<string>();
                    List<string> tools = new List<string>();
                    Project p = new Project();
                    p.Abstract = item.Abstract;
                    p.Description = item.Description;
                    p.From = item.From;
                    p.Id = item.Id;
                    p.Position = item.Position;
                    p.Sector = item.Sector;
                    p.To = item.To;
                    p.UntilToday = item.UntilToday;

                    string query1 = $"SELECT Aufgaben.Bezeichnung AS Aufgabe FROM Projekte JOIN Projekte_Aufgaben ON Projekte_Aufgaben.ProjektId = Projekte.Id JOIN Aufgaben ON Aufgaben.Id = Projekte_Aufgaben.AufgabeId WHERE Projekte.Id = {item.Id};";
                    SqlDataReader dataReader = new SqlCommand(query1, MisterDeleteDB.connection).ExecuteReader();
                    while (dataReader.Read())
                    {
                        string aufgabe = (string)dataReader["Aufgabe"];
                        tasks.Add(aufgabe);
                    }
                    p.Tasks = tasks.ToArray();
                    dataReader.Close();

                    string query2 = $"SELECT Werkzeuge.Bezeichnung AS Werkzeug FROM Projekte JOIN Projekte_Werkzeuge ON Projekte_Werkzeuge.ProjektId = Projekte.Id JOIN Werkzeuge ON Werkzeuge.Id = Projekte_Werkzeuge.WerkzeugId WHERE Projekte.Id = {item.Id}; ";
                    SqlDataReader dataReader2 = new SqlCommand(query2, MisterDeleteDB.connection).ExecuteReader();
                    while (dataReader2.Read())
                    {
                        string tool = (string)dataReader2["Werkzeug"];
                        tools.Add(tool);
                    }
                    p.Tools = tools.ToArray();
                    dataReader2.Close();

                    projects.Add(p);
                }
                ProjectRepo.Projects = projects;
                ProjectRepo.OnProjectCollectionImport();
            }
        }

        private string FormatDate(string dateUS)
        {
            string day = dateUS.Substring(8, 2);
            string month = dateUS.Substring(5, 2);
            string year = dateUS.Substring(0, 4);
            return $"{day}.{month}.{year}";
        }

        private void ExPorterDB(object sender, EventArgs e)
        {
            inputField.Visibility = Visibility.Visible;
            cStgring1.Text = Properties.Settings.Default.DBUsername ?? "";
            cStgring2.Text = Properties.Settings.Default.DBPassword ?? "";
            cStgring3.Text = Properties.Settings.Default.ConnectionString ?? "";
            this.command = "Export";
        }
        private void ExPorterDB1()
        {
            var x = MisterDeleteDB.connection.GetSchema("TABLES");
            var y = MisterDeleteDB.connection.GetSchema("TABLES", new string[] { null, null, "Publisher" });
            if (MisterDeleteDB.connection.GetSchema("TABLES", new string[] { null, null, "Publisher" }).Rows.Count <= 0)
            {
                SqlCommand createPublisher = MisterDeleteDB.connection.CreateCommand();
                createPublisher.CommandText = "CREATE TABLE Publisher (" +
                    "Id int IDENTITY(1, 1) PRIMARY KEY, " +
                    "Bezeichnung varchar(30) NOT NULL, " +
                    "Link varchar(255), " +
                ");";
                createPublisher.ExecuteNonQuery();
            }

            if (MisterDeleteDB.connection.GetSchema("TABLES", new string[] { null, null, "Medien" }).Rows.Count <= 0)
            {
                SqlCommand crateMedien = MisterDeleteDB.connection.CreateCommand();
                crateMedien.CommandText = "CREATE TABLE Medien (" +
                    "Id int IDENTITY(1, 1) PRIMARY KEY, " +
                    "Bezeichnung varchar(30) NOT NULL, " +
                    "Link varchar(255), " +
                    "PublisherId int" +
                ");";
                crateMedien.ExecuteNonQuery();
            }

            if (MisterDeleteDB.connection.GetSchema("TABLES", new string[] { null, null, "Publikationen" }).Rows.Count <= 0)
            {
                SqlCommand createPublikationen = MisterDeleteDB.connection.CreateCommand();
                createPublikationen.CommandText = "CREATE TABLE Publikationen (" +
                    "Id int IDENTITY(1, 1) PRIMARY KEY, " +
                    "Bezeichnung varchar(30) NOT NULL, " +
                    "Datum date NOT NULL, " +
                    "Beschreibung varchar(400), " + 
                    "Link varchar(255), " +
                    "Geprueft bit NOT NULL, " +
                    "Typ varchar(20) NOT NULL, " + 
                    "MediumId int, " +
                    "CHECK ([Typ]='Buch' OR [Typ]='Usergroup' OR [Typ]='Videotraining' OR [Typ]='Artikel' OR [Typ]='Vortrag' OR [Typ]='Workshop')" + 
                ");";
                createPublikationen.ExecuteNonQuery();
            }

            if (MisterDeleteDB.connection.GetSchema("TABLES", new string[] { null, null, "Projekte" }).Rows.Count <= 0)
            {
                SqlCommand createPublikationen = MisterDeleteDB.connection.CreateCommand();
                createPublikationen.CommandText = "CREATE TABLE Projekte (" +
                    "Id int IDENTITY(1, 1) PRIMARY KEY, " +
                    "Bezeichnung varchar(30) NOT NULL, " +
                    "Position varchar(64), " +
                    "Von varchar(32), " +
                    "Bis varchar(32), " +
                    "BisHeute bit, " + 
                    "Beschreibung varchar(400), " +
                    "Branche varchar(64), " +
                ");";
                createPublikationen.ExecuteNonQuery();

                SqlCommand createTasks = MisterDeleteDB.connection.CreateCommand();
                createTasks.CommandText = "CREATE TABLE Aufgaben (" +
                    "Id int IDENTITY(1, 1) PRIMARY KEY, " +
                    "Bezeichnung varchar(256) NOT NULL);";
                createTasks.ExecuteNonQuery();

                SqlCommand createTasksProjects = MisterDeleteDB.connection.CreateCommand();
                createTasksProjects.CommandText = "CREATE TABLE Projekte_Aufgaben (" +
                    "Id int IDENTITY(1, 1) PRIMARY KEY, " +
                    "ProjektId int NOT NULL, " +
                    "AufgabeId int NOT NULL);";
                createTasksProjects.ExecuteNonQuery();

                SqlCommand createTools = MisterDeleteDB.connection.CreateCommand();
                createTools.CommandText = "CREATE TABLE Werkzeuge (" +
                    "Id int IDENTITY(1, 1) PRIMARY KEY, " +
                    "Bezeichnung varchar(256) NOT NULL);";
                createTools.ExecuteNonQuery();

                SqlCommand createToolsProjects = MisterDeleteDB.connection.CreateCommand();
                createToolsProjects.CommandText = "CREATE TABLE Projekte_Werkzeuge (" +
                    "Id int IDENTITY(1, 1) PRIMARY KEY, " +
                    "ProjektId int NOT NULL, " +
                    "WerkzeugId int NOT NULL);";
                createToolsProjects.ExecuteNonQuery();
            }

            new SqlCommand("DELETE FROM Publikationen;", MisterDeleteDB.connection).ExecuteNonQuery();
            foreach (Publication p in this.publikationen.Items)
            {
                int myBit = p.Reviewed ? 1 : 0;
                string q = $"INSERT INTO Publikationen (Bezeichnung, Beschreibung, Datum, Link, Typ, MediumId, Geprueft) VALUES ('{p.Name}', '{p.Description}', '{DateTime.ParseExact(p.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}', '{p.Link}', '{p.Type}', {p.MediumId}, {myBit});";
                SqlCommand c = new SqlCommand(q, MisterDeleteDB.connection);
                c.ExecuteNonQuery();
            }

            new SqlCommand("DELETE FROM Medien;", MisterDeleteDB.connection).ExecuteNonQuery();
            foreach (Medium m in MediaRepo.Media)
            {
                string q = $"INSERT INTO Medien (Bezeichnung, Link, PublisherId) VALUES ('{m.Name}', '{m.Link}', {m.PublisherId});";
                SqlCommand c = new SqlCommand(q, MisterDeleteDB.connection);
                c.ExecuteNonQuery();
            }

            new SqlCommand("DELETE FROM Publisher;", MisterDeleteDB.connection).ExecuteNonQuery();
            foreach (Publisher p in PublisherRepo.Publisher)
            {
                string q = $"INSERT INTO Publisher (Bezeichnung, Link) VALUES ('{p.Name}', '{p.Link}');";
                SqlCommand c = new SqlCommand(q, MisterDeleteDB.connection);
                c.ExecuteNonQuery();
            }

            new SqlCommand("DELETE FROM Projekte;", MisterDeleteDB.connection).ExecuteNonQuery();
            new SqlCommand("DELETE FROM Aufgaben;", MisterDeleteDB.connection).ExecuteNonQuery();
            new SqlCommand("DELETE FROM Werkzeuge;", MisterDeleteDB.connection).ExecuteNonQuery();
            new SqlCommand("DELETE FROM Projekte_Aufgaben;", MisterDeleteDB.connection).ExecuteNonQuery();
            new SqlCommand("DELETE FROM Projekte_Werkzeuge;", MisterDeleteDB.connection).ExecuteNonQuery();


            foreach (Project item in ProjectRepo.Projects)
            {
                int myBit = item.UntilToday ? 1 : 0;
                string q = $"INSERT INTO Projekte (Bezeichnung, Position, Von, Bis, BisHeute, Beschreibung, Branche) VALUES ('{item.Abstract}', '{item.Position}', '{item.From}', '{item.To}', {myBit}, '{item.Description}', '{item.Sector}');";
                new SqlCommand(q, MisterDeleteDB.connection).ExecuteNonQuery();
                string q2 = $"SELECT Id FROM Projekte WHERE Bezeichnung = '{item.Abstract}' and Von = '{item.From}';";
                SqlDataReader reader = new SqlCommand(q2, MisterDeleteDB.connection).ExecuteReader();
                reader.Read();
                int index = (int)reader["Id"];
                reader.Close();
                foreach (string tool in item.Tools)
                {
                    string q3 = $"SELECT Id FROM Werkzeuge WHERE Bezeichnung ='{tool}';";
                    SqlDataReader reader2 = new SqlCommand(q3, MisterDeleteDB.connection).ExecuteReader();
                    if (reader2.Read())
                    {
                        int index2 = (int)reader2["Id"];
                        reader2.Close();
                        string q4 = $"INSERT INTO Projekte_Werkzeuge (ProjektId, WerkzeugId) VALUES ({index}, {index2});";
                        new SqlCommand(q4, MisterDeleteDB.connection).ExecuteNonQuery();
                    }
                    else
                    {
                        reader2.Close();
                        string q5 = $"INSERT INTO Werkzeuge (Bezeichnung) VALUES ('{tool}');";
                        new SqlCommand(q5, MisterDeleteDB.connection).ExecuteNonQuery();
                        string q6 = $"SELECT Id FROM Werkzeuge WHERE Bezeichnung ='{tool}';";
                        SqlDataReader reader3 = new SqlCommand(q6, MisterDeleteDB.connection).ExecuteReader();
                        reader3.Read();
                        int index3 = (int)reader3["Id"];
                        reader3.Close();
                        string q7 = $"INSERT INTO Projekte_Werkzeuge (ProjektId, WerkzeugId) VALUES ({index}, {index3});";
                        new SqlCommand(q7, MisterDeleteDB.connection).ExecuteNonQuery();
                    }
                }
                foreach (string tool in item.Tasks)
                {
                    string q8 = $"SELECT Id FROM Aufgaben WHERE Bezeichnung ='{tool}';";
                    SqlDataReader reader4 = new SqlCommand(q8, MisterDeleteDB.connection).ExecuteReader();
                    if (reader4.Read())
                    {
                        int index4 = (int)reader4["Id"];
                        string q9 = $"INSERT INTO Projekte_Aufgaben (ProjektId, AufgabeId) VALUES ({index}, {index4});";
                        reader4.Close();
                        new SqlCommand(q9, MisterDeleteDB.connection).ExecuteNonQuery();
                    }
                    else
                    {
                        reader4.Close();
                        string q10 = $"INSERT INTO Aufgaben (Bezeichnung) VALUES ('{tool}');";
                        new SqlCommand(q10, MisterDeleteDB.connection).ExecuteNonQuery();
                        string q11 = $"SELECT Id FROM Aufgaben WHERE Bezeichnung ='{tool}';";
                        SqlDataReader reader5 = new SqlCommand(q11, MisterDeleteDB.connection).ExecuteReader();
                        reader5.Read();
                        int index5 = (int)reader5["Id"];
                        string q12 = $"INSERT INTO Projekte_Aufgaben (ProjektId, AufgabeId) VALUES ({index}, {index5});";
                        reader5.Close();
                        new SqlCommand(q12, MisterDeleteDB.connection).ExecuteNonQuery();
                    }
                }
                // GET Index of Task and Tool
                // SET In Table Politik_Task und Politik_Tool
            }
        }

        private string command;
        private void connect(object sender, EventArgs e)
        {
            inputField.Visibility = Visibility.Hidden;
            string user = cStgring1.Text;
            string pass = cStgring2.Text;
            string cStr = cStgring3.Text;
            Properties.Settings.Default.DBUsername = user;
            Properties.Settings.Default.DBPassword = pass;
            Properties.Settings.Default.ConnectionString = cStr;

            inputField.Visibility = Visibility.Collapsed;
            if (command == "Import")
            {
                ImportDB1();
            } else if (command == "Export")
            {
                ExPorterDB1();
            } else if (command == "ImportEF")
            {
                ImportDB2();
            } else if (command == "ExportEF")
            {
                ExPorterDB2();
            }
        }

        private void ImportDB_EF(object sender, RoutedEventArgs e)
        {
            ImportDB2();
            /*inputField.Visibility = Visibility.Visible;
            cStgring1.Text = Properties.Settings.Default.DBUsername ?? "";
            cStgring2.Text = Properties.Settings.Default.DBPassword ?? "";
            cStgring3.Text = Properties.Settings.Default.ConnectionString ?? "";
            this.command = "ImportEF";*/
        }
        private void ImportDB2()
        {
            // Read DB
            using (var db = new DataContext())
            {
                // Drop DB
                // db.Database.Delete();

                // Init Database for EF
                if (!db.Publications.Any() && !db.Media.Any() && !db.Publishers.Any() && !db.Projects.Any())
                    InitEFDb();

                // Import Publication
                // Display all Publications from the database
                var query = from b in db.Publications
                            orderby b.Id
                            select b;

                Console.WriteLine("Read all Publications in the database...");
                this.publikationen.Items.Clear();
                foreach (var item in query)
                {
                    this.publikationen.Items.Add(item);
                }

                // Import Media
                // Display all Media from the database
                var query2 = from b in db.Media
                            orderby b.Id
                            select b;

                Console.WriteLine("Read all Media in the database...");
                MediaRepo.Media.Clear();
                foreach (var item in query2)
                {
                    MediaRepo.Media.Add(item);
                }
                MediaRepo.OnMediaCollectionImport();

                // Import Publishers
                // Display all Publishers from the database
                var query3 = from b in db.Publishers
                             orderby b.Id
                             select b;

                Console.WriteLine("Read all Publishers in the database...");
                PublisherRepo.Publisher.Clear();
                foreach (var item in query3)
                {
                    PublisherRepo.Publisher.Add(item);
                }
                PublisherRepo.OnPublisherCollectionImport();

                // Import Projects
                // Display all Projects from the database
                var query4 = from b in db.Projects
                             orderby b.Id
                             select b;

                Console.WriteLine("Read all Projects in the database...");
                ProjectRepo.Projects.Clear();
                foreach (var item in query4)
                {
                    ProjectRepo.Projects.Add(item);
                }
                ProjectRepo.OnProjectCollectionImport();
            }

            Console.WriteLine("Import Database with EF completed!");
        }
        private void InitEFDb()
        {
            using (var db = new DataContext())
            {
                var pub = new Publication { Name = "Publikation1", Description = "1. Publication", Date = "10.11.2019", Link = null, Type = "Vortrag", MediumId = 1, Reviewed = false };
                var pub2 = new Publication { Name = "Publikation2", Description = "2. Publication", Date = "10.11.2019", Link = null, Type = "Vortrag", MediumId = 2, Reviewed = true };
                db.Publications.Add(pub);
                db.Publications.Add(pub2);

                var med = new Medium { Name = "Media3", Link = null, PublisherId = 1 };
                var med2 = new Medium { Name = "Medium4", Link = null, PublisherId = 2 };
                db.Media.Add(med);
                db.Media.Add(med2);

                var publ = new Publisher { Name = "Publisher1", Link = null };
                var publ2 = new Publisher { Name = "Publisher1", Link = null };
                db.Publishers.Add(publ);
                db.Publishers.Add(publ2);

                var pro = new Project { Abstract = "Projekt1", Position = "Position1", From = "01.10.2019", To = "20.10.2019", UntilToday = false, Description = "Done", Tasks = new string[] { "Write code", "Read code", "Delete code" }, Tools = new string[] { "Visual Studio" }, Sector = "Autos" };
                var pro2 = new Project { Abstract = "Projekt2", Position = "Position1", From = "02.11.2019", To = "22.11.2019", UntilToday = true, Description = "A Bad Project", Tasks = new string[] { "Clean Up", "Debug" }, Tools = new string[] { "Jetbrains", "Visual Studio" }, Sector = "Autos" };
                db.Projects.Add(pro);
                db.Projects.Add(pro2);

                db.SaveChanges();
            }
        }

        private void ExPorterDB_EF(object sender, RoutedEventArgs e)
        {
            ExPorterDB2();
            /*inputField.Visibility = Visibility.Visible;
            cStgring1.Text = Properties.Settings.Default.DBUsername ?? "";
            cStgring2.Text = Properties.Settings.Default.DBPassword ?? "";
            cStgring3.Text = Properties.Settings.Default.ConnectionString ?? "";
            this.command = "ExportEF";*/
        }
        private void ExPorterDB2()
        {
            using (var db = new DataContext())
            {
                db.Database.Delete();
                foreach (Publication p in this.publikationen.Items)
                {
                    //Console.WriteLine(p.Id + " " + p.Name);
                    db.Publications.Add(p);
                }
                foreach (Medium m in MediaRepo.Media)
                {
                    db.Media.Add(m);
                }
                foreach (Publisher p in PublisherRepo.Publisher)
                {
                    db.Publishers.Add(p);
                }
                foreach (Project p in ProjectRepo.Projects)
                {
                    db.Projects.Add(p);
                }
                db.SaveChanges();
            }

            Console.WriteLine("Export Database with EF completed!");
        }

    }

    class ProjectRepoInstanceWithoutCollections
    {
        public List<ProjectWithoutCollections> Projects { get; set; }
    }

    class ProjectWithoutCollections
    {
        public int Id { get; set; }
        public string Abstract { get; set; }
        public string Position { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public bool UntilToday { get; set; }
        public string Description { get; set; }
        public string Sector { get; set; }
    }
}
