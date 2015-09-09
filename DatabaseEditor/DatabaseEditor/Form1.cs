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
            initDirectorSelectorMenu();
        }
        
        private SQLiteConnection connection;

        /// <summary>
        /// init the movie selector
        /// </summary>
        void initMovieSelectorMenu()
        {
            string sql = "SELECT * FROM 'Movies'";
            SQLiteCommand command = new SQLiteCommand(sql, this.connection);

            this.connection.Open();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                this.MovieSelector.Items.Add(reader["title"]);
            }
            this.connection.Close();
        }

        /// <summary>
        /// init the director selector
        /// </summary>
        void initDirectorSelectorMenu()
        {
            string sql = "SELECT * FROM Artists";
            SQLiteCommand command = new SQLiteCommand(sql, this.connection);

            this.connection.Open();
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                this.DirectorSelector.Items.Add((string) reader["first_name"] + " " + reader["last_name"]);
            }
            this.connection.Close();


            this.DirectorSelector.SelectedIndex = -1;
        }

        /// <summary>
        /// update the form when a movie is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MovieSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DirectorSelector.Enabled = true;
            try
            {
                string directorIdQuery = "SELECT id_artist FROM Directors WHERE id_movie = " + (this.MovieSelector.SelectedIndex + 1);
                SQLiteCommand command = new SQLiteCommand(directorIdQuery, this.connection);
                this.connection.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    this.DirectorSelector.SelectedIndex = (int)((Int64)reader["id_artist"] - 1);
                    this.DirectorSelector.Enabled = false;
                }
                else
                {
                    this.DirectorSelector.SelectedIndex = -1;
                }
                this.connection.Close();
            }
            catch (Exception ex)
            {
                this.connection.Close();
                MessageBox.Show(ex.Message);
            }
        }




        /// <summary>
        /// Update database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDatabase(object sender, EventArgs e)
        {
            if (this.DirectorSelector.Enabled)
            {
                string sql = "INSERT INTO Directors values ("+(this.DirectorSelector.SelectedIndex+1) + ","+ (this.MovieSelector.SelectedIndex+1) + ")";
                SQLiteCommand command = new SQLiteCommand(sql, this.connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception exc)
                {
                    this.connection.Close();
                    MessageBox.Show(exc.Message);
                }

                this.MovieSelector_SelectedIndexChanged(sender, e);
            }
        }


        
    }
}
