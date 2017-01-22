using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseDialoger
{
    private readonly SqliteConnection _sqltConnection = null;
    
    public DatabaseDialoger()
    {
        string databasePath = "URI=file:" + Path.Combine(Path.Combine(Application.dataPath, "Database"), "arachneeGraph.db");
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

        IEnumerable<Entry> correspondingEntries = Enumerable.Empty<Entry>();

        // #switch#
        // movie
        if (identifierTypeStr == typeof(Movie).Name)
        {
            correspondingEntries = this.GetMovies(new[] {databaseId}).Select(m => (Entry) m).ToList();
        }
        // artist
        else if (identifierTypeStr == typeof (Artist).Name)
        {
            correspondingEntries = this.GetArtists(new[] { databaseId }).Select(m => (Entry)m).ToList();
        }
        // serie
        else if (identifierTypeStr == typeof (Serie).Name)
        {
            correspondingEntries = this.GetSeries(new[] {databaseId}).Select(m => (Entry) m).ToList();
        }
        // default
        else
        {
            Debug.LogError(identifierTypeStr + " is not handled as an Entry type.");
            return Entry.DefaultEntry;
        }

        if (correspondingEntries.Count() != 1)
        {
            Debug.LogError(identifier + " returned " + correspondingEntries.Count() + " element(s).");
            return Entry.DefaultEntry;
        }
        return correspondingEntries.First();
    }

    /// <summary>
    /// return the movies corresponding to the given ids
    /// </summary>
    public IEnumerable<Movie> GetMovies(IEnumerable<long> movieIds)
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
    public IEnumerable<Artist> GetArtists(IEnumerable<long> artistIds)
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

    /// <summary>
    /// return the series corresponding to the given ids
    /// </summary>
    public IEnumerable<Serie> GetSeries(IEnumerable<long> serieIds)
    {
        var idsSet = new HashSet<long>(serieIds);
        var list = new List<Serie>();

        string query = "SELECT * FROM Series WHERE id IN (" + string.Join(",", idsSet.Select(l => l.ToString()).ToArray()) + ")";

        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;

            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Serie serie = new Serie(reader.GetInt64(0));
                    serie.Title = reader.GetString(1);
                    serie.StartYear = reader.GetInt32(2);
                    serie.PosterPath = reader.GetString(3);

                    list.Add(serie);
                }
                reader.Dispose();
            }
            this._sqltConnection.Close();
            cmd.Dispose();
        }

        return list;
    }

    #endregion getEntries

    #region getConnections
    /// <summary>
    /// Return the Entries connected to the given entry
    /// </summary>
    public IDictionary<Entry, IEnumerable<ConnectionType>> GetEntriesConnectedTo(Entry entry)
    {
        // #switch#
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

    /// <summary>
    /// Return a collection of ids corresponding to entries connected to the given entryId by the given connection type
    /// </summary>
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

    /// <summary>
    /// Return the entries corresponding to the given entryIds, knowing they are connected to the given entry type T by the given connection type
    /// </summary>
    private IEnumerable<Entry> GetEntriesOppositeOf<T>(IEnumerable<long> entryIds, ConnectionType connectionType)
    {
        // #switch#
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

    #region databaseFields
    private string GetOppositeIdFieldOf<T>(ConnectionType connectionType) where T : Entry
    {
        // #switch#
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
    
    private string GetIdFieldOf<T>() where  T : Entry
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

    #endregion databaseFields

    #endregion getConnections

    #endregion getData
    
    #region checks

    /// <summary>
    /// return true if movie id is in database, false otherwise
    /// </summary>
    public bool MovieIsRegistered(long id)
    {
        const string query = "SELECT id FROM Movies WHERE id=@id LIMIT 1;";
        bool isPresent;
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", id);

            this._sqltConnection.Open();

            using (var reader = cmd.ExecuteReader())
            {
                isPresent = reader.Read();
                this._sqltConnection.Close();
                reader.Dispose();
            }
            cmd.Dispose();
        }

        return isPresent;
    }

    /// <summary>
    /// return true if artist id is in database, false otherwise
    /// </summary>
    public bool ArtistIsRegistered(Int64 id)
    {
        const string query = "SELECT id FROM Artists WHERE id=@id LIMIT 1;";
        bool isPresent;
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", id);

            this._sqltConnection.Open();

            using (var reader = cmd.ExecuteReader())
            {
                isPresent = reader.Read();
                this._sqltConnection.Close();
                reader.Dispose();
                cmd.Dispose();
            }
        }
        return isPresent;
    }

    /// <summary>
    /// return true if director connection is in database, false otherwise
    /// </summary>
    public bool ActorIsRegistered(Int64 artistId, Int64 movieId)
    {
        const string query = "SELECT id_artist FROM Actors WHERE id_artist=@artistId AND id_movie=@movieId LIMIT 1;";
        bool isPresent = false;
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@artistId", artistId);
            cmd.Parameters.AddWithValue("@movieId", movieId);

            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                isPresent = reader.Read();
                this._sqltConnection.Close();
                reader.Dispose();
            }
            cmd.Dispose();
        }
        return isPresent;
    }

    /// <summary>
    /// return true if actor connection is in database, false otherwise
    /// </summary>
    public bool DirectorIsRegistered(Int64 artistId, Int64 movieId)
    {
        const string query = "SELECT id_artist FROM Directors WHERE id_artist=@artistId AND id_movie=@movieId LIMIT 1;";
        bool isPresent = false;
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@artistId", artistId);
            cmd.Parameters.AddWithValue("@movieId", movieId);

            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                isPresent = reader.Read();
                this._sqltConnection.Close();
                reader.Dispose();
                cmd.Dispose();
            }
        }
        return isPresent;
    }

    #endregion checks

    #region insertData

    #region entries

    public bool InsertOrUpdateEntry<T>(Entry e)
    {
        // #switch#
        if (typeof (T) == typeof (Movie))
        {
            return InsertOrUpdateMovie((Movie) e);
        }
        if (typeof (T) == typeof (Artist))
        {
            return InsertOrUpdateArtist((Artist) e);
        }

        Debug.LogError(typeof(T).Name + " is not handled as an Entry type");
        return false;
    }

    /// <summary>
    /// Insert a new movie or update an existing one
    /// </summary>
    public bool InsertOrUpdateMovie(Movie movie)
    {
        if (Entry.IsNullOrDefault(movie))
        {
            return false;
        }

        int added = 0;
        const string query = "INSERT OR REPLACE INTO Movies VALUES (@id,@title,@date,@poster)";
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", movie.DatabaseId);
            cmd.Parameters.AddWithValue("@title", movie.Title);
            cmd.Parameters.AddWithValue("@date", movie.Year);
            cmd.Parameters.AddWithValue("@poster", movie.PosterPath);

            this._sqltConnection.Open();
            added = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }

        return added > 0;
    }

    /// <summary>
    /// Insert a new serie or update an existing one
    /// </summary>
    public bool InsertOrUpdateSerie(Serie serie)
    {
        if (Entry.IsNullOrDefault(serie))
        {
            return false;
        }

        const string query = "INSERT OR REPLACE INTO Series VALUES (@id,@title,@startdate,@poster,@seen)";
        int added = 0;

        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", serie.DatabaseId);
            cmd.Parameters.AddWithValue("@title", serie.Title);
            cmd.Parameters.AddWithValue("@startdate", serie.StartYear);
            cmd.Parameters.AddWithValue("@poster", serie.PosterPath);

            this._sqltConnection.Open();
            added = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }

        return added > 0;
    }

    /// <summary>
    /// Insert a new artist or update an existing one
    /// </summary>
    public bool InsertOrUpdateArtist(Artist artist)
    {
        if (Entry.IsNullOrDefault(artist))
        {
            return false;
        }

        int added = 0;
        const string query = "INSERT OR REPLACE INTO Artists VALUES (@id,@firstName,@lastName,@poster)";
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", artist.DatabaseId);
            cmd.Parameters.AddWithValue("@firstName", artist.FirstName);
            cmd.Parameters.AddWithValue("@lastName", artist.LastName);
            cmd.Parameters.AddWithValue("@poster", artist.PosterPath);

            this._sqltConnection.Open();
            added = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }
        return added > 0;
    }

    #endregion entries

    #region connections

    public bool InsertConnection<TFrom, TTo>(long fromEntryId, long toEntryId, ConnectionType connectionType) where TFrom : Entry where TTo : Entry
    {
        // #switch#
        if (typeof (TFrom) == typeof (Movie))
        {
            if (typeof (TTo) == typeof (Artist))
            {
                return InsertArtistToMovieConnection(toEntryId, fromEntryId, connectionType);
            }

            Debug.LogError("Connection from Movie to " + typeof(TTo) + " is not handled.");
            return false;
        }
        if (typeof(TFrom) == typeof(Artist))
        {
            if (typeof(TTo) == typeof(Movie))
            {
                return InsertArtistToMovieConnection(fromEntryId, toEntryId, connectionType);
            }

            Debug.LogError("Connection from Artist to " + typeof(TTo) + " is not handled.");
            return false;
        }

        Debug.LogError(typeof(TFrom) + " is not handled as an entry type.");
        return false;
    }

    /// <summary>
    /// Insert a new connection between an artist and a movie
    /// </summary>
    public bool InsertArtistToMovieConnection(long artistId, long movieId, ConnectionType connectionType)
    {
        // #switch#
        switch (connectionType)
        {
            case ConnectionType.Actor:
                return this.InsertActorConnection(artistId, movieId);
            case ConnectionType.Director:
                return this.InsertDirectorConnection(artistId, movieId);
            default:
                Debug.LogError(connectionType + " is not handled as a connection between an artist and a movie.");
                return false;
        }
    }

    /// <summary>
    /// Insert a new actor connection
    /// </summary>
    public bool InsertActorConnection(long artistId, long movieId)
    {
        int added = 0;
        const string query = "INSERT OR IGNORE INTO Actors VALUES (@idArt,@idMv)";
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@idArt", artistId);
            cmd.Parameters.AddWithValue("@idMv", movieId);

            this._sqltConnection.Open();
            added = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }
        return added > 0;
    }

    /// <summary>
    /// Insert a new director connection
    /// </summary>
    public bool InsertDirectorConnection(long artistId, long movieId)
    {
        int added = 0;
        const string query = "INSERT OR IGNORE INTO Directors VALUES (@idArt,@idMv)";
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@idArt", artistId);
            cmd.Parameters.AddWithValue("@idMv", movieId);

            this._sqltConnection.Open();
            added = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }

        return added > 0;
    }

    #endregion connections

    #endregion insertData

}
