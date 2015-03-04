using UnityEngine;
using System.Collections;
using System.Data;
using Mono.Data.Sqlite;



public class MovieGenerator : MonoBehaviour
{
    public GameObject entryPrefab;

    void Start()
    {
        Debug.Log("Program begins.");

        string databasePath = "URI=file:" + Application.dataPath + "/Database/arachneeDatabase.db"; //Path to database.
        SqliteConnection connexion = new SqliteConnection(databasePath);  //Create connexion.
        connexion.Open(); //Open connection to the database.

        SqliteCommand query = new SqliteCommand(connexion);
        query.CommandText = "SELECT * FROM 'movies'";
        
        SqliteDataReader reader = query.ExecuteReader();

        int i = 0;

        
        
        while (reader.Read())
        {
            i++;
            GameObject createdEntry = (GameObject)Instantiate(entryPrefab, new Vector3(i*1.5F, 0, 0), Quaternion.LookRotation(Vector3.down));
            Movie createdMovie = createdEntry.AddComponent<Movie>();
            createdMovie.Id = (int) reader.GetInt32(0);
            createdMovie.Title = reader.GetString(1);
            createdMovie.Date = (int) reader.GetInt32(2);
        }

        reader.Close();
        reader = null;
        
        query.Dispose();
        query = null;
        connexion.Close();
        connexion = null;

        Debug.Log("Number of movies: "+i);
            
    }


}

