using UnityEngine;
using System.Collections;

public class rotator : MonoBehaviour 
{
    public float rotationSpeed = 1;
    public Vector3 axis = Vector3.one;
	
	// Update is called once per frame
	void Update () 
    {
        this.transform.Rotate(axis, rotationSpeed);	
	}

    
}
