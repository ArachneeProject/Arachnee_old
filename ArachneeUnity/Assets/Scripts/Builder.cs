using UnityEngine;
using System.Collections;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;


public class Builder : MonoBehaviour
{

    public GameObject moviePrefab;
    public GameObject artistPrefab;
    public GameObject edgePrefab;

    private int numberOfArtists = 0;
    private int numberOfJobs = 0;
    private int numberOfMovies = 0;

    private string databasePath;
    private SqliteConnection connexion;

    
    /// <summary>
    /// build the database
    /// </summary>
    void Start()
    {
        initializeConnexion();
        generateEntries();
        generateConnexions();
    }

    

    /// <summary>
    /// initialize connexion to database
    /// </summary>
    void initializeConnexion()
    {
        // path to database.
        this.databasePath = "URI=file:" + Application.dataPath + "/Database/arachneeDatabase.db";
        // creates connexion
        this.connexion = new SqliteConnection(databasePath);
    }

    
    /// <summary>
    /// takes a query and update this.reader with data from database
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public SqliteDataReader getReaderFromQuery(string query)
    {    
        // create query
        SqliteCommand cmd = new SqliteCommand(this.connexion);
        cmd.CommandText = query;

        // execute query and update reader
        SqliteDataReader reader = cmd.ExecuteReader();

        // clear query
        cmd.Dispose();
        cmd = null;

        return reader;
    }



    /// <summary>
    /// generates GameObjects representing entries
    /// </summary>
    private void generateEntries()
    {
        this.connexion.Open();

        SqliteDataReader reader;

        try
        {
            // generates artists
            reader = getReaderFromQuery("SELECT * FROM 'artists'");
            while (reader.Read())
            {
                GameObject createdEntry = (GameObject)Instantiate(artistPrefab, new Vector3(0, 0, ++this.numberOfArtists * 1.5F), Quaternion.Euler(Random.value * Vector3.one));
                Artist createdArtist = createdEntry.AddComponent<Artist>();
                createdArtist.Id = (int)reader.GetInt32(0);
                createdArtist.Name = reader.GetString(1);
                createdArtist.Surname = reader.GetString(2);
                createdArtist.Birthdate = reader.GetInt32(3);

                Graph.vertices.Add(createdArtist.Id, createdArtist.transform);
            }

            // generates movies
            reader = getReaderFromQuery("SELECT * FROM 'movies'"); 
            while (reader.Read())
            {
                GameObject createdEntry = (GameObject)Instantiate(moviePrefab, new Vector3(++this.numberOfMovies * 1.5F, 0, 0), Quaternion.Euler(Random.value * Vector3.one));
                Movie createdMovie = createdEntry.AddComponent<Movie>();
                createdMovie.Id = reader.GetInt32(0) + this.numberOfArtists;
                createdMovie.Title = reader.GetString(1);
                createdMovie.Date = reader.GetInt32(2);

                Graph.vertices.Add(createdMovie.Id, createdMovie.transform);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
        
        this.connexion.Close();
    }

    /// <summary>
    /// generates connexions between entries (code is not refactored yet)
    /// </summary>
    private void generateConnexions()
    {
        this.connexion.Open();
        SqliteDataReader reader;

        reader = getReaderFromQuery("SELECT id_artist,id_movie FROM 'castings'");
        while(reader.Read())
        {
            GameObject edgeObject = (GameObject) Instantiate(this.edgePrefab, Vector3.zero, Quaternion.identity);
            LineRenderer lineRend = edgeObject.GetComponent<LineRenderer>();
            Edge edge = edgeObject.AddComponent<Edge>();
            edge.setUp(reader.GetInt32(0), reader.GetInt32(1) + this.numberOfArtists, lineRend);
            Graph.edges.Add(edge);
        }

        /*
        // Movie -> Movie
        foreach (KeyValuePair<int,Movie> pair in Graph.buildedMovies)
        {
            reader = getReaderFromQuery("SELECT id_movie2,type FROM 'connexions' WHERE id_movie1="+pair.Key);
            while(reader.Read())
            {
                int connectedMovieId = reader.GetInt32(0);
                int type = reader.GetInt32(1);
                Nature nature = Nature.None;
                switch (type)
                {
                    case 1:
                        nature = Nature.Prequel;
                        break;
                    case 2:
                        nature = Nature.Sequel;
                        break;
                    default:
                        Debug.Log("Unrecognized nature id :"+type);
                        break;
                }

                GameObject createdConnexion = (GameObject)Instantiate(connexionPrefab, Vector3.zero, Quaternion.identity);
                Connexion connexion = createdConnexion.AddComponent<Connexion>();

                connexion.left = pair.Value;
                connexion.right = Graph.buildedMovies[connectedMovieId];
                connexion.nature = nature;
                connexion.initializeLineRenderer(connexion.GetComponent<LineRenderer>());

                Graph.connexions.Add(connexion);
                Debug.Log(pair.Value.Title + " is the " + nature + " of " + Graph.buildedMovies[connectedMovieId].Title);
            }
        }

        // Artist -> Movie
        foreach (KeyValuePair<int, Artist> pair in Graph.buildedArtists)
        {
            reader = getReaderFromQuery("SELECT id_movie,id_job FROM 'castings' WHERE id_artist=" + pair.Key);
            while (reader.Read())
            {
                int connectedMovieId = reader.GetInt32(0);
                int job = reader.GetInt32(1);
                Nature nature = Nature.None;
                switch (job)
                {
                    case 1:
                        nature = Nature.Director;
                        break;
                    case 2:
                        nature = Nature.Actor;
                        break;
                    default:
                        Debug.Log("Unrecognized job id :" + job);
                        break;
                }
                
                GameObject createdConnexion = (GameObject)Instantiate(connexionPrefab, Vector3.zero, Quaternion.identity);
                Connexion connexion = createdConnexion.AddComponent<Connexion>();

                connexion.left = pair.Value;
                connexion.right = Graph.buildedMovies[connectedMovieId];
                connexion.nature = nature;
                connexion.initializeLineRenderer(createdConnexion.GetComponent<LineRenderer>());

                Graph.connexions.Add(connexion);

                Debug.Log(pair.Value.Name + " is the " + nature + " of " + Graph.buildedMovies[connectedMovieId].Title);
            }
        }
         * */


    }
    
    

    
}

