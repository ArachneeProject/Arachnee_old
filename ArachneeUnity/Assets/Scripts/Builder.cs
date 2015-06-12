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

    public GameObject labelPrefab;

    public GameObject Replicatooooor; // for test
    public bool replicatorSpreadAndKillEverything = false;

    //private int numberOfArtists = 0;
    //private int numberOfMovies = 0;

    private int rangeOfBuild = 50;

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

        if (replicatorSpreadAndKillEverything)
        {
            replicatorSpread(); // for test
        }
        
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
                GameObject createdEntry = (GameObject)Instantiate(artistPrefab, new Vector3(Random.Range(0, rangeOfBuild), Random.Range(0, rangeOfBuild), Random.Range(0, rangeOfBuild)), Quaternion.Euler(Random.value * Vector3.one));
                Artist createdArtist = createdEntry.AddComponent<Artist>();
                createdArtist.Id = reader.GetInt32(0);
                createdArtist.Name = reader.GetString(1);
                createdArtist.Surname = reader.GetString(2);
                createdArtist.Birthdate = reader.GetInt32(3);

                
                GameObject labelObject = (GameObject)Instantiate(this.labelPrefab);
                labelObject.GetComponent<GUIText>().text = createdArtist.Surname + " " + createdArtist.Name;
                labelObject.GetComponent<ObjectLabel>().target = createdEntry.transform;
                createdArtist.label = labelObject.GetComponent<ObjectLabel>();
                

                Graph.vertices.Add(createdArtist.Id, createdArtist.transform);
                Graph.edges.Add(createdArtist.Id, new List<Edge>());
            }

            // generates movies
            reader = getReaderFromQuery("SELECT * FROM 'movies'"); 
            while (reader.Read())
            {
                GameObject createdEntry = (GameObject)Instantiate(moviePrefab, new Vector3(Random.Range(0, rangeOfBuild), Random.Range(0, rangeOfBuild), Random.Range(0, rangeOfBuild)), Quaternion.identity);
                Movie createdMovie = createdEntry.AddComponent<Movie>();
                createdMovie.Id = - reader.GetInt32(0);
                createdMovie.Title = reader.GetString(1);
                createdMovie.Date = reader.GetInt32(2);

                
                GameObject labelObject = (GameObject)Instantiate(this.labelPrefab);
                labelObject.GetComponent<GUIText>().text = createdMovie.Title + " (" + createdMovie.Date + ")";
                labelObject.GetComponent<ObjectLabel>().target = createdEntry.transform;
                createdMovie.label = labelObject.GetComponent<ObjectLabel>();
                
                  
                Graph.vertices.Add(createdMovie.Id, createdMovie.transform);
                Graph.edges.Add(createdMovie.Id, new List<Edge>());
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
            edge.setUp(reader.GetInt32(0), - reader.GetInt32(1), lineRend);
            Graph.addEdge(edge);
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


    // for tests
    void replicatorSpread()
    {
        int spread = 1000;
        for (int i = 1; i<spread ; i++)
        {
            GameObject createdEntry = (GameObject)Instantiate(Replicatooooor, new Vector3(Random.Range(0, spread / 3), Random.Range(0, spread / 4), Random.Range(0, spread / 3)), Quaternion.identity);
            Movie createdMovie = createdEntry.AddComponent<Movie>();
            createdMovie.Id = -1000 - i;
            createdMovie.Title = "replicator";
            createdMovie.Date = -1000 - i;

            
            GameObject labelObject = (GameObject)Instantiate(this.labelPrefab);
            labelObject.GetComponent<GUIText>().text = createdMovie.Title + " (" + createdMovie.Date + ")";
            labelObject.GetComponent<ObjectLabel>().target = createdEntry.transform;
            createdMovie.label = labelObject.GetComponent<ObjectLabel>();
             

            Graph.vertices.Add(createdMovie.Id, createdMovie.transform);
            Graph.edges.Add(createdMovie.Id, new List<Edge>());
        }

        
        for (int i = 2; i<spread ; i++)
        {
            GameObject edgeObject = (GameObject)Instantiate(this.edgePrefab, Vector3.zero, Quaternion.identity);
            LineRenderer lineRend = edgeObject.GetComponent<LineRenderer>();
            Edge edge = edgeObject.AddComponent<Edge>();
            edge.setUp(-1000 - i, - 999 - i, lineRend);
            Graph.addEdge(edge);
        }
       

        

    }
    
    

    
}

