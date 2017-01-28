using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseUpdater : MonoBehaviour
{
    public Text bigStep;
    public Text smallStep;

    private DatabaseDialoger _databaseDlg;
    
	void Start () 
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            bigStep.text = "Sorry, it looks like your Internet connection is not available.";
            bigStep.color = Color.red;

            smallStep.text = "Update aborted.";
	        return;
	    }

        _databaseDlg = new DatabaseDialoger();
        StartCoroutine(UpdateEntries(GetEntriesToUpdate()));
    }

    private IEnumerable<Entry> GetEntriesToUpdate()
    {
        return  _databaseDlg.GetEntries<Artist>(new HashSet<long>(new long[] { 1100, 2710 })).Select(a => (Entry) a).Concat( 
                (_databaseDlg.GetEntries<Movie>(new HashSet<long>(new long[] { 218 })).Select(m => (Entry) m)));
    }

    /// <summary>
    /// Update the given entries
    /// </summary>
    public IEnumerator UpdateEntries(IEnumerable<Entry> entriesToUpdate, bool forceUpdate = false)
    {
        entriesToUpdate = entriesToUpdate.Where(e => !Entry.IsNullOrDefault(e) 
                            && (forceUpdate || !_databaseDlg.EntryIsUpToDate(e.GetIdentifier()))).ToList();

        foreach (var entry in entriesToUpdate)
        {
            yield return StartCoroutine(UpdateEntry(entry));
        }
        
        // load next scene
        bigStep.text = "Done!";
        smallStep.text = "";
        yield return new WaitForEndOfFrame();
        Application.LoadLevel(Navigation.Graph);
    }
    
    public IEnumerator UpdateEntry(Entry entry)
    {
        #region ui
        this.bigStep.text = "Updating " + entry;
        yield return new WaitForEndOfFrame();
        #endregion ui

        foreach (ConnectionType connectionType in Enum.GetValues(typeof(ConnectionType)))
        {
            yield return StartCoroutine(UpdateConnectedEntries(entry, connectionType));
        }
    }

    public IEnumerator UpdateConnectedEntries(Entry entry, ConnectionType connectionType)
    {
        #region ui
        this.smallStep.text = "Getting connections...";
        yield return new WaitForEndOfFrame();
        #endregion ui

        var retriever = entry.GetConnectionRetriever(connectionType);
        yield return StartCoroutine(retriever.RetrieveData(new HashSet<string>(new[] {entry.DatabaseId.ToString()})));

        object connectionIdsObj;
        if (!retriever.RetrievedData.TryGetValue(entry.DatabaseId.ToString(), out connectionIdsObj))
        {
            Debug.LogError("Nothing found");
            yield break;
        }
        
        var connectedIds = connectionIdsObj as List<long>;
        if (connectedIds == null)
        {
            Debug.LogError("Unable to get list of entry ids from object " + connectionIdsObj.GetType());
            yield break;
        }

        Debug.Log(connectionType + "(s) of " + entry + ":" + string.Join(", ", connectedIds.Select(x => x.ToString()).ToArray()));

        foreach (var connectedId in connectedIds)
        {
            var oppositeRetriever = entry.GetOppositeEntryRetriever(connectionType);
            yield return StartCoroutine(oppositeRetriever.RetrieveData(new HashSet<string>(new[] {connectedId.ToString()})));
            
            object connectedEntryObj;
            if (!oppositeRetriever.RetrievedData.TryGetValue(connectedId.ToString(), out connectedEntryObj))
            {
                Debug.LogError("Unable to find anything for " + connectedId);
                continue;
            }

            var connectedEntry = connectedEntryObj as Entry;
            if (connectedEntry == null)
            {
                Debug.LogError("Unable to get entry from object " + connectedEntryObj.GetType());
                continue;
            }
            if (!Entry.IsNullOrDefault(connectedEntry))
            {
                #region ui
                string message = connectedEntry + " (" + connectionType + ")";
                Debug.Log(message);
                this.smallStep.text = message;
                yield return new WaitForEndOfFrame();
                #endregion ui

                this._databaseDlg.InsertOrUpdate(new[] { connectedEntry });
                this._databaseDlg.InsertConnection(entry, connectedEntry, connectionType);
            }
        }
        
    }
}
