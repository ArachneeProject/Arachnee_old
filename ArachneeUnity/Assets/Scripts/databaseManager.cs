using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class databaseManager : MonoBehaviour 
{
    public InputField nameIF;
    public InputField surnameIF;
    public InputField yearIF;

    public ToggleGroup toggleGrp;
    public GameObject toggleBtnPrefab;
    public Transform toogleSpawPoint;
    public int spaceBetweenToggles = 1;

    private List<string> jobsNames = new List<string>(); // just for tests
    
    void Start()
    {
        // just for tests
        this.jobsNames.Add("Director");
        this.jobsNames.Add("Actor");
        this.jobsNames.Add("Toilet guy");
        this.jobsNames.Add("Motherfucker");

        for (int i = 0; i<jobsNames.Count ; i++)
        {
            GameObject toggleObject = (GameObject)Instantiate(toggleBtnPrefab, toogleSpawPoint.position + -1 * spaceBetweenToggles * i * Vector3.up, Quaternion.identity);
            toggleObject.transform.SetParent(toggleGrp.transform);
            Toggle toggleScript = toggleObject.GetComponent<Toggle>();
            toggleScript.group = toggleGrp;
            toggleScript.isOn = false;
            Text textInToggle = toggleScript.GetComponentInChildren<Text>();
            Debug.Log(textInToggle.text);
            textInToggle.text = jobsNames[i];
        }
        
        
    }


    public void SubmitName()
    {
        Debug.Log(nameIF.text + " " + surnameIF.text + " (" + yearIF.text + ")");
    }
}
