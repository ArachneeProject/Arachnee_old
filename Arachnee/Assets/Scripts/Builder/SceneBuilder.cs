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
        buildEntries();
        buildConnections();

        StartCoroutine(retrievePosters());
    }


    #region Entries
    private void buildEntries()
    {
        buildMovies();
        buildArtists();
    }

    private void buildMovies()
    {
        MovieObjectBuilder mvBuilder = new MovieObjectBuilder(this.MoviePrefab, this.GraphBuilder, this.rangeOfBuilding);
        mvBuilder.BuildGameObject(this.dataDlg.GetDataSet("SELECT * FROM 'movies'"));
    }

    private void buildArtists()
    {
        ArtistObjectBuilder artBuilder = new ArtistObjectBuilder(this.ArtistPrefab, this.GraphBuilder, this.rangeOfBuilding);
        artBuilder.BuildGameObject(this.dataDlg.GetDataSet("SELECT * FROM 'artists'"));
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
            if (e is Artist || e.PosterPath == "null")
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
    private void buildConnections()
    {
        buildArtistToMovieConnections();
    }

    private void buildArtistToMovieConnections()
    {
        buildActorsConnections();
        buildDirectorsConnections();
    }

    private void buildActorsConnections()
    {
        ActorConnectionBuilder acb = new ActorConnectionBuilder(this.ActorConnectionPrefab, this.GraphBuilder);
        acb.BuildGameObject(this.dataDlg.GetDataSet("SELECT * FROM 'Actors'"));
    }

    private void buildDirectorsConnections()
    {
        DirectorConnectionBuilder dcb = new DirectorConnectionBuilder(this.DirectorConnectionPrefab, this.GraphBuilder);
        dcb.BuildGameObject(this.dataDlg.GetDataSet("SELECT * FROM 'Directors'"));
    }
    #endregion Connections

    void FixedUpdate()
    {
        this.GraphBuilder.Graph.UpdateForces();
    }

}
