using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;

public class DatabaseConnexion : MonoBehaviour 
{
    private string databasePath;
    private SqliteConnection connexion;

    /// <summary>
    /// initialize connexion to database
    /// </summary>
    void initializeConnexion()
    {
        // path to database.
        this.databasePath = "URI=file:" + Application.dataPath + "/Database/arachneeDatabase.db";
        // creates connexion
        this.connexion = new SqliteConnection(databasePath);
    }

    /// <summary>
    /// takes a query and update this.reader with data from database
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public SqliteDataReader getReaderFromQuery(string query)
    {
        // create query
        SqliteCommand cmd = new SqliteCommand(this.connexion);
        cmd.CommandText = query;

        // execute query and update reader
        SqliteDataReader reader = cmd.ExecuteReader();

        // clear query
        cmd.Dispose();
        cmd = null;

        return reader;
    }
}
