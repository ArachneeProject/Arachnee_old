using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour 
{
    public RectTransform title;
    public RectTransform menuItem1;
    public RectTransform menuItem2;
    public Transform mainCamera;

    

    public IEnumerator getOut()
    {
        
        Vector3 position1 = menuItem1.position;

        for (float i = position1.x ; i < 50; i++)
        {
            menuItem1.position += 0.5F * Vector3.right;
            menuItem2.position += 0.5F * Vector3.right;
            yield return null;
        }
    }

    

    public IEnumerator rotateCamera()
    {
        for (float alpha = 0; alpha < 45; alpha++)
        {
            mainCamera.Rotate(Vector3.up,4);
            yield return null;
        }
    }

   public void startAnimation(string name)
    {
        StartCoroutine(name);
    }

    public void disableMainMenu()
   {
       title.gameObject.SetActive(false);
       menuItem1.gameObject.SetActive(false);
       menuItem2.gameObject.SetActive(false);
   }

    
}
