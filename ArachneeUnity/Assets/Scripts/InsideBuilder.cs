using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;

public class InsideBuilder : MonoBehaviour 
{
    public static int IdToBuild = -1;
    private string databasePath;
    private SqliteConnection connexion;

    public GameObject quad;

    /// <summary>
    /// start point
    /// </summary>
    void Start()
    {
        initializeConnexion();
        buildInside();
        displayConnections(10);
    }

    /// <summary>
    /// initialize connexion to database
    /// </summary>
    private void initializeConnexion()
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
    private SqliteDataReader getReaderFromQuery(string query)
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

    private void buildInside()
    {
        this.connexion.Open();
        SqliteDataReader reader;
        try
        {
            // code
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        this.connexion.Close();
    }
    
    /// <summary>
    ///  Convert polar to cartesian x,y,z (with y the up-axis)
    /// </summary>
    /// <param name="r"> radius </param>
    /// <param name="theta"> theta longitude in degrees (0 < theta < 360)</param>
    /// <param name="phi"> phi latitude in degrees (-90 < phi < 90) </param>
    /// <returns></returns>
    private Vector3 polarToCart(float r, float theta, float phi)
    {
        theta *= Mathf.Deg2Rad;
        phi *= Mathf.Deg2Rad;
        float x = r * Mathf.Cos(theta) * Mathf.Cos(phi);
        float z = r * Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = r * Mathf.Sin(phi);
        return new Vector3(x, y, z);
    }

    private void displayConnections(int number)
    {
        float squareRoot = Mathf.Sqrt(number);
        int square = (int)squareRoot;

        if(square != squareRoot)
        {
            square++;
        }

        int spacing = 20;
        for (int theta = 0; theta < square * spacing; theta += spacing)
        {
            for (int phi = 0; number > 0 && phi < square * spacing; phi += spacing)
            {
                Instantiate(this.quad, this.polarToCart(5, theta, phi), Quaternion.identity);
                number--;
            }
                
        }
            
    }
}
