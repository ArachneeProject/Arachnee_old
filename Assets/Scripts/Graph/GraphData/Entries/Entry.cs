using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Entry
{
    public long DatabaseId { get; protected set; }

    public string PosterPath { get; set; }

    private static Movie _defaultMovie = null;
    public static Entry DefaultEntry
    {
        get { return _defaultMovie ?? (_defaultMovie = new Movie(long.MinValue)); }
    }

    public string GetIdentifier()
    {
        return this.GetType().Name + Constants.EntryIdentifierSeparator + this.DatabaseId;
    }

    public static bool IsNullOrDefault(Entry entry)
    {
        if (entry == null)
        {
            Debug.LogError("Entry was null");
            return true;
        }
        if (entry.GetIdentifier() == DefaultEntry.GetIdentifier())
        {
            Debug.LogError("Entry was the default entry");
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
}
