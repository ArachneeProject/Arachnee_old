using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SceneBuilder : MonoBehaviour
{
    public int rangeOfBuilding = 30;

    public Graph graph;

    public int maxInactiveEntries = 1000;

    public GameObject MoviePrefab;
    public GameObject ArtistPrefab;
    public LineRenderer ActorConnectionPrefab;
    public LineRenderer DirectorConnectionPrefab;


    private DatabaseDialoger dataDlg = new DatabaseDialoger();

    private GraphBuilder GraphBuilder
    {
        get;
        set;
    }

    /// <summary>
    /// Init scene
    /// </summary>
    void Start()
    {
        this.dataDlg.Initialize();
        this.GraphBuilder = new GraphBuilder(this.graph);

        buildScene();        
    }

    // main
    private void buildScene()
    {
        int seed = PlayerPrefs.GetInt(Constants.PP_MOVIE_ID);
        Debug.Log("seed: " + seed);
        

        HashSet<long> moviesToBuild = new HashSet<long>();
        HashSet<long> artistsToBuild = new HashSet<long>();
        moviesToBuild.Add(seed);
        this.maxInactiveEntries--;

        HashSet<long> movieStep = new HashSet<long>();
        HashSet<long> artistStep = new HashSet<long>();
        movieStep.Add(seed);
        
        while (this.maxInactiveEntries > 0 && movieStep.Count > 0)
        {
            // movie -> artists
            foreach (long mvId in movieStep)
            {
                HashSet<long> connectedArtists = this.dataDlg.getConnectedIds(mvId,false);
                foreach(long art in connectedArtists)
                {
                    if (artistsToBuild.Add(art))
                    {
                        if (this.maxInactiveEntries-- > 0)
                        {
                            artistStep.Add(art);
                        }
                        else
                        {
                            break;
                        }                        
                    }
                }
                if (this.maxInactiveEntries <= 0)
                {
                    break;
                }
            }
            movieStep.Clear();

            if (this.maxInactiveEntries<= 0 || artistStep.Count==0)
            {
                break;
            }

            // artist -> movies
            foreach(long artId in artistStep)
            {
                var connectedMovies = this.dataDlg.getConnectedIds(artId, true);                
                foreach (long mvId in connectedMovies)
                {
                    if (moviesToBuild.Add(mvId))
                    {
                        if (this.maxInactiveEntries-- > 0)
                        {
                            movieStep.Add(mvId);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (this.maxInactiveEntries <= 0)
                {
                    break;
                }
            }
            artistStep.Clear();
        }

        // build entries
        string movies = this.formatCollection(moviesToBuild);
        string artists = this.formatCollection(artistsToBuild);
        this.buildMovies(movies);
        this.buildArtists(artists);
        
        // build connections
        this.buildActorsConnections(artists, movies);
        this.buildDirectorsConnections(artists, movies);        

        // unfold the graph
        this.GraphBuilder.UnfoldStart();

        Logger.Trace("Vertices " + this.GraphBuilder.Graph.Vertices.Count, LogLevel.Info);
        Logger.Trace("Edges " + this.GraphBuilder.Graph.Edges.Count, LogLevel.Info);

        StartCoroutine(retrievePosters());
    }

    /// <summary>
    /// Convert the collection to a string like (1,2,3,4)
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    private string formatCollection(ICollection<long> collection)
    {
        if (collection.Count == 0)
        {
            return "()";
        }
        string res = "(";
        foreach (long i in collection)
        {
            res += i + ",";
        }
        return res.Remove(res.Length - 1, 1) + ")";
    }

    #region Entries
    
    /// <summary>
    /// Build the gameobjects of movies
    /// </summary>
    /// <param name="list"></param>
    private void buildMovies(string list)
    {
        MovieObjectBuilder mvBuilder = new MovieObjectBuilder(this.MoviePrefab, this.GraphBuilder, this.rangeOfBuilding);
        mvBuilder.BuildGameObject(this.dataDlg.GetDataSet("SELECT * FROM movies WHERE id IN " + list));
    }

    /// <summary>
    /// Build the gameobjects of artists
    /// </summary>
    /// <param name="list"></param>
    private void buildArtists(string list)
    {
        ArtistObjectBuilder artBuilder = new ArtistObjectBuilder(this.ArtistPrefab, this.GraphBuilder, this.rangeOfBuilding);
        artBuilder.BuildGameObject(this.dataDlg.GetDataSet("SELECT * FROM artists WHERE id IN " + list));
    }

    /// <summary>
    /// Retrive poster for each entry
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator retrievePosters()
    {
        OnlineRetriever onlret = new OnlineRetriever();
        foreach (Entry e in this.GraphBuilder.Graph.Vertices)
        {
            if (e.PosterPath == "null")
            {
                continue;
            }
            yield return StartCoroutine(onlret.RetrievePoster(e.PosterPath));
            if (onlret.Texture != null)
            {
                e.gameObject.GetComponent<Renderer>().material.mainTexture = onlret.Texture;
            }
            else
            {
                e.gameObject.GetComponent<Renderer>().material.mainTexture = Resources.Load(Constants.RES_DEFAULT_IMAGE) as Texture;
            }
        }
    }
    #endregion Entries

    #region Connections
    /// <summary>
    /// Build the gameobjects for 'actor' connections
    /// </summary>
    /// <param name="list"></param>
    /// <param name="isMovieList"></param>
    private void buildActorsConnections(string artistlist, string movielist)
    {
        string query = "SELECT * FROM Actors WHERE id_artist IN " + artistlist + " AND id_movie IN " + movielist;

        ActorConnectionBuilder acb = new ActorConnectionBuilder(this.ActorConnectionPrefab, this.GraphBuilder);
        acb.BuildGameObject(this.dataDlg.GetDataSet(query));
    }

    /// <summary>
    /// Build the gameobject for 'director' connections
    /// </summary>
    /// <param name="list"></param>
    /// <param name="isMovieList"></param>
    private void buildDirectorsConnections(string artistlist, string movielist)
    {
        string query = "SELECT * FROM Directors WHERE id_artist IN " + artistlist + " AND id_movie IN " + movielist;

        DirectorConnectionBuilder dcb = new DirectorConnectionBuilder(this.DirectorConnectionPrefab, this.GraphBuilder);
        dcb.BuildGameObject(this.dataDlg.GetDataSet(query));
    }
    #endregion Connections

    
    // for debug
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 30), "Back to menu"))
        {
            Application.LoadLevel(Constants.SC_INTERNET_MOVIE_SEARCH);
        }
    }


    
}
