using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

    public float keyboardSensitivity = 1;
    private Vector3 forward = new Vector3(1, 0, 1);
    private Vector3 right = new Vector3(1, 0, -1);
	
	void FixedUpdate () 
    {
        this.transform.position += forward * Input.GetAxis("Vertical") * keyboardSensitivity;
        this.transform.position += right * Input.GetAxis("Horizontal") * keyboardSensitivity;
	}
}
