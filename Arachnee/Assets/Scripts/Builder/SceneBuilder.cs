using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SceneBuilder : MonoBehaviour
{
    public bool build = true;
    public int rangeOfBuilding = 20;

    public float coulombRepulsion = 1;
    public float hookeAttraction = 1;

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
        if (!build)
        {
            return;
        }
        this.dataDlg.Initialize("URI=file:" + Application.dataPath + "/Database/arachneeDatabase.db");
        this.GraphBuilder = new GraphBuilder(this.coulombRepulsion,this.hookeAttraction);


        buildScene();

        StartCoroutine(retrievePosters());
    }


    private void buildScene()
    {
        HashSet<uint> moviesDone = new HashSet<uint>();
        HashSet<uint> artistsDone = new HashSet<uint>();
        
        // init
        HashSet<uint> set = new HashSet<uint>();
        string setStr = "";
        set.Add((uint) PlayerPrefs.GetInt("MovieID"));
        setStr = this.formatCollection(set);

        int range = 2;

        while (range-- > 0)
        {
            // build movies
            Debug.Log("list movies " + setStr);
            buildMovies(setStr);

            // build connections movie -> artist
            buildActorsConnections(setStr, true);
            buildDirectorsConnections(setStr, true);

            // save what was done
            foreach (uint id in set)
            {
                moviesDone.Add(id);
            }
            
            // go on to artists
            set.Clear();
            foreach (Connection c in this.GraphBuilder.Graph.Edges)
            {                
                if ((c is ActorConnection || c is DirectorConnection) && !artistsDone.Contains(c.InitLeftId))
                {
                    set.Add(c.InitLeftId);
                }
            }
            setStr = this.formatCollection(set);


            // build artists
            buildArtists(setStr);
            Debug.Log("list artists " + setStr);

            
            // build connections artist->movie
            buildActorsConnections(setStr, false);
            buildDirectorsConnections(setStr, false);

            // save what was done
            foreach (uint id in set)
            {
                artistsDone.Add(id);
            }

            // go on to movies
            set.Clear();
            foreach (Connection c in this.GraphBuilder.Graph.Edges)
            {
                if ((c is ActorConnection || c is DirectorConnection ) && !moviesDone.Contains(c.InitRightId))
                {
                    set.Add(c.InitRightId);
                }                
            }
            setStr = this.formatCollection(set);
                      
        }

        this.GraphBuilder.InitEdges();
        
    }

    #region Entries
    

    private void buildMovies(string list)
    {
        MovieObjectBuilder mvBuilder = new MovieObjectBuilder(this.MoviePrefab, this.GraphBuilder, this.rangeOfBuilding);
        mvBuilder.BuildGameObject(this.dataDlg.GetDataSet("SELECT * FROM movies WHERE id IN " + list));
    }

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
        foreach (Entry e in this.GraphBuilder.Graph.Vertices.Values)
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
        }
    }
    #endregion Entries

    #region Connections
    private void buildActorsConnections(string list, bool isMovieList)
    {
        string query = "SELECT * FROM 'Actors' WHERE id_artist IN " + list;
        if (isMovieList)
        {
            query = "SELECT * FROM 'Actors' WHERE id_movie IN " + list;
        }

        Debug.Log("query : " + query);

        ActorConnectionBuilder acb = new ActorConnectionBuilder(this.ActorConnectionPrefab, this.GraphBuilder);
        acb.BuildGameObject(this.dataDlg.GetDataSet(query));
    }

    private void buildDirectorsConnections(string list, bool isMovieList)
    {
        string query = "SELECT * FROM 'Directors' WHERE id_artist IN " + list;
        if (isMovieList)
        {
            query = "SELECT * FROM 'Directors' WHERE id_movie IN " + list;
        }

        DirectorConnectionBuilder dcb = new DirectorConnectionBuilder(this.DirectorConnectionPrefab, this.GraphBuilder);
        dcb.BuildGameObject(this.dataDlg.GetDataSet(query));
    }
    #endregion Connections

    void FixedUpdate()
    {
        this.GraphBuilder.Graph.UpdateForces();
    }

    /// <summary>
    /// Convert the collection to a string like (1,2,3,4)
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    private string formatCollection(ICollection<uint> collection)
    {
        if (collection.Count == 0)
        {
            return "()";
        }
        string res = "(";
        foreach (uint i in collection)
        {
            res += i + ",";
        }
        return res.Remove(res.Length - 1, 1) + ")";
    }
}
