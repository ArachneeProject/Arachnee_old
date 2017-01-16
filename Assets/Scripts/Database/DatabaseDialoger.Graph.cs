using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Mono.Data.Sqlite;
using UnityEngine;

public partial class DatabaseDialoger
{
    private readonly SqliteConnection _sqltConnection = null;
    
    public DatabaseDialoger()
    {
        string databasePath = "URI=file:" + Path.Combine(Path.Combine(Application.dataPath, "Database"), "arachnee.db");
        this._sqltConnection = new SqliteConnection(databasePath);
    }
    
    #region getData

    #region getEntries
    /// <summary>
    /// Get the entry corresponding to the vertex identfier (ex: "Movie-218" returns The Terminator)
    /// </summary>
    public Entry GetEntry(string identifier)
    {
        // split identifier into entry type and database id
        var identifierArr = identifier.Split(Constants.EntryIdentifierSeparator);
        if (identifierArr.Length != 2)
        {
            Debug.LogError("\"" + identifier + "\" is an incorrect entry identifier.");
            return Entry.DefaultEntry;
        }

        string identifierTypeStr = identifierArr[0];
        string identifierIdStr = identifierArr[1];

        // get database id of entry
        long databaseId;
        if (!long.TryParse(identifierIdStr, out databaseId))
        {
            Debug.LogError(identifierIdStr + " couldn't be parsed as an Int64, \"" + identifier + "\" is an incorrect entry identifier.");
            return Entry.DefaultEntry;
        }

        // movie
        if (identifierTypeStr == typeof(Movie).Name)
        {
            var correspondingMovies = this.GetMovies(new[] {databaseId}).ToList();
            if (correspondingMovies.Count() != 1)
            {
                Debug.LogError(identifier + " returned " + correspondingMovies.Count() + " movie(s).");
                return Entry.DefaultEntry;
            }
            return correspondingMovies.First();
        }

        // artist
        if (identifierTypeStr == typeof (Artist).Name)
        {
            var correspondingArtists = this.GetArtists(new[] { databaseId }).ToList();
            if (correspondingArtists.Count() != 1)
            {
                Debug.LogError(identifier + " returned " + correspondingArtists.Count() + " artist(s).");
                return Entry.DefaultEntry;
            }
            return correspondingArtists.First();
        }

        // default
        Debug.LogError(identifierTypeStr + " is not handled as an Entry type.");
        return Entry.DefaultEntry;
    }
    
