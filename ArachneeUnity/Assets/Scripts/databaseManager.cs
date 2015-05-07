using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class databaseManager : MonoBehaviour 
{
    private string databasePath;
    private SqliteConnection connexion;

    // "add new entry" menu
    public GameObject addNewEntryMenu;

    // artist
    public InputField nameIF;
    public InputField surnameIF;
    public InputField yearIF;

    // job
    public InputField jobIF;

    // movie
    public InputField titleIF;
    public InputField dateIF;

    // toggle
    public ToggleGroup artistToggleGrp;
    public ToggleGroup movieToggleGrp;
    public ToggleGroup jobToggleGrp;

    public GameObject toggleBtnPrefab;
    public Transform toogleSpawPoint;

    public int spaceBetweenToggles = 1;
    public int spaceBetweenColumnsOfToggles = 100;

    // to be changed
    private List<Toggle> buttons = new List<Toggle>();


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
    /// takes a query and update reader with data from database
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


    void Start()
    {
        initializeConnexion();
        buildToogles(toogleSpawPoint.position - Vector3.right * spaceBetweenColumnsOfToggles, "SELECT id,name FROM 'artists'", artistToggleGrp);
        buildToogles(toogleSpawPoint.position, "SELECT id,title FROM 'jobs'", jobToggleGrp);
        buildToogles(toogleSpawPoint.position + Vector3.right * spaceBetweenColumnsOfToggles, "SELECT id,title FROM 'movies'", movieToggleGrp);
        setEnabledNewEntryMenu(true);
    }

    
    /// <summary>
    /// build toggle buttons
    /// </summary>
    /// <param name="spawnPoint"></param>
    /// <param name="query"> "SELECT id,title FROM 'jobs'" for example </param>
    void buildToogles(Vector3 spawnPoint, string query, ToggleGroup group)
    {
        this.connexion.Open();
        SqliteDataReader reader;

        try
        {
            reader = getReaderFromQuery(query);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return;
        }
        
        int offset = 0;
        while(reader.Read())
        {
            int id = 0;
            string text = "";

            // get text and id
            try
            {
                id = reader.GetInt32(0);
                text = reader.GetString(1);
            }
            catch (System.Exception e)
            {
                Debug.Log("Failed to build toggle buttons.\n"+e.Message);
                return;
            }
           
            // create toggle button
            GameObject toggleObject = (GameObject)Instantiate(toggleBtnPrefab, spawnPoint + -1 * spaceBetweenToggles * offset * Vector3.up, Quaternion.identity);
            toggleObject.transform.SetParent(group.transform);

            // add identifier to the toggle button
            ButtonDataHolder dataHolder = toggleObject.AddComponent<ButtonDataHolder>();
            dataHolder.Identifier = id;

            // set the toggle group of the toggle button
            Toggle toggleScript = toggleObject.GetComponent<Toggle>();
            toggleScript.group = group;
            toggleScript.isOn = false;

            // set label of the toggle button to title of the job
            Text textInToggle = toggleScript.GetComponentInChildren<Text>();
            textInToggle.text = text;

            // add the created toggle to list (ugly, to be changed)
            this.buttons.Add(toggleScript);

            // increase offset to list the toggle buttons in a vertical arrangement
            offset++;
        }

        this.connexion.Close();  

    }


    // to be refactored

    public void SubmitArtist()
    {
        Debug.Log("Submitting " + nameIF.text + " " + surnameIF.text + " (" + yearIF.text + ")");
        connexion.Open();
        SqliteCommand cmd = new SqliteCommand(this.connexion);
        try
        {
            cmd.CommandText = "INSERT OR ABORT INTO  artists (name,surname,birthdate) VALUES ('" + nameIF.text + "', '" + surnameIF.text + "', " + System.Convert.ToInt32(yearIF.text) + ")";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            Debug.Log("Successfully added to database: " + nameIF.text + " " + surnameIF.text + " (" + yearIF.text + ")");
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed: " + e.Message);
        }
        connexion.Close();
        clearIF();
    }

    public void SubmitMovie()
    {
        Debug.Log("Submitting " + titleIF.text + " (" + dateIF.text + ")");
        connexion.Open();
        SqliteCommand cmd = new SqliteCommand(this.connexion);
        try
        {
            cmd.CommandText = "INSERT OR ABORT INTO  movies (title,date) VALUES ('" + titleIF.text + "', " + System.Convert.ToInt32(dateIF.text) + ")";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            Debug.Log("Successfully added to database: " + titleIF.text + " (" + dateIF.text + ")");
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed: " + e.Message);
        }
        connexion.Close();
        clearIF();
    }

    public void SubmitJob()
    {
        Debug.Log("Submitting " + jobIF.text);
        connexion.Open();
        SqliteCommand cmd = new SqliteCommand(this.connexion);
        try
        {
            cmd.CommandText = "INSERT OR ABORT INTO  jobs (title) VALUES ('" + jobIF.text + "')";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            Debug.Log("Successfully added to database: " + jobIF.text);
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed: " + e.Message);
        }
        connexion.Close();
        clearIF();
    }


    /// <summary>
    /// clear Input fields 
    /// </summary>
    void clearIF()
    {
        nameIF.text = "";
        surnameIF.text = "";
        yearIF.text = "";
        jobIF.text = "";
        titleIF.text = "";
        dateIF.text = "";
    }


    /// <summary>
    /// used to activate/desactivate "new entry" menu
    /// </summary>
    /// <param name="value"></param> 
    public void setEnabledNewEntryMenu(bool value)
    {
        this.addNewEntryMenu.SetActive(value);
        this.toogleSpawPoint.gameObject.SetActive(!value);
    }

    public void SubmitConnexion()
    {
        // soooo ugly, it will cause bugs
        // order : artist, job, movie
        Debug.Log("Note: the ugliness of this code is too damn high");

        int id_movie = 0;
        int id_artist = 0;
        int id_job = 0;

        bool id_artist_notfound = true;
        bool id_job_notfound = true;
        foreach (Toggle button in buttons)
        {
            if (button.isOn)
            {
                if (id_artist_notfound)
                {
                    id_artist = button.GetComponent<ButtonDataHolder>().Identifier;
                    id_artist_notfound = false;
                }
                else if (id_job_notfound)
                {
                    id_job = button.GetComponent<ButtonDataHolder>().Identifier;
                    id_job_notfound = false;
                }
                else
                {
                    id_movie = button.GetComponent<ButtonDataHolder>().Identifier;
                    break;
                }
            }
        }

        Debug.Log("Here is what I found: movie " + id_movie + ", job " + id_job + ", artist "+id_artist);

        Debug.Log("Submitting connection...");
        connexion.Open();
        SqliteCommand cmd = new SqliteCommand(this.connexion);
        try
        {
            cmd.CommandText = "INSERT OR ABORT INTO  castings (id_movie,id_artist,id_job) VALUES (" + id_movie + ", " + id_artist + ", " + id_job + ")";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            Debug.Log("Connection successfully added to database.");
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed: " + e.Message);
        }
        connexion.Close();
    }
}
