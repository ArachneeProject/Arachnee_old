using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;

public class UserFoldersDialoger
{
    private readonly SqliteConnection _sqltConnection = null;

    public UserFoldersDialoger()
    {
        string databasePath = "URI=file:" + Path.Combine(Path.Combine(Application.dataPath, "Database"), "userFolders.db");
        this._sqltConnection = new SqliteConnection(databasePath);
    }

    #region seen

    /// <summary>
    /// Add or delete the video identifier from the Seen table
    /// </summary>
    public bool InsertOrUpdateSeen(string videoIdentifier, bool seen)
    {
        int updated = 0;

        string query = seen ? "INSERT OR REPLACE INTO Seen VALUES (@videoId)"
                            : "DELETE FROM Seen WHERE videoId=@videoId";
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@videoId", videoIdentifier);

            this._sqltConnection.Open();
            updated = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }

        return updated > 0;
    }

    /// <summary>
    /// return true if the video identifier is marked as Seen
    /// </summary>
    public bool VideoWasSeen(string videoIdentifier)
    {
        bool seen = false;
        const string query = "SELECT * FROM Seen WHERE videoId=@videoId";
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@videoId", videoIdentifier);

            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                seen = reader.Read();

                this._sqltConnection.Close();
                reader.Dispose();
            }
            cmd.Dispose();
        }
        return seen;
    }

    #endregion seen

    #region folders

    /// <summary>
    /// return true if folder hash is in database, false otherwise
    /// </summary>
    public bool FolderIsRegistered(string path)
    {
        bool isRegistered = false;
        const string query = "SELECT path FROM Folders WHERE path=@path " +
                             " UNION SELECT path FROM IgnoredFolders WHERE path=@path";
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@path", path);

            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                isRegistered = reader.Read();
                this._sqltConnection.Close();
                reader.Dispose();
            }
            cmd.Dispose();
        }

        return isRegistered;
    }
    
    /// <summary>
    /// insert or update a folder as ignored
    /// </summary>
    public bool InsertOrupdateIgnoredFolder(string path, bool ignored)
    {
        int updated = 0;
        string query = ignored ? "INSERT OR IGNORE INTO Ignored VALUES @path)"
                                : "DELETE FROM IgnoredFolders WHERE path=@path";

        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@path", path);

            this._sqltConnection.Open();
            updated = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }
        return updated > 0;
    }

    /// <summary>
    /// insert a new folder or update an existing one
    /// </summary>
    public bool InsertFolder(string path, string videoIdentifier)
    {
        int added = 0;
        const string query = "INSERT OR REPLACE INTO Folders VALUES (@path,@videoId)";
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@path", path);
            cmd.Parameters.AddWithValue("@videoId", videoIdentifier);

            this._sqltConnection.Open();
            added = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }

        return added > 0;
    }

    /// <summary>
    /// delete a folder
    /// </summary>
    public bool DeleteFolder(string path)
    {
        int deleted = 0;
        const string query = "DELETE FROM Folders WHERE path=@path";
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@path", path);

            this._sqltConnection.Open();
            deleted = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }

        return deleted > 0;
    }

    #endregion folders
    
    #region insertGenres

    // insert genres of movie
    public void InsertGenresOfMovie(long movieId, List<string> genres)
    {
        if (genres.Count == 0)
        {
            return;
        }

        foreach (string g in genres)
        {
            string query = "INSERT OR IGNORE INTO GenresM VALUES (@id,@genre)";
            SqliteCommand cmd = this._sqltConnection.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", movieId);
            cmd.Parameters.AddWithValue("@genre", g);

            this._sqltConnection.Open();
            cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }
    }

    // insert genres of a serie
    public void InsertGenresOfSerie(long serieId, List<string> genres)
    {
        if (genres.Count == 0)
        {
            return;
        }

        foreach (string g in genres)
        {
            string query = "INSERT OR IGNORE INTO GenresS VALUES (@id,@genre)";
            SqliteCommand cmd = this._sqltConnection.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", serieId);
            cmd.Parameters.AddWithValue("@genre", g);

            this._sqltConnection.Open();
            cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }
    }

    #endregion inserGenres
}

