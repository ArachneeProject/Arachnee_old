using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SUPPPPRRRRRRRRRRRR : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
        StartCoroutine(this.yolo());
	}

    private IEnumerator yolo()
    {
        OnlineRetriever onlret = new OnlineRetriever();
        DatabaseDialoger dlg = new DatabaseDialoger();


        int[] movies = { 1, 2 };
	
        foreach (int i in movies)
        {
            yield return StartCoroutine(onlret.RetrieveMovie(i));

            var res = onlret.NodeRetrieved;


        }

    }
	
}
