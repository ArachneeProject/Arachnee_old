using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnlineRetriever
{
    private readonly object _tinyLock = new object();
    private bool _wasAlreadyUsed = false;

    protected Dictionary<string, object> _retrievedData = new Dictionary<string, object>();

    public Dictionary<string, object> RetrievedData
    {
        get
        {
            lock (_tinyLock)
            {
                return _retrievedData;
            }
        }
    }

    public IEnumerator RetrieveData(HashSet<string> dataIdentifiers)
    {
        if (_wasAlreadyUsed)
        {
            Debug.LogError("Retreiver already used");
            yield break;
        }
        _wasAlreadyUsed = true;

        lock (_tinyLock)
        {
            // hacking hack of hackerz allowing starting a nested coroutine
            var e = RetrieveDataImpl(dataIdentifiers);
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }
    }

    protected abstract IEnumerator RetrieveDataImpl(HashSet<string> identifiers);
}

