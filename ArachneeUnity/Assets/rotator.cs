using UnityEngine;
using System.Collections;

public class rotator : MonoBehaviour 
{
    public float rotationSpeed;
    public float accelCoeff;
	
	// Update is called once per frame
	void Update () 
    {
        this.transform.Rotate(Vector3.one, rotationSpeed);	
	}

    void OnMouseEnter()
    {
        rotationSpeed *= accelCoeff;
    }

    void OnMouseExit()
    {
        rotationSpeed /= accelCoeff;
    }
}
