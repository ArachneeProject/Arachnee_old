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

    // artist input fields
    public InputField nameIF;
    public InputField surnameIF;
    public InputField yearIF;

    // job input fields
    public InputField jobIF;

    // movie input fields
    public InputField titleIF;
    public InputField dateIF;



    // toggle button things
    public ToggleGroup artistToggleGrp;
    public ToggleGroup movieToggleGrp;
    public ToggleGroup jobToggleGrp;

    public GameObject toggleBtnPrefab;
    public Transform toogleSpawPoint;

    public int spaceBetweenToggles = 55;
    public int spaceBetweenColumnsOfToggles = 500;

    public float speedOfScrooling = 1000;

    // map
    private Dictionary<ToggleGroup,Dictionary<Toggle,int>> superDico = new Dictionary<ToggleGroup,Dictionary<Toggle,int>>();

    // identifiers of the selected toggles
    private int selectedArtists;
    private int selectedJob;
    private int selectedMovie;

    // true if toggle list needs to be updated, false otherwise
    private bool toggleListNeedsToUpdate = false;



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


    /// <summary>
    /// initialize the world
    /// </summary>
    void Start()
    {
        // set up connection to database
        initializeConnexion();

        // set up superdico
        this.superDico.Add(artistToggleGrp, new Dictionary<Toggle, int>());
        this.superDico.Add(jobToggleGrp, new Dictionary<Toggle, int>());
        this.superDico.Add(movieToggleGrp, new Dictionary<Toggle, int>());

        // build toggles
        buildToggles(toogleSpawPoint.position - Vector3.right * spaceBetweenColumnsOfToggles, "SELECT id,name FROM 'artists'", artistToggleGrp);
        buildToggles(toogleSpawPoint.position, "SELECT id,title FROM 'jobs'", jobToggleGrp);
        buildToggles(toogleSpawPoint.position + Vector3.right * spaceBetweenColumnsOfToggles, "SELECT id,title FROM 'movies'", movieToggleGrp);
        

        // set new entry menu as visible
        setEnabledNewEntryMenu(true);
    }

    
    

    // submit new entry (needs to be refactored...)
    public void SubmitArtist()
    {
        Debug.Log("Submitting " + nameIF.text + " " + surnameIF.text + " (" + yearIF.text + ")");
        connexion.Open();
        SqliteCommand cmd = new SqliteCommand(this.connexion);
        try
        {
            // test if artist is already in database
            SqliteDataReader reader = this.getReaderFromQuery("SELECT id FROM artists WHERE name=\'" + nameIF.text + "\' AND surname=\'" + surnameIF.text + "\' AND birthdate=" + System.Convert.ToInt32(yearIF.text));
            if (reader.Read())
            {
                Debug.Log("This artist is already in the database!");
                connexion.Close();
                return;
            }
            
            // insert the artist
            cmd.CommandText = "INSERT OR ABORT INTO  artists (name,surname,birthdate) VALUES ('" + nameIF.text + "', '" + surnameIF.text + "', " + System.Convert.ToInt32(yearIF.text) + ")";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            Debug.Log("Successfully added to database: " + nameIF.text + " " + surnameIF.text + " (" + yearIF.text + ")");
            this.toggleListNeedsToUpdate = true;
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
            // test if movie is already in database
            SqliteDataReader reader = this.getReaderFromQuery("SELECT id FROM movies WHERE title=\'" + titleIF.text + "\' AND date=" + System.Convert.ToInt32(dateIF.text));
            if (reader.Read())
            {
                Debug.Log("This movie is already in the database!");
                connexion.Close();
                return;
            }

            // insert the movie
            cmd.CommandText = "INSERT OR ABORT INTO  movies (title,date) VALUES ('" + titleIF.text + "', " + System.Convert.ToInt32(dateIF.text) + ")";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            Debug.Log("Successfully added to database: " + titleIF.text + " (" + dateIF.text + ")");
            this.toggleListNeedsToUpdate = true;
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
            // test if job is already in database
            SqliteDataReader reader = this.getReaderFromQuery("SELECT id FROM jobs WHERE title=\'" + jobIF.text + "\'");
            if (reader.Read())
            {
                Debug.Log("This job is already in the database!");
                connexion.Close();
                return;
            }

            // insert the job
            cmd.CommandText = "INSERT OR ABORT INTO  jobs (title) VALUES ('" + jobIF.text + "')";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            Debug.Log("Successfully added to database: " + jobIF.text);
            this.toggleListNeedsToUpdate = true;
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
        if (!value && this.toggleListNeedsToUpdate)
        {
            updateToggles();
        }
    }


    /// <summary>
    /// set up the toggles buttons
    /// </summary>
    void updateToggles()
    {
        foreach (Toggle toggle in superDico[artistToggleGrp].Keys)
        {
            Debug.Log("destroying" + toggle.name);
            Destroy(toggle.gameObject);
        }
        foreach (Toggle toggle in superDico[jobToggleGrp].Keys)
        {
            Destroy(toggle.gameObject);
        }
        foreach (Toggle toggle in superDico[movieToggleGrp].Keys)
        {
            Destroy(toggle.gameObject);
        }
        this.superDico = new Dictionary<ToggleGroup, Dictionary<Toggle, int>>();

        // reset up superdico
        this.superDico.Add(artistToggleGrp, new Dictionary<Toggle, int>());
        this.superDico.Add(jobToggleGrp, new Dictionary<Toggle, int>());
        this.superDico.Add(movieToggleGrp, new Dictionary<Toggle, int>());

        // re-build toggles
        buildToggles(Vector3.up + toogleSpawPoint.position - Vector3.right * spaceBetweenColumnsOfToggles, "SELECT id,name FROM 'artists'", artistToggleGrp);
        buildToggles(Vector3.up + toogleSpawPoint.position, "SELECT id,title FROM 'jobs'", jobToggleGrp);
        buildToggles(Vector3.up + toogleSpawPoint.position + Vector3.right * spaceBetweenColumnsOfToggles, "SELECT id,title FROM 'movies'", movieToggleGrp);

        // toggle list updated
        this.toggleListNeedsToUpdate = false;
    }

    /// <summary>
    /// build toggle buttons
    /// </summary>
    /// <param name="spawnPoint"></param>
    /// <param name="query"> "SELECT id,title FROM 'jobs'" for example </param>
    void buildToggles(Vector3 spawnPoint, string query, ToggleGroup group)
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
        while (reader.Read())
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
                Debug.Log("Failed to build toggle button.\n" + e.Message);
                return;
            }

            // create toggle button
            GameObject toggleObject = (GameObject)Instantiate(toggleBtnPrefab, spawnPoint + -1 * spaceBetweenToggles * offset * Vector3.up, Quaternion.identity);
            toggleObject.transform.SetParent(group.transform);

            // set the toggle group of the toggle button
            Toggle toggleScript = toggleObject.GetComponent<Toggle>();
            toggleScript.group = group;
            toggleScript.isOn = false;

            // identify each toggle
            this.superDico[group].Add(toggleScript, id);

            // set label of the toggle button
            Text textInToggle = toggleScript.GetComponentInChildren<Text>();
            textInToggle.text = text;
            // increase offset to list the toggle buttons in a vertical arrangement
            offset++;
        }

        this.connexion.Close();

    }


    /// <summary>
    /// submit the new connection to the database
    /// </summary>
    public void SubmitConnection()
    {
        int idArtist = 1;
        int idJob = 1;
        int idMovie = 1;
        foreach (Toggle toggle in superDico[artistToggleGrp].Keys)
        {
            if (toggle.isOn)
            {
                idArtist = superDico[artistToggleGrp][toggle];
                break;
            }
        }
        foreach (Toggle toggle in superDico[jobToggleGrp].Keys)
        {
            if (toggle.isOn)
            {
                idJob = superDico[jobToggleGrp][toggle];
                break;
            }
        }
        foreach (Toggle toggle in superDico[movieToggleGrp].Keys)
        {
            if (toggle.isOn)
            {
                idMovie = superDico[movieToggleGrp][toggle];
                break;
            }
        }


        Debug.Log("Submitting artist " + idArtist + " was doing " + idJob + " in movie " + idMovie);

        connexion.Open();
        SqliteCommand cmd = new SqliteCommand(this.connexion);
        try
        {
            cmd.CommandText = "INSERT OR ABORT INTO  castings (id_movie,id_artist,id_job) VALUES (" + idMovie + ", " + idArtist + ", " + idJob + ")";
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


    /// <summary>
    /// scrolling of toggle buttons (ok I just found out Unity has a UI Scroll already set up... ...) 
    /// </summary>
    void Update()
    {
        if (this.addNewEntryMenu.activeInHierarchy)
        {
            return;
        }

        Vector3 scroll = -1F * this.speedOfScrooling * Vector3.up * Input.GetAxis("Mouse ScrollWheel");
        if (Input.mousePosition.x < ((Screen.width / 2) - 300 ))
        {
            this.artistToggleGrp.transform.position += scroll;
            return;
        }
        else if (Input.mousePosition.x > ((Screen.width / 2) + 300))
        {
            this.movieToggleGrp.transform.position += scroll;
            return;
        }
        else
        {
            this.jobToggleGrp.transform.position += scroll;
        }
    }
}