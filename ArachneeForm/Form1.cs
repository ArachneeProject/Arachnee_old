using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Arachnee;

namespace ArachneeForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DatabaseConnection objConnect;  // connexion
        string conString;   // database path
        DataSet dataSet;    // contains database
        DataRow readRow;       // contains the current read row of the database

        int maxRows;
        int inc = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                objConnect = new DatabaseConnection();  // create connexion object
                conString = Properties.Settings.Default.moviesConnectionString; // path of database
                objConnect.connection_string = conString; // load path of database
                objConnect.sql = Properties.Settings.Default.SQL; // load standard query (select * from database)
                dataSet = objConnect.getConnection; // copy database into dataset
                maxRows = dataSet.Tables[0].Rows.Count;
                showRecord(); // read a row from database and show it
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        // fills the form with an entry
        private void showRecord()
        {
            readRow = dataSet.Tables[0].Rows[inc];

            textBoxTitle.Text = readRow.ItemArray.GetValue(1).ToString();
            textBoxDate.Text = readRow.ItemArray.GetValue(2).ToString();
            pictureBox.ImageLocation = "C:\\Users\\Valentin\\Arachnee\\SQLDatabase\\pictures\\"+readRow.ItemArray.GetValue(0).ToString()+".jpg";
        }

        private void textBoxTitle_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            buttonPrevious.Enabled = true;
            inc++;
            if (inc == maxRows - 1)
            {
                buttonNext.Enabled = false;
            }
            showRecord();
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            buttonNext.Enabled = true;
            inc--;
            if (inc == 0)
            {
                buttonPrevious.Enabled = false;
            }
            showRecord();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBoxDate_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelDate_Click(object sender, EventArgs e)
        {

        }

        

        private void btnCancel_Click(object sender, EventArgs e)
        {
            showRecord();
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnAdd.Enabled = true;
            if (inc > 0)
            {
                buttonPrevious.Enabled = true;
            }
            if (inc < maxRows-1)
            {
                buttonNext.Enabled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DataRow row = dataSet.Tables[0].NewRow();
            row[1] = textBoxTitle.Text;
            row[2] = textBoxDate.Text;
            dataSet.Tables[0].Rows.Add(row);

            try
            {
                objConnect.updateDataBase(dataSet);
                maxRows++;
                MessageBox.Show("Database successfuly updated.");
                showRecord();
                btnCancel.Enabled = false;
                btnSave.Enabled = false;
                btnAdd.Enabled = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            textBoxTitle.Clear();
            textBoxDate.Clear();
            pictureBox.ImageLocation = "";
            buttonNext.Enabled = false;
            buttonPrevious.Enabled = false;
            btnAdd.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
        }

        

    }
}
