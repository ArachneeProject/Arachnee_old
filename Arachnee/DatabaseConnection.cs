using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arachnee
{
    public class DatabaseConnection
    {
        private string sql_string;  // holds sql query
        private string strCon;      // connexion path
        System.Data.SqlClient.SqlDataAdapter dataAdapter;

        public string sql
        {
            set { sql_string = value; }
        }

        public string connection_string
        {
            set { strCon = value; }
        }

        public System.Data.DataSet getConnection
        {
            get { return myDataSet(); }
        }

        /**
         * open connexion to database, copy it in a DataSet then return this DataSet
         * */
        private System.Data.DataSet myDataSet()
        {
            System.Data.SqlClient.SqlConnection connexion = new System.Data.SqlClient.SqlConnection(strCon);
            connexion.Open();
            dataAdapter = new System.Data.SqlClient.SqlDataAdapter(sql_string, connexion);
            System.Data.DataSet dataSet = new System.Data.DataSet();
            dataAdapter.Fill(dataSet);
            connexion.Close();

            return dataSet;
        }

        /**
         * update database
         * */
        public void updateDataBase(System.Data.DataSet dataSet)
        {
            System.Data.SqlClient.SqlCommandBuilder cb = new System.Data.SqlClient.SqlCommandBuilder(dataAdapter);
            cb.DataAdapter.Update(dataSet.Tables[0]);
        }
    }
}
