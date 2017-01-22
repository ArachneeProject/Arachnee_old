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

    public static OnlineRetriever GetConnectionRetriever<T1, T2>()
    {
        // #switch#
        if (typeof (T1) == typeof (Movie))
        {
            if (typeof (T2) == typeof (Artist))
            {
                return new CastRetriever();
            }
        }
        else if (typeof (T1) == typeof (Artist))
        {
            if (typeof(T2) == typeof(Movie))
            {
                return new CreditsRetriever();
            }
        }

        Debug.LogWarning("No retriever found for connection" +
                       " between " + typeof(T1) +
                       " and " + typeof(T2));
        return new EmptyRetriever();
    }

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
}

