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
    public ToggleGroup toggleGrp;
    public GameObject toggleBtnPrefab;
    public Transform toogleSpawPoint;
    public int spaceBetweenToggles = 1;

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
        /*
        this.connexion.Open();
        SqliteDataReader reader;

        reader = getReaderFromQuery("SELECT id,title FROM 'jobs'");
        int offset = 0;
        while(reader.Read())
        {
            // get string and id
            int id = reader.GetInt32(0);
            string jobTitle = reader.GetString(1);
            
            // create toggle button
            GameObject toggleObject = (GameObject)Instantiate(toggleBtnPrefab, toogleSpawPoint.position + -1 * spaceBetweenToggles * offset * Vector3.up, Quaternion.identity);
            toggleObject.transform.SetParent(toggleGrp.transform);

            // add identifier to the toggle button
            ButtonDataHolder dataHolder = toggleObject.AddComponent<ButtonDataHolder>();
            dataHolder.Identifier = id;

            // set the toggle group of the toggle button
            Toggle toggleScript = toggleObject.GetComponent<Toggle>();
            toggleScript.group = toggleGrp;
            toggleScript.isOn = false;

            // set label of the toggle button to title of the job
            Text textInToggle = toggleScript.GetComponentInChildren<Text>();
            textInToggle.text = jobTitle;
            Debug.Log(textInToggle.text);

            // add the created toggle to list
            this.buttons.Add(toggleScript);

            // increase offset to list the toggle buttons in a vertical arrangement
            offset++;
        }

        this.connexion.Close();  
         * */
    }


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
    }
}
