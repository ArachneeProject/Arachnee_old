using UnityEngine;
using System.Collections;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;


public class Builder : MonoBehaviour
{
    public GameObject entryPrefab;

    private string databasePath;
    private SqliteConnection connexion;

    public static Dictionary<int, Movie> buildedMovies = new Dictionary<int, Movie>();
    public static Dictionary<int, Artist> buildedArtists = new Dictionary<int, Artist>();
    public static Dictionary<int, Job> buildedJobs = new Dictionary<int, Job>(); 
    
    /// <summary>
    /// main
    /// </summary>
    void Start()
    {
        initializeConnexion();
        generateEntries();
        generateConnexions();
    }

    

    /// <summary>
    /// initializes connexion
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

        // generates jobs
        SqliteDataReader reader = getReaderFromQuery("SELECT * FROM 'jobs'");
        while (reader.Read())
        {
            GameObject createdEntry = (GameObject)Instantiate(entryPrefab, Vector3.zero, Quaternion.identity);
            Job createdJob = createdEntry.AddComponent<Job>();
            createdJob.Id = reader.GetInt32(0);
            createdJob.Title = reader.GetString(1);
            createdJob.Description = reader.GetString(2);

            buildedJobs.Add(createdJob.Id, createdJob);
        }

        // generates artists
        reader = getReaderFromQuery("SELECT * FROM 'artists'");
        int generatedArtists = 0;
        while (reader.Read())
        {
            generatedArtists++;
            GameObject createdEntry = (GameObject)Instantiate(entryPrefab, new Vector3(0, 0, generatedArtists * 1.5F), Quaternion.LookRotation(Vector3.down));
            Artist createdArtist = createdEntry.AddComponent<Artist>();
            createdArtist.Id = (int)reader.GetInt32(0);
            createdArtist.Name = reader.GetString(1);
            createdArtist.Surname = reader.GetString(2);
            createdArtist.Birthdate = reader.GetInt32(3);
            

            buildedArtists.Add(createdArtist.Id, createdArtist);
        }

        // generates movies
        reader = getReaderFromQuery("SELECT * FROM 'movies'"); 
        int generatedMovies = 0;
        while (reader.Read())
        {
            generatedMovies++;
            GameObject createdEntry = (GameObject)Instantiate(entryPrefab, new Vector3(generatedMovies * 1.5F, 0, 0), Quaternion.LookRotation(Vector3.down));
            Movie createdMovie = createdEntry.AddComponent<Movie>();
            createdMovie.Id = reader.GetInt32(0);
            createdMovie.Title = reader.GetString(1);
            createdMovie.Date = reader.GetInt32(2);

            buildedMovies.Add(createdMovie.Id, createdMovie);
        }

        this.connexion.Close();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="artist"></param>
    private void generateConnexions()
    {
        this.connexion.Open();
        SqliteDataReader reader;

        // Movie -> Movie
        foreach (KeyValuePair<int,Movie> pair in buildedMovies)
        {
            reader = getReaderFromQuery("SELECT id_movie2,type FROM 'connexions' WHERE id_movie1="+pair.Key);
            int connectedMovieId = reader.GetInt32(0);
            int type = reader.GetInt32(1);
            switch (type)
            {
                case 1: // prequel
                    ConnexionBank.connexions.Add(new Connexion(pair.Value,buildedMovies[connectedMovieId],Nature.Prequel));
                    break;
                case 2: // sequel
                    break;
            }
        }

         
    }
    


    
}

