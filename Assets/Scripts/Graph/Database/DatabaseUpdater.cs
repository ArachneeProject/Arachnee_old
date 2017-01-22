﻿using System.Collections;
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
        return  _databaseDlg.GetArtists(new HashSet<long>(new long[] { 1100 })).Select(a => (Entry) a).Concat( 
                (_databaseDlg.GetMovies(new HashSet<long>(new long[] { 218 })).Select(m => (Entry) m)));
    }

    private IEnumerator UpdateEntries(IEnumerable<Entry> entriesToUpdate)
    {
        // #switch#
        bigStep.text = "Processing movies";
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(UpdateEntriesFrom<Movie>(entriesToUpdate.OfType<Movie>()));

        bigStep.text = "Processing artists";
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(UpdateEntriesFrom<Artist>(entriesToUpdate.OfType<Artist>()));

        bigStep.text = "Done!";
        Application.LoadLevel(Navigation.Graph);
    }

    private IEnumerator UpdateEntriesFrom<T>(IEnumerable<T> entriesToUpdate) where T : Entry
    {
        // #switch#
        smallStep.text = "Getting connected artists";
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(UpdateEntriesFromTo<T, Artist>(entriesToUpdate));

        smallStep.text = "Getting connected movies";
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(UpdateEntriesFromTo<T, Movie>(entriesToUpdate));

        smallStep.text = "Done!";
    }

    private IEnumerator UpdateEntriesFromTo<TFrom, TTo>(IEnumerable<TFrom> entriesToUpdate) 
        where TFrom : Entry where TTo : Entry
    {
        var retriever = GraphElementRetriever.GetConnectionRetriever<TFrom, TTo>();
        yield return StartCoroutine(retriever.RetrieveData(
            new HashSet<string>(entriesToUpdate.Select(e => e.DatabaseId.ToString()))));

        var data = retriever.RetrievedData;

        foreach (var entryId in data.Keys)
        {
            var connectedEntries = data[entryId] as Dictionary<string, List<ConnectionType>>;
            if (connectedEntries == null)
            {
                Debug.LogWarning("No connection found from " + typeof(TFrom).Name + " " + entryId + " to " + typeof(TTo).Name);
                continue;
            }

            var oppositeRetriever = GraphElementRetriever.GetEntryRetriever<TTo>();

            yield return StartCoroutine(oppositeRetriever.RetrieveData(
                new HashSet<string>(connectedEntries.Keys)));
            var oppositeEntries = oppositeRetriever.RetrievedData.Values.OfType<Entry>().Where(e => !Entry.IsNullOrDefault(e));

            foreach (var oppositeEntry in oppositeEntries)
            {
                this._databaseDlg.InsertOrUpdateEntry<TTo>(oppositeEntry);

                foreach (var connectionType in connectedEntries[oppositeEntry.DatabaseId.ToString()])
                {
                    this._databaseDlg.InsertConnection<TFrom,TTo>(long.Parse(entryId), oppositeEntry.DatabaseId, connectionType);
                }
            }
        }
    }

}
