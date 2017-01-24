using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EntryButtonBuilder : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
        StartCoroutine(BuidButtonsFromQueries<Movie>(new[] { "Planet of the Apes" }));
	}

    /// <summary>
    /// Instantiate some Entry Buttons from the given queries
    /// </summary>
    public IEnumerator BuidButtonsFromQueries<T>(IEnumerable<string> queries)
    {
        var retriever = GraphElementRetriever.GetEntrySearchRetriever<T>();
        yield return StartCoroutine(retriever.RetrieveData(new HashSet<string>(queries)));

        foreach (var query in queries)
        {
            object data;
            if (!retriever.RetrievedData.TryGetValue("Planet of the Apes", out data))
            {
                Debug.LogError("Nothing retrieved");
                continue;
            }
            var entries = data as List<Entry>;
            if (entries == null)
            {
                Debug.LogError("No data retrieved");
                continue;
            }
            entries = entries.Where(e => !Entry.IsNullOrDefault(e)).ToList();
            if (!entries.Any())
            {
                Debug.LogWarning("No " + typeof(T) + " retrieved from query " + query);
                continue;
            }

            foreach (var entry in entries)
            {
                yield return StartCoroutine(BuildEntryButton(entry));
            }
        }
    }

    /// <summary>
    /// Instantiate an Entry Button from the given entry
    /// </summary>
    public IEnumerator BuildEntryButton(Entry entry)
    {
        Debug.Log(entry);
        yield break;
    }
}
