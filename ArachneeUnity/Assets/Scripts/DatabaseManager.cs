using UnityEngine;
using System.Collections;
using System.Data;
using Mono.Data.Sqlite;



public class DatabaseManager : MonoBehaviour
{
    public GameObject entryPrefab;

    private string databasePath;
    private SqliteConnection connexion;

    /// <summary>
    /// initialisation
    /// </summary>
    void Start()
    {
        initializeConnexion();

        generateMovies();
        generateArtists();

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
    SqliteDataReader getReaderFromQuery(string query)
    {    
        // create query
        SqliteCommand cmd = new SqliteCommand(connexion);
        cmd.CommandText = query;

        // execute query and update reader
        SqliteDataReader reader = cmd.ExecuteReader();

        // clear query
        cmd.Dispose();
        cmd = null;

        return reader;
    }

   
    /// <summary>
    /// generates GameObjects representing movies
    /// </summary>
    private void generateMovies()
    {
        this.connexion.Open();
        SqliteDataReader reader = getReaderFromQuery("SELECT * FROM 'movies'"); 

        // generates movies
        int generatedMovies = 0;
        while (reader.Read())
        {
            generatedMovies++;
            GameObject createdEntry = (GameObject)Instantiate(entryPrefab, new Vector3(generatedMovies * 1.5F, 0, 0), Quaternion.LookRotation(Vector3.down));
            Movie createdMovie = createdEntry.AddComponent<Movie>();
            createdMovie.Id = (int)reader.GetInt32(0);
            createdMovie.Title = reader.GetString(1);
            createdMovie.Date = (int)reader.GetInt32(2);
        }
        Movie.moviesCounter = generatedMovies;

        this.connexion.Close();
        
    }

    /// <summary>
    /// generates GameObjects representing artists
    /// </summary>
    private void generateArtists()
    {
        this.connexion.Open();
        SqliteDataReader reader = getReaderFromQuery("SELECT * FROM 'artists'");

        // generates artists
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
        }

        this.connexion.Close();
        
    }

}

