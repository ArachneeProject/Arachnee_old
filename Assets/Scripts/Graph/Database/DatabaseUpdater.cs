using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DatabaseUpdater : MonoBehaviour
{
    private DatabaseDialoger _databaseDlg;
	void Start () 
    {
        _databaseDlg = new DatabaseDialoger();
	    var movies = _databaseDlg.GetMovies(new HashSet<long>(new long[] { 218 }));
	    StartCoroutine(UpdateEntry(movies.Select(m => (Entry) m)));
    }
	
    public IEnumerator UpdateEntry(IEnumerable<Entry> entries)
    {
        //  processing movies among the given entries
        //      |
        //      -- artists connected to these movies
        
        var castRetriever = new CastRetriever();
        yield return StartCoroutine(castRetriever.RetrieveData(new HashSet<string>(
            entries.OfType<Movie>().Select(e => e.DatabaseId.ToString()))));
        
        // data is Dictionary with key:movie and value:(Dictionary with key:artist and value:List of connection types)
        var data = castRetriever.RetrievedData; 

        foreach (var movieId in data.Keys)
        {
            var connections = data[movieId] as Dictionary<string, List<ConnectionType>>;

            if (connections == null)
            {
                Debug.LogError("Error trying to get artists connected to movie " + movieId);
                continue;
            }

            // get the Artists connected to the movie
            var artistRetriever = new ArtistRetriever();
            yield return StartCoroutine(artistRetriever.RetrieveData(new HashSet<string>(
                connections.Keys.Where(k => connections[k].Contains(ConnectionType.Actor) 
                                         || connections[k].Contains(ConnectionType.Director)))));
            var artists = artistRetriever.RetrievedData.Values.Where(a => !Entry.IsNullOrDefault((Entry) a));

            Debug.Log("Movie " + movieId + " is connected to " + string.Join(",", artists.Select(a => a.ToString()).ToArray()));
        }
    }
    
}
