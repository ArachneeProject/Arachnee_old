using UnityEngine;
using System.Collections;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;


public class Builder : MonoBehaviour
{

    public GameObject moviePrefab;
    public GameObject artistPrefab;
    public GameObject connexionPrefab;

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

       
        // generates artists
        reader = getReaderFromQuery("SELECT * FROM 'artists'");
        int generatedArtists = 0;
        while (reader.Read())
        {
            generatedArtists++;
            GameObject createdEntry = (GameObject)Instantiate(artistPrefab, new Vector3(0, 0, generatedArtists * 1.5F), Quaternion.Euler(Random.value * Vector3.one));
            Artist createdArtist = createdEntry.AddComponent<Artist>();
            createdArtist.Id = (int)reader.GetInt32(0);
            createdArtist.Name = reader.GetString(1);
            createdArtist.Surname = reader.GetString(2);
            createdArtist.Birthdate = reader.GetInt32(3);

            Graph.buildedArtists.Add(createdArtist.Id, createdArtist);
        }

        // generates movies
        reader = getReaderFromQuery("SELECT * FROM 'movies'"); 
        int generatedMovies = 0;
        while (reader.Read())
        {
            generatedMovies++;
            GameObject createdEntry = (GameObject)Instantiate(moviePrefab, new Vector3(generatedMovies * 1.5F, 0, 0), Quaternion.Euler(Random.value * Vector3.one));
            Movie createdMovie = createdEntry.AddComponent<Movie>();
            createdMovie.Id = reader.GetInt32(0);
            createdMovie.Title = reader.GetString(1);
            createdMovie.Date = reader.GetInt32(2);

            Graph.buildedMovies.Add(createdMovie.Id, createdMovie);
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
    }
    


    
}

