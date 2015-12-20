using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public enum SoundEffect
    {
        PLAYERHIT,
        fireball01,
        fireball02,
        fireball03
    }
    
    public AudioSource PlaySoundAttached(Vector3 _position, SoundEffect _soundEffect, GameObject _parent)
    {
        AudioSource audioSource = InstantiateAudioSource(_position, _soundEffect);
        audioSource.gameObject.transform.parent = _parent.transform;
        return audioSource;
    }

    public AudioSource PlaySoundAtPosition(Vector3 _position, SoundEffect _soundEffect)
    {
        AudioSource audioSource = InstantiateAudioSource(_position, _soundEffect);
        return audioSource;
    }

    public void RandomPitch(AudioSource _audioSource, float _min, float _max)
    {
        _audioSource.pitch = Random.Range(_min, _max);
    }

    public void RandomVolume(AudioSource _audioSource, float _min, float _max)
    {
        _audioSource.volume = Random.Range(_min, _max);
    }

    public void RandomVariation(AudioSource _audioSource, string _baseName, int _maxIndex)
    {
        _audioSource.Stop();
        int rng = Random.Range(0, _maxIndex);
        _audioSource.clip = GetAudioClip(_baseName + rng.ToString());
        _audioSource.Play();
    }

    /// <summary>
    /// Instantiates a new AudioSource.
    /// </summary>
    AudioSource InstantiateAudioSource(Vector3 _position, SoundEffect _soundEffect)
    {
        GameObject go = (GameObject)Instantiate(Resources.Load("Sound/SoundFx"), _position, Quaternion.identity);
        AudioSource audioSource = go.GetComponent<AudioSource>();
        audioSource.clip = GetAudioClip(_soundEffect.ToString());
        audioSource.Play();
        go.GetComponent<SoundDestroyFinished>().enabled = true;
        return audioSource;
    }

    /// <summary>
    /// Gets an AudioClip.
    /// </summary>
    AudioClip GetAudioClip(string _soundEffect)
    {
        return (Resources.Load("Sound/Sounds/" + _soundEffect) as AudioClip);
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
