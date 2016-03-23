using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

public class DatabaseDialoger
{
    private SqliteConnection sqltConnection = null;

    /// <summary>
    /// Initializes the database connection
    /// </summary>
    /// <param name="databasePath"></param>
    public void Initialize()
    {
        string databasePath = "URI=file:" + Application.dataPath + "/Database/arachneeDatabase.db";
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
    /// Check if the specified value in the specified column exists in the specified table
    /// </summary>
    /// <param name="columnName"></param>
    /// <param name="valueToCheck"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public bool CheckIfEntryExistsInTable(string columnName, string valueToCheck, string tableName)
    {
        return (this.GetDataSet("SELECT * FROM " + tableName + " WHERE " + columnName + "=\"" + valueToCheck + "\" LIMIT 1;").Tables[0].Rows.Count != 0);
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
    public void InsertConnection(Int64 leftId, Int64 rightId, string tableName)
    {
        if (this.sqltConnection == null)
        {
            Debug.LogError("not init");
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
            Debug.LogError(ex.Message + " (on " + leftId + " - " + rightId + ")");
        }
        this.sqltConnection.Close();
    }

    public void InsertMovie(long id, string title, Int64 date, string poster)
    {
        if (this.sqltConnection == null)
        {
            Debug.LogError("not init");
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
            Debug.LogError("query: " + "INSERT INTO movies VALUES (" + id + ", \"" + title + "\", " + date + ");" + " - has a problem");
            Debug.LogError(ex.Message);
        }
        this.sqltConnection.Close();
    }

    public void InsertArtist(long id, string firstName, string lastName, string posterPath)
    {
        if (this.sqltConnection == null)
        {
            Debug.LogError("not init");
            return;
        }

        this.sqltConnection.Open();
        SqliteCommand insertSQL = new SqliteCommand("INSERT INTO artists VALUES (" + id + ", \""+ firstName  + "\", \"" + lastName + "\", \"" + posterPath + "\")", this.sqltConnection);
        try
        {
            insertSQL.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        this.sqltConnection.Close();
    }



    internal HashSet<long> getConnectedIds(long id, bool fromArtistToMovie)
    {
        HashSet<long> hSet = new HashSet<long>();
        if (this.sqltConnection == null)
        {
            Logger.Trace("Connection not initialized", LogLevel.Error);
            return hSet;
        }

        string columnFrom = "id_movie";
        string columnTo = "id_artist";        
        if (fromArtistToMovie)
        {            
            columnFrom = "id_artist";
            columnTo = "id_movie";
        }

        this.sqltConnection.Open();

        DataSet dSet = new DataSet();
        SqliteDataAdapter ad = new SqliteDataAdapter("SELECT " + columnTo + " FROM Actors WHERE " + columnFrom + "=" + id + " UNION SELECT " + columnTo + " FROM Directors WHERE " + columnFrom + "=" + id, this.sqltConnection);
        ad.Fill(dSet);

        this.sqltConnection.Close();

        var rows = dSet.Tables[0].Rows;
        foreach (DataRow row in rows)
        {
            hSet.Add((Int64)row[0]);
        }

        return hSet;
    }
}