    /// <summary>
    /// return the movies corresponding to the given ids
    /// </summary>
    private IEnumerable<Movie> GetMovies(IEnumerable<long> movieIds)
    {
        var idsSet = new HashSet<long>(movieIds);
        var list = new List<Movie>();

        string query = "SELECT * FROM Movies WHERE id IN (" + string.Join(",", idsSet.Select(l => l.ToString()).ToArray()) + ")";

        using (SqliteCommand cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;

            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Movie movie = new Movie(reader.GetInt64(0));
                    movie.Title = reader.GetString(1);
                    movie.Year = reader.GetInt32(2);
                    movie.PosterPath = reader.GetString(3);

                    list.Add(movie);
                }
                reader.Dispose();
            }
            this._sqltConnection.Close();
            cmd.Dispose();
        }

        return list;
    }

    /// <summary>
    /// return the artists corresponding to the given ids
    /// </summary>
    private IEnumerable<Artist> GetArtists(IEnumerable<long> artistIds)
    {
        var idsSet = new HashSet<long>(artistIds);
        var list = new List<Artist>();

        string query = "SELECT * FROM Artists WHERE id IN (" + string.Join(",", idsSet.Select(l => l.ToString()).ToArray()) + ")";

        using (SqliteCommand cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;

            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Artist artist = new Artist(reader.GetInt64(0));
                    artist.FirstName = reader.GetString(1);
                    artist.LastName = reader.GetString(2);
                    artist.PosterPath = reader.GetString(3);

                    list.Add(artist);
                }

                this._sqltConnection.Close();
                reader.Dispose();
            }
            cmd.Dispose();
        }
        return list;
    }

    #endregion getEntries

    #region getConnections
    public IDictionary<Entry, IEnumerable<ConnectionType>> GetEntriesConnectedTo(Entry entry)
    {
        if (Entry.IsNullOrDefault(entry))
        {
            return new Dictionary<Entry, IEnumerable<ConnectionType>>();
        }
        if (entry.GetType() == typeof(Movie))
        {
            return GetEntriesConnectedTo<Movie>(entry.DatabaseId);
        }
        if (entry.GetType() == typeof(Artist))
        {
            return GetEntriesConnectedTo<Artist>(entry.DatabaseId);
        }

        Debug.LogError(entry.GetType().Name + " is not handled as an Entry type");
        return new Dictionary<Entry, IEnumerable<ConnectionType>>();
    }

    /// <summary>
    /// Return a dictionary of the entries connected to the given entry id
    /// (Keys: connected entries ; Value associated to key: collection of connection types)
    /// </summary>
    private IDictionary<Entry, IEnumerable<ConnectionType>> GetEntriesConnectedTo<T>(long entryId) where T : Entry
    {
        var results = new Dictionary<Entry, IEnumerable<ConnectionType>>();
        foreach (ConnectionType connectionType in Enum.GetValues(typeof(ConnectionType)))
        {
            var connectedEntries = this.GetConnectedEntries<T>(entryId, connectionType);
            foreach (var connectedEntry in connectedEntries)
            {
                if (!results.ContainsKey(connectedEntry))
                {
                    results[connectedEntry] = new HashSet<ConnectionType>();
                }
                ((HashSet<ConnectionType>) results[connectedEntry]).Add(connectionType);
            }
        }

        return results;
    }
    
    /// <summary>
    /// Return a collection of the connected entries of the given connection type
    /// </summary>
    private IEnumerable<Entry> GetConnectedEntries<T>(long entryId, ConnectionType connectionType) where T : Entry
    {
        var ids = this.GetEntryIdsConnectedTo<T>(entryId, connectionType);
        return this.GetEntriesOppositeOf<T>(ids, connectionType);
    }

    private HashSet<long> GetEntryIdsConnectedTo<T>(long entryId, ConnectionType connectionType) where T : Entry
    {
        HashSet<long> hSet = new HashSet<long>();

        var sourceField = GetIdFieldOf<T>();
        var returnField = GetOppositeIdFieldOf<T>(connectionType);
        var table = GetTableNameOf(connectionType);

        if (string.IsNullOrEmpty(sourceField)
            || string.IsNullOrEmpty(returnField)
            || string.IsNullOrEmpty(table))
        {
            Debug.LogError("Getting ids connected to " + typeof(T).Name + Constants.EntryIdentifierSeparator + entryId +
                " by " + connectionType + " connection aborted");
            return hSet;
        }

        string query = " SELECT " + returnField +
                       " FROM " +  table +
                       " WHERE " + sourceField + "=@entryId";
        
        using (SqliteCommand cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@entryId", entryId);
            
            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    hSet.Add(reader.GetInt64(0));
                }

                this._sqltConnection.Close();
                reader.Dispose();
            }
            cmd.Dispose();
        }

        return hSet;
    }

    private IEnumerable<Entry> GetEntriesOppositeOf<T>(IEnumerable<long> entryIds, ConnectionType connectionType)
    {
        if (typeof(T) == typeof(Movie))
        {
            switch (connectionType)
            {
                case ConnectionType.Actor:
                case ConnectionType.Director:
                    return this.GetArtists(entryIds).Select(e => (Entry)e);
            }
        }
        if (typeof(T) == typeof(Artist))
        {
            switch (connectionType)
            {
                case ConnectionType.Actor:
                case ConnectionType.Director:
                    return this.GetMovies(entryIds).Select(e => (Entry)e);
            }
        }

        Debug.LogError(typeof(T).Name + " is not handled as an Entry type");
        return Enumerable.Empty<Entry>();
    }

    private string GetOppositeIdFieldOf<T>(ConnectionType connectionType) where T : Entry
    {
        if (typeof(T) == typeof(Movie))
        {
            switch (connectionType)
            {
                case ConnectionType.Actor:
                case ConnectionType.Director:
                    return this.GetIdFieldOf<Artist>();
                default:
                    Debug.LogError(connectionType + " is not handled as a connection for " + typeof(Movie).Name);
                    return string.Empty;
            }
        }
        if (typeof(T) == typeof(Artist))
        {
            switch (connectionType)
            {
                case ConnectionType.Actor:
                case ConnectionType.Director:
                    return this.GetIdFieldOf<Movie>();
                default:
                    Debug.LogError(connectionType + " is not handled as a connection for " + typeof(Artist).Name);
                    return string.Empty;
            }
        }

        Debug.LogError(typeof(T).Name + " is not handled as an Entry type");
        return string.Empty;
    }
    
    private string GetIdFieldOf<T>()
    {
        return "id_" + typeof (T).Name.ToLower();
    }

    private string GetTableNameOf(ConnectionType connectionType)
    {
        return connectionType + "s";
    }

    private string GetTableNameOf<T>() where T : Entry
    {
        return typeof(T).Name + "s";
    }
    
    #endregion getConnections

    #endregion getData


    #region checks

    /// <summary>
    /// return true if movie id is in database, false otherwise
    /// </summary>
    public bool MovieIsRegistered(Int64 id)
    {
        string query = "SELECT id FROM Movies WHERE id=@id LIMIT 1;";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", id);

        this._sqltConnection.Open();
        var reader = cmd.ExecuteReader();
        var res = reader.Read();
        this._sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }

    /// <summary>
    /// return true if artist id is in database, false otherwise
    /// </summary>
    public bool ArtistIsRegistered(Int64 id)
    {
        string query = "SELECT id FROM Artists WHERE id=@id LIMIT 1;";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", id);

        this._sqltConnection.Open();
        var reader = cmd.ExecuteReader();
        var res = reader.Read();
        this._sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }

    /// <summary>
    /// return true if director connection is in database, false otherwise
    /// </summary>
    public bool ActorIsRegistered(Int64 artistId, Int64 movieId)
    {
        string query = "SELECT id_artist FROM Actors WHERE id_artist=@artistId AND id_movie=@movieId LIMIT 1;";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@artistId", artistId);
        cmd.Parameters.AddWithValue("@movieId", movieId);

        this._sqltConnection.Open();
        var reader = cmd.ExecuteReader();
        var res = reader.Read();
        this._sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }

    /// <summary>
    /// return true if actor connection is in database, false otherwise
    /// </summary>
    public bool DirectorIsRegistered(Int64 artistId, Int64 movieId)
    {
        string query = "SELECT id_artist FROM Directors WHERE id_artist=@artistId AND id_movie=@movieId LIMIT 1;";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@artistId", artistId);
        cmd.Parameters.AddWithValue("@movieId", movieId);

        this._sqltConnection.Open();
        var reader = cmd.ExecuteReader();
        var res = reader.Read();
        this._sqltConnection.Close();
        reader.Dispose();
        cmd.Dispose();

        return res;
    }

    #endregion checks

    #region insertGraphData

    /// <summary>
    /// Insert a new movie to the database (ignored if this movie id is already present)
    /// </summary>
    public bool InsertMovie(long id, string title, string date, string poster, bool seen)
    {
        int seenInt = seen ? 1 : 0;

        string query = "INSERT OR IGNORE INTO Movies VALUES (@id,@title,@date,@poster,@seen)";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@title", title);
        cmd.Parameters.AddWithValue("@date", date);
        cmd.Parameters.AddWithValue("@poster", poster);
        cmd.Parameters.AddWithValue("@seen", seenInt);

        this._sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this._sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    /// <summary>
    /// insert a new serie
    /// </summary>
    public bool InsertSerie(int id, string title, string startYear, string posterPath, bool seen)
    {
        int seenInt = seen ? 1 : 0;

        string query = "INSERT OR IGNORE INTO Series VALUES (@id,@title,@startdate,@poster,@seen)";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@title", title);
        cmd.Parameters.AddWithValue("@startdate", startYear);
        cmd.Parameters.AddWithValue("@poster", posterPath);
        cmd.Parameters.AddWithValue("@seen", seenInt);

        this._sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this._sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    // insert a new artist
    public bool InsertArtist(long id, string firstName, string lastName, string posterPath)
    {
        string query = "INSERT OR IGNORE INTO Artists VALUES (@id,@firstName,@lastName,@poster)";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@firstName", firstName);
        cmd.Parameters.AddWithValue("@lastName", lastName);
        cmd.Parameters.AddWithValue("@poster", posterPath);

        this._sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this._sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    // insert a new actor connection
    public bool InsertActorConnection(long idArtist, long idMovie)
    {
        string query = "INSERT OR IGNORE INTO Actors VALUES (@idArt,@idMv)";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@idArt", idArtist);
        cmd.Parameters.AddWithValue("@idMv", idMovie);

        this._sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this._sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    // insert a new director connection
    public bool InsertDirectorConnection(long idArtist, long idMovie)
    {
        string query = "INSERT OR IGNORE INTO Directors VALUES (@idArt,@idMv)";
        SqliteCommand cmd = this._sqltConnection.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@idArt", idArtist);
        cmd.Parameters.AddWithValue("@idMv", idMovie);

        this._sqltConnection.Open();
        int added = cmd.ExecuteNonQuery();
        this._sqltConnection.Close();
        cmd.Dispose();

        return added > 0;
    }

    #endregion insertGraphData

}
