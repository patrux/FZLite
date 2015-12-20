using UnityEngine;
using System.Collections;

public class FireballTest : Bolt.EntityEventListener<IObjectBase>
{
    Vector3 sourcePosition;
    Vector3 targetPosition;

    public float speed;

    public float maxDistance;
    float distance;

    bool isInitialized = false;
    bool isMaxDistance = false;

    public void Initialize(Vector3 _origin, float _rotation)
    {
        // Set start position and rotation
        sourcePosition = _origin;
        transform.localPosition = _origin;
        transform.localRotation = Quaternion.Euler(Vector3.up * _rotation);

        isInitialized = true;
    }

    void Start()
    {
        AudioSource audioSource = SoundManager.instance.PlaySoundAttached(sourcePosition, SoundManager.SoundEffect.fireball01, gameObject);
        SoundManager.instance.RandomPitch(audioSource, 0.9f, 1.1f);
        SoundManager.instance.RandomVolume(audioSource, 0.2f, 0.25f);
        SoundManager.instance.RandomVariation(audioSource, "fireball0", 3);
    }

    /// <summary>
    /// Set up BoltEntity.
    /// </summary>
    public override void Attached()
    {
        state.Transform.SetTransforms(transform);
    }

    public override void SimulateOwner()
    {
        if (!isInitialized)
            return;

        CheckDistance();

        if (!isMaxDistance)
        {
            Vector3 frameDistance = (transform.forward * speed);
            transform.localPosition += (frameDistance * Time.deltaTime);
            //transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + frameDistance, Time.deltaTime);
        }
    }

    // Check distance every Nth frame
    int checkDistanceCounter = 0;
    int checkDistanceMaxCount = 5;

    void CheckDistance()
    {
        if (isMaxDistance)
            // Remove
            return;

        checkDistanceCounter++;

        if (checkDistanceCounter >= checkDistanceMaxCount)
        {
            distance = (transform.position - sourcePosition).sqrMagnitude;

            if (distance > maxDistance * maxDistance)
            {
                isMaxDistance = true;
            }

            checkDistanceCounter = 0;
        }
    }
}
