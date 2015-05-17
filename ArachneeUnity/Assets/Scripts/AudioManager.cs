using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour 
{
    public AudioClip[] sounds;
    public static AudioManager manager = null;
    

	// Use this for initialization
	void Start () 
    {
        if (AudioManager.manager == null)
        {
            AudioManager.manager = this;	
        }
        else
        {
            Debug.Log("wtf");
        }        
	}
	
    public void playOnceAt(int track, Vector3 position)
    {
        if (track < sounds.Length)
        {
            AudioSource.PlayClipAtPoint(manager.sounds[track],position,0.5F);
        }
    }

}
