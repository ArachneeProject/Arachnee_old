using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

public class DatabaseDialoger
{
    private SqliteConnection sqltConnection;

    /// <summary>
    /// Initializes the database connection
    /// </summary>
    /// <param name="databasePath"></param>
    public void Initialize(string databasePath)
    {
        this.sqltConnection = new SqliteConnection(databasePath);
    }


    /// <summary>
    /// Return 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public DataSet GetDataSet(string query)
    {
        this.sqltConnection.Open();

        DataSet set = new DataSet();
        SqliteDataAdapter ad = new SqliteDataAdapter(query,this.sqltConnection);
        ad.Fill(set);

        this.sqltConnection.Close();

        return set;
    }

}
