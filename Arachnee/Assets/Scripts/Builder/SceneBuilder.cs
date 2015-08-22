using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SceneBuilder : MonoBehaviour
{
    public GameObject MoviePrefab;
    public GameObject ArtistPrefab;

    void Start()
    {
        buildEntries();
        buildConnections();
    }

    void buildEntries()
    {
        buildMovies();
        buildArtists();
    }

    void buildMovies()
    {
        MovieObjectBuilder mvBuilder = new MovieObjectBuilder(this.MoviePrefab);

        /*
        foreach (Movie mv in database.Movies)
        {
            mvBuilder.BuildGameObject(mv);
        }
         * */
    }

    void buildArtists()
    {
        ArtistObjectBuilder artBuilder = new ArtistObjectBuilder(this.ArtistPrefab);

        /*
        foreach (Artist at in database.Artists)
        {
            artBuilder.BuildGameObject(at);
        }
         * */
    }

    void buildConnections()
    {

    }
}
