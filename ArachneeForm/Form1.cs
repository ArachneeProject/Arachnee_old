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
        DataSet dataset;    // contains database
         

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
