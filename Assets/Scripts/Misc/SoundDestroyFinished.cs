using UnityEngine;
using System.Collections;

public class SoundDestroyFinished : MonoBehaviour 
{
    AudioSource source;

	void Start () 
    {
        source = GetComponent<AudioSource>();
	}
	
	void FixedUpdate () 
    {
        if (!source.isPlaying)
            Destroy(gameObject);
	}
}
