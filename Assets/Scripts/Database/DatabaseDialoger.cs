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
      
    public DatabaseDialoger()
    {
        string databasePath = "URI=file:" + Application.dataPath + "/Database/arachnee.db";
        this.sqltConnection = new SqliteConnection(databasePath);
    }

    
    public DataSet GetDataSet(string query)
    {
        this.sqltConnection.Open();

        DataSet set = new DataSet();
        SqliteDataAdapter ad = new SqliteDataAdapter(query,this.sqltConnection);
        ad.Fill(set);

        this.sqltConnection.Close();

        return set;
    }
    
    #region checks

    // return true if movie id is in database, false otherwise
    public bool MovieIsRegistered(Int64 id)
    {
        string query = "SELECT id FROM Movies WHERE id=@id LIMIT 1;";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", id);

        this.sqltConnection.Open();
        var reader = cmd.ExecuteReader();
        var res = reader.Read();
        this.sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }

    // return true if artist id is in database, false otherwise
    public bool ArtistIsRegistered(Int64 id)
    {
        string query = "SELECT id FROM Artists WHERE id=@id LIMIT 1;";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", id);

        this.sqltConnection.Open();
        var reader = cmd.ExecuteReader();
        var res = reader.Read();
        this.sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }


    // return true if folder hash is in database, false otherwise
    public bool FolderIsRegistered(string hash)
    {
        string query = "SELECT hash FROM FoldersM WHERE hash=@hash "
            + " UNION SELECT hash FROM FoldersS WHERE hash=@hash"
            + " UNION SELECT hash FROM Ignored WHERE hash=@hash";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@hash", hash);

        this.sqltConnection.Open();
        var reader = cmd.ExecuteReader();
        var res = reader.Read();
        this.sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }

    // return true if actor connection is in database, false otherwise
    public bool ActorIsRegistered(Int64 artistId, Int64 movieId)
    {
        string query = "SELECT id_artist FROM Actors WHERE id_artist=@artistId AND id_movie=@movieId LIMIT 1;";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@artistId", artistId);
        cmd.Parameters.AddWithValue("@movieId", movieId);

        this.sqltConnection.Open();
        var reader = cmd.ExecuteReader();
        var res = reader.Read();
        this.sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }

    // return true if actor connection is in database, false otherwise
    public bool DirectorIsRegistered(Int64 artistId, Int64 movieId)
    {
        string query = "SELECT id_artist FROM Directors WHERE id_artist=@artistId AND id_movie=@movieId LIMIT 1;";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@artistId", artistId);
        cmd.Parameters.AddWithValue("@movieId", movieId);

        this.sqltConnection.Open();
        var reader = cmd.ExecuteReader();
        var res = reader.Read();
        this.sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }

    // return true if movie is marked as Seen
    public bool MovieWasSeen(Int64 movieId)
    {
        string query = "SELECT seen FROM Movies WHERE id=@movieId";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@movieId", movieId);

        this.sqltConnection.Open();
        var reader = cmd.ExecuteReader();

        bool res = false;
        if (reader.Read())
        {
            res = reader.GetInt64(0) != 0;
        }

        this.sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }

    // return true if serie is marked as Seen
    public bool SerieWasSeen(Int64 serieId)
    {
        string query = "SELECT seen FROM Series WHERE id=@serieId";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@serieId", serieId);

        this.sqltConnection.Open();
        var reader = cmd.ExecuteReader();

        bool res = false;
        if (reader.Read())
        {
            res = reader.GetInt64(0) != 0;
        }

        this.sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }

    #endregion checks

    #region insertFolders

    // insert a new folder as ignored
    public bool InsertIgnoredFolder(string hash)
    {
        string query = "INSERT OR IGNORE INTO Ignored VALUES (@hash)";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@hash", hash);

        this.sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this.sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    // insert a new movie folder
    public bool InsertMovieFolder(string hash, long id, string quality)
    {
        string query = "INSERT OR IGNORE INTO FoldersM VALUES (@hash,@id,@quality)";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@hash", hash);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@quality", quality);

        this.sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this.sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    // insert a new serie folder
    public bool InsertSerieFolder(string hash, long id)
    {
        string query = "INSERT OR IGNORE INTO FoldersS VALUES (@hash,@id)";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@hash", hash);
        cmd.Parameters.AddWithValue("@id", id);

        this.sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this.sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    #endregion insertFolders

    #region insertGraphData

    // insert a new movie
    public bool InsertMovie(long id, string title, string date, string poster, bool seen)
    {
        int seenInt = 0;
        if (seen)
        {
            seenInt = 1;
        }

        string query = "INSERT OR IGNORE INTO Movies VALUES (@id,@title,@date,@poster,@seen)";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@title", title);
        cmd.Parameters.AddWithValue("@date", date);
        cmd.Parameters.AddWithValue("@poster", poster);
        cmd.Parameters.AddWithValue("@seen", seenInt);

        this.sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this.sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

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
            SqliteCommand cmd = this.sqltConnection.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", movieId);
            cmd.Parameters.AddWithValue("@genre", g);

            this.sqltConnection.Open();
            cmd.ExecuteNonQuery();
            this.sqltConnection.Close();
            cmd.Dispose();
        }
    }


    // insert a new serie
    public bool InsertSerie(int id, string title, string startYear, string posterPath, bool seen)
    {
        int seenInt = 0;
        if (seen)
        {
            seenInt = 1;
        }

        string query = "INSERT OR IGNORE INTO Series VALUES (@id,@title,@startdate,@poster,@seen)";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@title", title);
        cmd.Parameters.AddWithValue("@startdate", startYear);
        cmd.Parameters.AddWithValue("@poster", posterPath);
        cmd.Parameters.AddWithValue("@seen", seenInt);

        this.sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this.sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
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
            SqliteCommand cmd = this.sqltConnection.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", serieId);
            cmd.Parameters.AddWithValue("@genre", g);

            this.sqltConnection.Open();
            cmd.ExecuteNonQuery();
            this.sqltConnection.Close();
            cmd.Dispose();
        }
    }

    // insert a new artist
    public bool InsertArtist(long id, string firstName, string lastName, string posterPath)
    {
        string query = "INSERT OR IGNORE INTO Artists VALUES (@id,@firstName,@lastName,@poster)";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@firstName", firstName);
        cmd.Parameters.AddWithValue("@lastName", lastName);
        cmd.Parameters.AddWithValue("@poster", posterPath);

        this.sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this.sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    // insert a new actor connection
    public bool InsertActorConnection(long idArtist, long idMovie)
    {
        string query = "INSERT OR IGNORE INTO Actors VALUES (@idArt,@idMv)";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@idArt", idArtist);
        cmd.Parameters.AddWithValue("@idMv", idMovie);

        this.sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this.sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    // insert a new director connection
    public bool InsertDirectorConnection(long idArtist, long idMovie)
    {
        string query = "INSERT OR IGNORE INTO Directors VALUES (@idArt,@idMv)";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@idArt", idArtist);
        cmd.Parameters.AddWithValue("@idMv", idMovie);

        this.sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this.sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }
    
    #endregion insertGraphData

    #region GetData

    // return the set of movie ids connected to the given artist id
    public HashSet<long> getMovies(long artistId)
    {
        HashSet<long> hSet = new HashSet<long>();

        string query = "SELECT id_movie FROM Actors WHERE id_artist=@id UNION SELECT id_movie FROM Directors WHERE id_artist=@id";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", artistId);

        this.sqltConnection.Open();
        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            hSet.Add(reader.GetInt64(0));
        }

        this.sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return hSet;
    }

    // return the set of artist ids connected to the given movie id
    public HashSet<long> getArtists(long movieId)
    {
        HashSet<long> hSet = new HashSet<long>();

        string query = "SELECT id_artist FROM Actors WHERE id_movie=@id UNION SELECT id_artist FROM Directors WHERE id_movie=@id";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", movieId);

        this.sqltConnection.Open();
        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            hSet.Add(reader.GetInt64(0));
        }

        this.sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return hSet;
    }


    public List<object[]> getMovieFoldersData()
    {
        List<object[]> data = new List<object[]>();

        string query = "SELECT id, title, year, poster_path, seen, hash, quality FROM Movies JOIN FoldersM ON Movies.id=FoldersM.idMovie";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;

        this.sqltConnection.Open();
        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            object[] row = new object[reader.FieldCount + 1];
            reader.GetValues(row);
            data.Add(row);
        }

        this.sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        foreach (object[] row in data)
        {
            query = "SELECT genre FROM GenresM WHERE movieId = " + row[0];
            cmd = this.sqltConnection.CreateCommand();
            cmd.CommandText = query;

            List<string> rowGenres = new List<string>();

            this.sqltConnection.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rowGenres.Add(reader.GetString(0));
            }

            this.sqltConnection.Close();
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
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;

        this.sqltConnection.Open();
        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            object[] row = new object[reader.FieldCount + 1];
            reader.GetValues(row);
            data.Add(row);
        }

        this.sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        foreach (object[] row in data)
        {
            query = "SELECT genre FROM GenresS WHERE serieId = " + row[0];
            cmd = this.sqltConnection.CreateCommand();
            cmd.CommandText = query;

            List<string> rowGenres = new List<string>();

            this.sqltConnection.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rowGenres.Add(reader.GetString(0));
            }

            this.sqltConnection.Close();
            reader.Dispose();
            cmd.Dispose();

            row[row.Length - 1] = rowGenres;
        }

        return data;
    }

    #endregion GetData

    #region update
    public void UpdateMovieSeen(long movieId, bool seen)
    {
        int seenInt = 0;
        if (seen)
        {
            seenInt = 1;
        }

        string query = "UPDATE OR IGNORE Movies SET Seen=@seen WHERE id=@movieId;";
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@seen", seenInt);
        cmd.Parameters.AddWithValue("@movieId", movieId);

        this.sqltConnection.Open();
        cmd.ExecuteNonQuery();
        this.sqltConnection.Close();
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
        SqliteCommand cmd = this.sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@seen", seenInt);
        cmd.Parameters.AddWithValue("@serieId", serieId);

        this.sqltConnection.Open();
        cmd.ExecuteNonQuery();
        this.sqltConnection.Close();
        cmd.Dispose();
    }

    #endregion update

}
