using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public partial class DatabaseDialoger
{
    /// <summary>
    /// return true if folder hash is in database, false otherwise
    /// </summary>
    public bool FolderIsRegistered(string hash)
    {
        string query = "SELECT hash FROM FoldersM WHERE hash=@hash "
            + " UNION SELECT hash FROM FoldersS WHERE hash=@hash"
            + " UNION SELECT hash FROM Ignored WHERE hash=@hash";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@hash", hash);

        this._sqltConnection.Open();
        var reader = cmd.ExecuteReader();
        var res = reader.Read();
        this._sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }
    
    /// <summary>
    /// return true if movie is marked as Seen
    /// </summary>
    public bool MovieWasSeen(Int64 movieId)
    {
        string query = "SELECT seen FROM Movies WHERE id=@movieId";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@movieId", movieId);

        this._sqltConnection.Open();
        var reader = cmd.ExecuteReader();

        bool res = false;
        if (reader.Read())
        {
            res = reader.GetInt64(0) != 0;
        }

        this._sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }

    /// <summary>
    /// return true if serie is marked as Seen
    /// </summary>
    public bool SerieWasSeen(Int64 serieId)
    {
        string query = "SELECT seen FROM Series WHERE id=@serieId";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@serieId", serieId);

        this._sqltConnection.Open();
        var reader = cmd.ExecuteReader();

        bool res = false;
        if (reader.Read())
        {
            res = reader.GetInt64(0) != 0;
        }

        this._sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }
    
    #region insertFolders

    // insert a new folder as ignored
    public bool InsertIgnoredFolder(string hash)
    {
        string query = "INSERT OR IGNORE INTO Ignored VALUES (@hash)";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@hash", hash);

        this._sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this._sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    // insert a new movie folder
    public bool InsertMovieFolder(string hash, long id, string quality)
    {
        string query = "INSERT OR IGNORE INTO FoldersM VALUES (@hash,@id,@quality)";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@hash", hash);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@quality", quality);

        this._sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this._sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    // insert a new serie folder
    public bool InsertSerieFolder(string hash, long id)
    {
        string query = "INSERT OR IGNORE INTO FoldersS VALUES (@hash,@id)";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@hash", hash);
        cmd.Parameters.AddWithValue("@id", id);

        this._sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this._sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    #endregion insertFolders
    
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
    
    public List<object[]> getMovieFoldersData()
    {
        List<object[]> data = new List<object[]>();

        string query = "SELECT id, title, year, poster_path, seen, hash, quality FROM Movies JOIN FoldersM ON Movies.id=FoldersM.idMovie";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;

        this._sqltConnection.Open();
        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            object[] row = new object[reader.FieldCount + 1];
            reader.GetValues(row);
            data.Add(row);
        }

        this._sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        foreach (object[] row in data)
        {
            query = "SELECT genre FROM GenresM WHERE movieId = " + row[0];
            cmd = this._sqltConnection.CreateCommand();
            cmd.CommandText = query;

            List<string> rowGenres = new List<string>();

            this._sqltConnection.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rowGenres.Add(reader.GetString(0));
            }

            this._sqltConnection.Close();
            reader.Dispose();
            cmd.Dispose();

            row[row.Length - 1] = rowGenres;
        }

        return data;
    }

    public List<object[]> getSerieFoldersData()
    {
        List<object[]> data = new List<object[]>();

        string query = "SELECT id, title, startyear, poster_path, seen, hash FROM Series JOIN FoldersS ON Series.id=FoldersS.idSerie";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;

        this._sqltConnection.Open();
        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            object[] row = new object[reader.FieldCount + 1];
            reader.GetValues(row);
            data.Add(row);
        }

        this._sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        foreach (object[] row in data)
        {
            query = "SELECT genre FROM GenresS WHERE serieId = " + row[0];
            cmd = this._sqltConnection.CreateCommand();
            cmd.CommandText = query;

            List<string> rowGenres = new List<string>();

            this._sqltConnection.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rowGenres.Add(reader.GetString(0));
            }

            this._sqltConnection.Close();
            reader.Dispose();
            cmd.Dispose();

            row[row.Length - 1] = rowGenres;
        }

        return data;
    }


    #region update
    public void UpdateMovieSeen(long movieId, bool seen)
    {
        int seenInt = 0;
        if (seen)
        {
            seenInt = 1;
        }

        string query = "UPDATE OR IGNORE Movies SET Seen=@seen WHERE id=@movieId;";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@seen", seenInt);
        cmd.Parameters.AddWithValue("@movieId", movieId);

        this._sqltConnection.Open();
        cmd.ExecuteNonQuery();
        this._sqltConnection.Close();
        cmd.Dispose();
    }

    public void UpdateSerieSeen(long serieId, bool seen)
    {
        int seenInt = 0;
        if (seen)
        {
            seenInt = 1;
        }

        string query = "UPDATE OR IGNORE Series SET Seen=@seen WHERE id=@serieId;";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@seen", seenInt);
        cmd.Parameters.AddWithValue("@serieId", serieId);

        this._sqltConnection.Open();
        cmd.ExecuteNonQuery();
        this._sqltConnection.Close();
        cmd.Dispose();
    }

    #endregion update


}

