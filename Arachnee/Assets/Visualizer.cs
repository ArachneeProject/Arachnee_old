using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour 
{
    public Slider slider;
    public FakeVertice[] vert;
    public float repulsion = 1;
    public float attraction = 1;


    private List<FakeVertice> vertices = new List<FakeVertice>();
    private List<FakeEdge> edges = new List<FakeEdge>();

	// Use this for initialization
	void Start () 
    {
        UpdateValue();
	}
	
    // event
	public void UpdateValue() 
    {
        this.vertices.Clear();
        this.edges.Clear();
        float level = this.slider.value;
        foreach (FakeVertice v in this.vert)
        {
            if (v.level <= level)
            {
                v.gameObject.SetActive(true);
                v.SetEdgeActive(true);
                this.vertices.Add(v);
                foreach(FakeEdge e in v.edges)
                {
                    this.edges.Add(e);
                }
            }
            else
            {                
                v.SetEdgeActive(false);
                v.gameObject.SetActive(false);
            }
        }
	}

    void FixedUpdate()
    {
        foreach (FakeVertice v in this.vertices)
        {
            foreach (FakeVertice otherV in this.vertices)
            {
                if (otherV.name == v.name)
                {
                    continue;
                }
                Vector3 repulsion = this.repulsion * (v.transform.position - otherV.transform.position) *   (1F / MiniMath.getSquaredDistance(v.transform.position,otherV.transform.position));
                v.RigidBody.AddForce(repulsion);
            }

            v.RigidBody.AddForce(-100 * v.transform.position);
        }

        foreach (FakeEdge e in this.edges)
        {
            Vector3 attract = this.attraction * (e.left.transform.position - e.right.transform.position);
            e.left.GetComponent<Rigidbody>().AddForce(-attract);
            e.right.GetComponent<Rigidbody>().AddForce(attract);
        }
    }

}
