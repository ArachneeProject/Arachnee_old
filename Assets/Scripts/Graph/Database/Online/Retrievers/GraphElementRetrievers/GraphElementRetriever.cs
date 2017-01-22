using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;


public abstract class GraphElementRetriever : OnlineRetriever
{
    protected override IEnumerator RetrieveDataImpl(HashSet<string> identifiers)
    {
        var entryIds = identifiers.Where(id => !string.IsNullOrEmpty(id));

        foreach (var entryId in entryIds)
        {
            var www = new WWW(GetQuery(entryId));
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
                continue;
            }

            var node = JSON.Parse(www.text);
            if (node == null)
            {
                Debug.LogWarning("No result found");
                continue;
            }

            RetrievedData.Add(entryId, BuildResult(node));
        }
    }

    protected abstract string GetQuery(string entryId);
    protected abstract object BuildResult(JSONNode node);

    /// <summary>
    /// Return a new retriever corresponding to the given T
    /// </summary>
    public static OnlineRetriever GetEntryRetriever<T>()
    {
        // #switch#
        if (typeof(T) == typeof(Movie))
        {
            return new MovieRetriever();
        }
        if (typeof(T) == typeof(Artist))
        {
            return new ArtistRetriever();
        }

        Debug.LogError(typeof(T) + " is not handled as an entry type.");
        return new EmptyRetriever();
    }

    /// <summary>
    /// Return a new retriever corresponding to a connection from the given TFrom to the given TTo
    /// </summary>
    public static OnlineRetriever GetConnectionRetriever<TFrom, TTo>()
    {
        // #switch#
        if (typeof (TFrom) == typeof (Movie))
        {
            if (typeof (TTo) == typeof (Artist))
            {
                return new CastRetriever();
            }
        }
        else if (typeof (TFrom) == typeof (Artist))
        {
            if (typeof(TTo) == typeof(Movie))
            {
                return new CreditsRetriever();
            }
        }

        Debug.LogWarning("No retriever found for connection" +
                       " between " + typeof(TFrom) +
                       " and " + typeof(TTo));
        return new EmptyRetriever();
    }

    /// <summary>
    /// Return a new retriever corresponding to the opposite type in a connection including the given type T
    /// </summary>
    public static OnlineRetriever GetEntryRetrieverOppositeOf<T>(ConnectionType connectionType)
    {
        // #switch#
        if (typeof (T) == typeof (Movie))
        {
            switch (connectionType)
            {
                case ConnectionType.Actor:
                case ConnectionType.Director:
                    return new ArtistRetriever();
                default:
                    Debug.LogError(connectionType + " connection applied to Movie is not handled.");
                    return new EmptyRetriever();
            }
        }
        else if (typeof(T) == typeof(Artist))
        {
            switch (connectionType)
            {
                case ConnectionType.Actor:
                case ConnectionType.Director:
                    return new MovieRetriever();
                default:
                    Debug.LogError(connectionType + " connection applied to Artist is not handled.");
                    return new EmptyRetriever();
            }
        }
        Debug.LogError(typeof(T) + " is not handled as an entry type.");
        return new EmptyRetriever();
    }

    /// <summary>
    /// Return a new retriever corresponding to the given T
    /// </summary>
    public static OnlineRetriever GetEntrySearchRetriever<T>()
    {
        // #switch#
        if (typeof(T) == typeof(Movie))
        {
            return new MovieSearchRetriever();
        }
        if (typeof(T) == typeof(Artist))
        {
            return new ArtistSearchRetriever();
        }
        if (typeof(T) == typeof(Serie))
        {
            return new SerieSearchRetriever();
        }

        Debug.LogWarning(typeof(T) + " is not handled as an entry type.");
        return new EmptyRetriever();
    }

}

