using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;


namespace DatabaseEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.connection = new SQLiteConnection("URI=file:" + System.IO.Directory.GetCurrentDirectory() + "/../../../../Arachnee/Assets/Database/arachneeDatabase.db");

            initMovieSelectorMenu();
            initArtistSelectorMenu();
            initJobSelectorMenu();
        }
        
        private SQLiteConnection connection;

        private Dictionary<int, Int64> movieIds = new Dictionary<int, long>();
        private Dictionary<int, Int64> artistIds = new Dictionary<int, long>();


        #region selectorInit
        /// <summary>
        /// init the movie selector
        /// </summary>
        void initMovieSelectorMenu()
        {
            string sql = "SELECT * FROM 'Movies' ORDER BY title";
            SQLiteCommand command = new SQLiteCommand(sql, this.connection);

            this.connection.Open();
            SQLiteDataReader reader = command.ExecuteReader();
            int i = 0;
            while (reader.Read())
            {
                this.MovieSelector.Items.Add(reader["title"]);
                this.movieIds.Add(i++, (Int64) reader["id"]);
            }
            this.connection.Close();
        }

        /// <summary>
        /// init the artist selector
        /// </summary>
        void initArtistSelectorMenu()
        {
            string sql = "SELECT * FROM Artists ORDER BY first_name";
            SQLiteCommand command = new SQLiteCommand(sql, this.connection);

            this.connection.Open();
            SQLiteDataReader reader = command.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                string artist = (string)reader["first_name"] + " " + reader["last_name"];
                this.ArtistSelector.Items.Add(artist);
                this.artistIds.Add(i++, (Int64)reader["id"]);
            }
            this.connection.Close();
            this.ArtistSelector.SelectedIndex = -1;
        }

        /// <summary>
        /// init the job selector
        /// </summary>
        void initJobSelectorMenu()
        {
            this.JobSelector.Items.Add("Director"); // 0
            this.JobSelector.Items.Add("Actor");    // 1
        }

        #endregion selectorInit


        /// <summary>
        /// update the form when a movie is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MovieSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ArtistSelector.Enabled = true;
            this.JobSelector.Enabled = true;

            this.DirectorLabel.Text = this.getNames("Directors");
            this.ActorsLabel.Text = this.getNames("Actors");
        }

        /// <summary>
        /// return the string with the names of the artists implied in the movie selected
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string getNames(string tableName)
        {
            string res = "";
            var list = this.getArtistListFromQuery("SELECT id_artist FROM " + tableName + " WHERE id_movie=" + this.movieIds[this.MovieSelector.SelectedIndex]);
            foreach (string s in list)
            {
                res += s + " - ";
            }
            return res;
        }

        /// <summary>
        /// return a list of string
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private List<string> getArtistListFromQuery(string query)
        {
            List<string> list = new List<string>();

            try
            {
                SQLiteCommand command = new SQLiteCommand("SELECT first_name,last_name FROM Artists WHERE id IN (" + query + ")", this.connection);
                this.connection.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add((string)reader["first_name"] + reader["last_name"]);
                }
                reader.Dispose();
                this.connection.Close();
            }
            catch (Exception ex)
            {
                this.connection.Close();
                MessageBox.Show(ex.Message);
            }
            return list;
        }



        /// <summary>
        /// Update database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDatabase(object sender, EventArgs e)
        {
            if (this.ArtistSelector.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an artist");
                return;
            }

            string tableName = "";
            switch (this.JobSelector.SelectedIndex)
            {
                case 0:
                    tableName = "Directors";
                    break;
                case 1:
                    tableName = "Actors";
                    break;

                default:
                    MessageBox.Show("Nothing added");
                    return;
            }
            this.insertInDatabase(tableName, (int)this.artistIds[this.ArtistSelector.SelectedIndex], (int)this.movieIds[this.MovieSelector.SelectedIndex]);

            this.MovieSelector_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// insert (valueA,ValueB) in the table tableName of the database
        /// </summary>
        /// <param name="tableName"></param>
        private void insertInDatabase(string tableName, int valueA, int valueB)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand("INSERT INTO " + tableName + " values (" + valueA + "," + valueB + ")",this.connection);
                this.connection.Open();

                command.ExecuteNonQuery();
                
                this.connection.Close();
            }
            catch (Exception ex)
            {
                this.connection.Close();
                MessageBox.Show(ex.Message);
            }
        }



        
    }
}
