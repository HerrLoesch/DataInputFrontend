using DataInputt.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace DataInputt
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool Nothing = true;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                //First get the 'user-scoped' storage information location reference in the assembly
                IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();
                //create a stream reader object to read content from the created isolated location
                StreamReader srReader = new StreamReader(new IsolatedStorageFileStream("iso", FileMode.OpenOrCreate, isolatedStorage));

                //Open the isolated storage
                if (srReader == null)
                {
                    return;
                }
                else
                {
                    DataInputt.Properties.Settings.Default.ConnectionString = srReader.ReadLine();
                    DataInputt.Properties.Settings.Default.DBUsername = srReader.ReadLine();
                    DataInputt.Properties.Settings.Default.DBPassword = srReader.ReadLine();
                }
                //close reader
                srReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            MisterDeleteDB.connection?.Close();
            MisterDeleteDB.connection?.Dispose();
            try
            {

                //First get the 'user-scoped' storage information location reference in the assembly
                IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();
                //create a stream writer object to write content in the location
                StreamWriter srWriter = new StreamWriter(new IsolatedStorageFileStream("iso", FileMode.Create, isolatedStorage));
                //check the Application property collection contains any values.
                {
                    //write to the isolated storage created in the above code section.
                    srWriter.WriteLine(DataInputt.Properties.Settings.Default.ConnectionString);
                    srWriter.WriteLine(DataInputt.Properties.Settings.Default.DBUsername);
                    srWriter.WriteLine(DataInputt.Properties.Settings.Default.DBPassword);

                }

                srWriter.Flush();
                srWriter.Close();
            }
            catch (System.Security.SecurityException sx)
            {
                MessageBox.Show(sx.Message);
                throw;
            }
        }
        protected override void OnNavigated(NavigationEventArgs e)
        {
            base.OnNavigated(e);
            DataInputt.Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
            try
            {
                if (String.IsNullOrEmpty(DataInputt.Properties.Settings.Default.ConnectionString))
                    return;
                if (null == DataInputt.Properties.Settings.Default.DBUsername)
                    return;
                if (null == DataInputt.Properties.Settings.Default.DBPassword)
                    return;

                Nothing = false;
                if (DataInputt.Properties.Settings.Default.DBUsername == "")
                {
                    MisterDeleteDB.connection = new SqlConnection(DataInputt.Properties.Settings.Default.ConnectionString);
                    MisterDeleteDB.connection.Open();
                }
                else
                {

                    SecureString x = new SecureString();
                    foreach (char c in DataInputt.Properties.Settings.Default.DBPassword.ToCharArray())
                    {
                        x.AppendChar(c);
                    }

                    MisterDeleteDB.connection = new SqlConnection(DataInputt.Properties.Settings.Default.ConnectionString,
                        new SqlCredential(DataInputt.Properties.Settings.Default.DBUsername, x));
                    MisterDeleteDB.connection.Open();
                }
            }
            catch (Exception ex)
            {
            }
        }

        int x = 0;
        private void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(DataInputt.Properties.Settings.Default.ConnectionString))
                return;
            if (null == DataInputt.Properties.Settings.Default.DBUsername)
                return;
            if (null == DataInputt.Properties.Settings.Default.DBPassword)
                return;
            if (x < 2)
            {
                x++; return;
            }
            x = 0;
            if (DataInputt.Properties.Settings.Default.DBUsername == "")
            {
                try
                {
                    MisterDeleteDB.connection = new SqlConnection(DataInputt.Properties.Settings.Default.ConnectionString);
                    MisterDeleteDB.connection.Open();
                }
                catch
                {

                }
            }
            else
            {
                SecureString x = new SecureString();
                foreach (char c in DataInputt.Properties.Settings.Default.DBPassword.ToCharArray())
                {
                    x.AppendChar(c);
                }
                MisterDeleteDB.connection = new SqlConnection(DataInputt.Properties.Settings.Default.ConnectionString,
                    new SqlCredential(DataInputt.Properties.Settings.Default.DBUsername, x));
                MisterDeleteDB.connection.Open();
            }
        }
    }
}
