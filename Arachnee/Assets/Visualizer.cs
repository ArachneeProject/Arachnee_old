using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Visualizer : MonoBehaviour 
{
    public GameObject[] vertices;
    public FakeEdge[] edges;
    public float repulsion = 1;
    public float attraction = 1;


    private List<GameObject> v;

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    void FixedUpdate()
    {
        foreach (GameObject go in this.vertices)
        {
            foreach (GameObject otherGo in this.vertices)
            {
                if (otherGo.name == go.name)
                {
                    continue;
                }
                Vector3 repulsion = this.repulsion * (go.transform.position - otherGo.transform.position) *   (float)System.Math.Pow((1F / Vector3.Distance(go.transform.position, otherGo.transform.position)), 2);
                go.GetComponent<Rigidbody>().AddForce(repulsion);
            }
        }

        foreach (FakeEdge e in this.edges)
        {
            Vector3 attract = this.attraction * (e.left.transform.position - e.right.transform.position);
            e.left.GetComponent<Rigidbody>().AddForce(-attract);
            e.right.GetComponent<Rigidbody>().AddForce(attract);
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Kill someone"))
        {
            this.vertices[1].SetActive(false);
        }
    }
}
