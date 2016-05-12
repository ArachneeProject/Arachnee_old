using UnityEngine;
using System.Collections;

public class lookAtKamera : MonoBehaviour 
{
	void Update () 
    {
        this.transform.LookAt(Camera.main.transform);
	}
}
