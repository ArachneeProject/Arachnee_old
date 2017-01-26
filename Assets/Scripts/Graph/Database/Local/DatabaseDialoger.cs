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
            correspondingEntries = this.GetEntries<Movie>(new[] {databaseId}).Select(m => (Entry) m).ToList();
        }
        // artist
        else if (identifierTypeStr == typeof (Artist).Name)
        {
            correspondingEntries = this.GetEntries<Artist>(new[] {databaseId}).Select(m => (Entry) m).ToList();
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
    /// Get entries of the given type
    /// </summary>
    public IEnumerable<T> GetEntries<T>(IEnumerable<long> entryIds) where T : Entry, new()
    {
        var idsSet = new HashSet<long>(entryIds);

        var entryAttribute = typeof (T).GetCustomAttributes(false).OfType<DatabaseAttribute>().First();
        if (entryAttribute == null)
        {
            Debug.LogError("Attribute " + typeof(DatabaseAttribute).Name + " not found for " + typeof(T).Name + " type");
            return Enumerable.Empty<T>();
        }
        
        // get number of columns in table
        int columnsCount = 0;
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA table_info (" + entryAttribute.TableName + ")";
            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    columnsCount++;
                }
                reader.Dispose();
            }
            this._sqltConnection.Close();
        }
        
        // get data
        string query = "SELECT * FROM " + entryAttribute.TableName 
                    + " WHERE " + entryAttribute.IdFieldName 
                    + " IN (" + string.Join(",", idsSet.Select(l => l.ToString()).ToArray()) + ")";
        
        var list = new List<T>();
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var databaseValues = new Dictionary<string, object>();
                    for (int i = 0; i < columnsCount; i++)
                    {
                        databaseValues.Add(reader.GetName(i), reader.GetValue(i));
                    }

                    var entry = new T();
                    entry.Deserialize(databaseValues);
                    list.Add(entry);
                }
                reader.Dispose();
            }
            this._sqltConnection.Close();
            cmd.Dispose();
        }

        return list;
    }
    
    /// <summary>
    /// Get entries connected to the given entry
    /// </summary>
    public Dictionary<Entry, IEnumerable<ConnectionType>> GetConnectedEntries(Entry entry)
    {
        var result = new Dictionary<Entry, IEnumerable<ConnectionType>>();

        foreach (ConnectionType connectionType in Enum.GetValues(typeof(ConnectionType)))
        {
            var connectedEntries = GetConnectedEntries(entry, connectionType);
            foreach (var connectedEntry in connectedEntries)
            {
                if (!result.ContainsKey(connectedEntry))
                {
                    result[connectedEntry] = new List<ConnectionType>();
                }
                ((List<ConnectionType>) result[connectedEntry]).Add(connectionType);
            }
        }

        return result;
    }
    
    /// <summary>
    /// Get entries connected to the given entry by the given connection type
    /// </summary>
    public IEnumerable<Entry> GetConnectedEntries(Entry entry, ConnectionType connectionType)
    {
        if (Entry.IsNullOrDefault(entry))
        {
            return Enumerable.Empty<Entry>();
        }

        // get entry attribute
        var entryAttribute = entry.GetType().GetCustomAttributes(false).OfType<DatabaseAttribute>().First();
        if (entryAttribute == null)
        {
            Debug.LogError("Attribute " + typeof(DatabaseAttribute).Name + " not found for " + entry.GetType() + " type");
            return Enumerable.Empty<Entry>();
        }

        // get opposite entry attribute
        Type oppositeType = entry.GetOppositeOf(connectionType);
        var oppositeEntryAttribute = oppositeType.GetCustomAttributes(false).OfType<DatabaseAttribute>().First();
        if (oppositeEntryAttribute == null)
        {
            Debug.LogError("Attribute " + typeof(DatabaseAttribute).Name + " not found for " + oppositeType.Name + " type");
            return Enumerable.Empty<Entry>();
        }

        // get opposite ids
        string query = " SELECT " + oppositeEntryAttribute.IdFieldName +
                       " FROM " +  GetTableName(connectionType) +
                       " WHERE " + entryAttribute.IdFieldName + "=@entryId";
        
        var oppositeIds = new HashSet<long>();
        using (SqliteCommand cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@entryId", entry.DatabaseId);
            
            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    oppositeIds.Add(reader.GetInt64(0));
                }

                this._sqltConnection.Close();
                reader.Dispose();
            }
            cmd.Dispose();
        }

        return entry.GetOppositeEntries(connectionType, oppositeIds, database:this);
    }

    /// <summary>
    /// Get table name from the given connection type
    /// </summary>
    public string GetTableName(ConnectionType connectionType)
    {
        return connectionType + "s";
    }

    #endregion getEntries

    #region insertOrUpdate

    /// <summary>
    /// Insert a new entry or update an existing one
    /// </summary>
    public bool InsertOrUpdate<T>(Entry entry)
    {
        if (Entry.IsNullOrDefault(entry))
        {
            return false;
        }

        var entryAttribute = typeof(T).GetCustomAttributes(false).OfType<DatabaseAttribute>().First();
        if (entryAttribute == null)
        {
            Debug.LogError("Attribute " + typeof(DatabaseAttribute).Name + " not found for " + typeof(T).Name + " type");
            return false;
        }

        int added = 0;

        var serializedEntry = entry.Serialize();

        string query = "INSERT OR REPLACE INTO " + entryAttribute.TableName +
                       " VALUES (" + string.Join(",", serializedEntry.Keys.Select(f => "@" + f).ToArray()) + ")";

        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;

            foreach (var fieldName in serializedEntry.Keys)
            {
                cmd.Parameters.AddWithValue("@" + fieldName, serializedEntry[fieldName]);
            }
            
            this._sqltConnection.Open();
            added = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }

        return added > 0;
    }
    
    /// <summary>
    /// Insert a new actor connection
    /// </summary>
    public bool InsertConnection(Entry fromEntry, Entry toEntry, ConnectionType connectionType)
    {
        if (Entry.IsNullOrDefault(fromEntry) || Entry.IsNullOrDefault(toEntry))
        {
            return false;
        }

        // get From attribute
        var fromAttribute = fromEntry.GetType().GetCustomAttributes(false).OfType<DatabaseAttribute>().First();
        if (fromAttribute == null)
        {
            Debug.LogError("Attribute " + typeof(DatabaseAttribute).Name + " not found for " + fromEntry.GetType().Name + " type");
            return false;
        }

        // get To attribute
        var toAttribute = toEntry.GetType().GetCustomAttributes(false).OfType<DatabaseAttribute>().First();
        if (toAttribute == null)
        {
            Debug.LogError("Attribute " + typeof (DatabaseAttribute).Name + " not found for " + toEntry.GetType().Name + " type");
            return false;
        }

        // execute query
        string query = "INSERT OR IGNORE INTO " + this.GetTableName(connectionType) +
                       "(" + fromAttribute.IdFieldName + "," + toAttribute.IdFieldName + ")" +
                       " VALUES (@" + fromAttribute.IdFieldName + ",@" + toAttribute + ")";

        Debug.Log("Inserting " + connectionType + " from " + fromEntry + " to " + toEntry + " with query " + query);

        int added = 0;
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@" + fromAttribute.IdFieldName, fromEntry.DatabaseId);
            cmd.Parameters.AddWithValue("@" + toAttribute.IdFieldName, toEntry.DatabaseId);

            this._sqltConnection.Open();
            added = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }
        return added > 0;
    }

    #endregion insertOrUpdate

    #region upToDate

    /// <summary>
    /// add the entry identifier as Up To Date
    /// </summary>
    public bool InsertUpToDate(string entryIdentifier)
    {
        int added = 0;
        const string query = "INSERT OR REPLACE INTO EntriesUpToDate VALUES (@entryId)";
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@entryId", entryIdentifier);

            this._sqltConnection.Open();
            added = cmd.ExecuteNonQuery();
            this._sqltConnection.Close();
            cmd.Dispose();
        }
        return added > 0;
    }

    /// <summary>
    /// return wether or not the entry is up to date
    /// </summary>
    public bool EntryIsUpToDate(string entryIdentifier)
    {
        bool upToDate = false;
        const string query = "SELECT entryId FROM EntriesUpToDate WHERE entryId=@entryId LIMIT 1";
        using (var cmd = this._sqltConnection.CreateCommand())
        {
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@entryId", entryIdentifier);

            this._sqltConnection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                upToDate = reader.Read();

                this._sqltConnection.Close();
                reader.Dispose();
            }
            cmd.Dispose();
        }
        Debug.Log(entryIdentifier + " " + upToDate);
        return upToDate;
    }

    #endregion upToDate
}
