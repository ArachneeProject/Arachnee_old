using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GraphBuilder
{
    private uint idCounter = 1;

    private Dictionary<uint, uint> movieIds = new Dictionary<uint, uint>();
    private Dictionary<uint, uint> artistIds = new Dictionary<uint, uint>();

    public Graph Graph
    {
        get;
        private set;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="coulombRepulsion"></param>
    /// <param name="hookAttraction"></param>
    public GraphBuilder(float coulombRepulsion, float hookeAttraction)
    {
        this.Graph = new Graph(coulombRepulsion, hookeAttraction);
    }


    /// <summary>
    /// Assign a new unique id to the movie
    /// </summary>
    /// <param name="entry"></param>
    public void AddEntryToGraph(Entry entry)
    {
        //Logger.Trace(entry.ToString() + " - old id: " + entry.Id + " - new id: " + idCounter, LogLevel.Debug);

        if (entry is Movie)
        {
            this.movieIds.Add(entry.Id, idCounter);            
        }
        else if (entry is Artist)
        {
            this.artistIds.Add(entry.Id, idCounter);
        }
        else
        {
            Logger.Trace("Entry is not handled", LogLevel.Error);
            return;
        }
        
        entry.Id = idCounter;
        this.Graph.Vertices.Add(entry.Id, entry);

        this.idCounter++;
    }


    /// <summary>
    /// Update the matrix of the graph
    /// </summary>
    /// <param name="c"></param>
    public void AddConnectionToGraph(Connection c)
    {
        this.Graph.Edges.Add(c);

        // update matrix
    }


    /// <summary>
    /// Return the unique id of the movie
    /// </summary>
    /// <param name="databaseId"></param>
    /// <returns></returns>
    public uint GetNewIdOfMovie(uint databaseId)
    {
        uint newId = 0;
        if (this.movieIds.TryGetValue(databaseId, out newId))
        {
            return newId;
        }
        else
        {
            Logger.Trace("movie with id " + databaseId + " was not found", LogLevel.Error);
            return 0;
        }
    }


    /// <summary>
    /// return the unique id of the artist
    /// </summary>
    /// <param name="databaseId"></param>
    /// <returns></returns>
    public uint GetNewIdOfArtist(uint databaseId)
    {
        uint newId = 0;
        if (this.artistIds.TryGetValue(databaseId, out newId))
        {
            return newId;
        }
        else
        {
            Logger.Trace("artist with id " + databaseId + " was not found", LogLevel.Error);
            return 0;
        }
    }


    /// <summary>
    /// Return the entry with the specified id from the graph
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Entry GetEntryWithId(uint id)
    {
        Entry e;
        if (this.Graph.Vertices.TryGetValue(id, out e))
        {
            return e;
        }
        else
        {
            Logger.Trace("Entry with id " + id + " was not present in the graph", LogLevel.Error);
            return null;
        }
    }


}
