using UnityEngine;
using System.Collections;

public class FakeEdge : MonoBehaviour 
{
    public GameObject left;
    public GameObject right;
    private LineRenderer lr;

	// Use this for initialization
	void Start () 
    {
        this.lr = this.gameObject.GetComponent<LineRenderer>();	
	}
	
	// Update is called once per frame
	void Update () 
    {
        this.lr.SetPosition(0, left.transform.position);
        this.lr.SetPosition(1, right.transform.position);	
	}
}
