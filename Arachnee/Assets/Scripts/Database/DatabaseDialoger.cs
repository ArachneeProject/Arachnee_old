using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

public class DatabaseDialoger
{
    private SqliteConnection sqltConnection = null;

    /// <summary>
    /// Initializes the database connection
    /// </summary>
    /// <param name="databasePath"></param>
    public void Initialize(string databasePath)
    {
        this.sqltConnection = new SqliteConnection(databasePath);
    }


    /// <summary>
    /// Return dataset corresponding to query
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public DataSet GetDataSet(string query)
    {
        if (this.sqltConnection == null)
        {
            Logger.Trace("Database Dialoger not initialized!", LogLevel.Error);
            return new DataSet();
        }

        this.sqltConnection.Open();

        DataSet set = new DataSet();
        SqliteDataAdapter ad = new SqliteDataAdapter(query,this.sqltConnection);
        ad.Fill(set);

        this.sqltConnection.Close();

        return set;
    }

    /// <summary>
    /// Checki if the id exist in the given table
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public bool CheckIfEntryExistsInTable(Int64 id, string tableName)
    {
        return (this.GetDataSet("SELECT id FROM " + tableName + " WHERE id=\"" + id + "\" LIMIT 1;").Tables[0].Rows.Count != 0);
    }

    /// <summary>
    /// Check if the actor->movie connection exists in the given table
    /// </summary>
    /// <param name="artistId"></param>
    /// <param name="movieId"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public bool CheckIfAMConnectionExistsInTable(Int64 artistId, Int64 movieId, string tableName)
    {
        return (this.GetDataSet("SELECT id_artist FROM " + tableName + " WHERE id_artist=\"" + artistId + "\" AND id_movie=\"" + movieId + "\" LIMIT 1;").Tables[0].Rows.Count != 0);
    }


    /// <summary>
    /// Insert new values in database
    /// </summary>
    /// <param name="leftId"></param>
    /// <param name="rightId"></param>
    /// <param name="tableName"></param>
    internal void InsertConnection(Int64 leftId, Int64 rightId, string tableName)
    {
        if (this.sqltConnection == null)
        {
            Logger.Trace("Database Dialoger not initialized!", LogLevel.Error);
            return;
        }

        this.sqltConnection.Open();
        SqliteCommand insertSQL = new SqliteCommand("INSERT INTO " + tableName + " VALUES (" + leftId + "," + rightId + ")", this.sqltConnection);
        try 
        {
            insertSQL.ExecuteNonQuery();
        }
        catch (Exception ex) 
        {
            Logger.Trace(ex.Message, LogLevel.Error);
        }
        this.sqltConnection.Close();
    }

    internal void InsertMovie(int id, string title, Int64 date, string poster)
    {
        if (this.sqltConnection == null)
        {
            Logger.Trace("Database Dialoger not initialized!", LogLevel.Error);
            return;
        }

        this.sqltConnection.Open();
        SqliteCommand insertSQL = new SqliteCommand("INSERT INTO movies VALUES (" + id + ", \"" + title + "\", " + date +", \"" + poster + "\");", this.sqltConnection);
        try
        {
            insertSQL.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Logger.Trace("query: " + "INSERT INTO movies VALUES (" + id + ", \"" + title + "\", " + date + ");" + " - has a problem", LogLevel.Error);
            Logger.Trace(ex.Message, LogLevel.Error);
        }
        this.sqltConnection.Close();
    }

    internal void InsertArtist(int id, string firstName, string lastName)
    {
        if (this.sqltConnection == null)
        {
            Logger.Trace("Database Dialoger not initialized!", LogLevel.Error);
            return;
        }

        this.sqltConnection.Open();
        SqliteCommand insertSQL = new SqliteCommand("INSERT INTO artists VALUES (" + id + ", \""+ firstName  + "\", \"" + lastName + "\")", this.sqltConnection);
        try
        {
            insertSQL.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Logger.Trace(ex.Message, LogLevel.Error);
        }
        this.sqltConnection.Close();
    }
}
