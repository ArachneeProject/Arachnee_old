using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entry
{
    public long DatabaseId { get; protected set; }

    public string PosterPath { get; set; }

    protected const long DefaultId = long.MinValue;

    private static Movie _default = null;
    public static Entry DefaultEntry
    {
        get { return _default ?? (_default = new Movie(DefaultId)); }
    }

    public string GetIdentifier()
    {
        return this.GetType().Name + Constants.EntryIdentifierSeparator + this.DatabaseId;
    }

    public abstract Dictionary<string, object> Serialize();
    public abstract void Deserialize(Dictionary<string, object> data);

    public static bool IsNullOrDefault(Entry entry)
    {
        if (entry == null)
        {
            Debug.LogError("Entry was null");
            return true;
        }
        if (entry.DatabaseId == DefaultId)
        {
            Debug.LogError("Entry " + entry + " has the default id");
            return true;
        }
        if (entry.GetIdentifier() == DefaultEntry.GetIdentifier())
        {
            Debug.LogWarning("Entry was the default entry");
            return true;
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (obj is Entry)
        {
            return obj.GetType() == this.GetType() && ((Entry) obj).GetIdentifier() == this.GetIdentifier();
        }
        return false;
    }

    public override int GetHashCode()
    {
        return this.GetIdentifier().GetHashCode();
    }

    public abstract IEnumerable<Entry> GetOppositeEntries(ConnectionType connectionType, HashSet<long> oppositeIds, DatabaseDialoger database);
    public abstract Type GetOppositeOf(ConnectionType connectionType);
    public abstract OnlineRetriever GetConnectionRetriever(ConnectionType connectionType);
    public abstract OnlineRetriever GetOppositeEntryRetriever(ConnectionType connectionType);
}
