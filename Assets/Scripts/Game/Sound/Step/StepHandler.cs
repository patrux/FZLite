using UnityEngine;
using System.Collections;

public class StepHandler : MonoBehaviour 
{
    public AudioClip[] stepAudioClips;

    void Start()
    {
        int rng = Random.Range(0, stepAudioClips.Length);
        AudioSource source = GetComponent<AudioSource>();
        source.pitch = source.pitch + Random.Range(-0.05f, 0.05f);
        source.volume = source.volume + Random.Range(-0.05f, 0.05f);
        source.clip = stepAudioClips[rng];
    }
}
