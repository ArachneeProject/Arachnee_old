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
}

