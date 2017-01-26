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
        return  _databaseDlg.GetEntries<Artist>(new HashSet<long>(new long[] { 1100 })).Select(a => (Entry) a).Concat( 
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
            yield return StartCoroutine(GetConnectedEntries(entry, connectionType));
        }
    }

    public IEnumerator GetConnectedEntries(Entry entry, ConnectionType connectionType)
    {
        #region ui
        this.smallStep.text = "Getting " + connectionType + "s";
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

        OnlineRetriever oppositeRetriever = entry.GetOppositeEntryRetriever(connectionType);
        yield return StartCoroutine(oppositeRetriever.RetrieveData(new HashSet<string>(connectedIds.Select(id => id.ToString()))));

        List<Entry> connectedEntries = new List<Entry>();
        foreach (object connectedEntryObj in oppositeRetriever.RetrievedData.Values)
        {
            var connectedEntry = connectedEntryObj as Entry;
            if (connectedEntry == null)
            {
                Debug.LogError("Unable to get entry from object " + connectedEntryObj.GetType());
                continue;
            }
            if (!Entry.IsNullOrDefault(connectedEntry))
            {
                connectedEntries.Add(connectedEntry);
            }
        }
        
        #region ui
        string message = entry + " has " + connectionType + "(s)  " +
                         string.Join(", ", connectedEntries.Select(e => e.ToString()).ToArray());
        Debug.Log(message);
        this.smallStep.text = message;
        yield return new WaitForEndOfFrame();
        #endregion ui
        
        foreach (var oppositeEntry in connectedEntries)
        {
            this._databaseDlg.InsertOrUpdate(new[] {oppositeEntry});
            this._databaseDlg.InsertConnection(entry, oppositeEntry, connectionType);
        }
    }
}
