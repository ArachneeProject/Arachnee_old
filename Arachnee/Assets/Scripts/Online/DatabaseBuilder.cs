using UnityEngine;
using System.Collections;

public class DatabaseBuilder : MonoBehaviour 
{
    private OnlineRetriever onlineRetriever = new OnlineRetriever();
    private DatabaseDialoger dlg = new DatabaseDialoger();

	// Use this for initialization
	void Start () 
    {
        Logger.Trace("ID of movie: " + PlayerPrefs.GetInt("MovieID"), LogLevel.Debug);
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
