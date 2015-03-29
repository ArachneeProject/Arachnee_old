using UnityEngine;
using System.Collections;

public class rotator : MonoBehaviour 
{
    public float rotationSpeed = 1;
	
	// Update is called once per frame
	void Update () 
    {
        this.transform.Rotate(Vector3.one, rotationSpeed);	
	}

    
}
