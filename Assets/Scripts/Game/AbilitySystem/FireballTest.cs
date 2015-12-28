using UnityEngine;
using System.Collections;

public class FireballTest : Bolt.EntityEventListener<IObjectBase>
{
    Vector3 sourcePosition;
    Vector3 targetPosition;

    public float speed;
    public float areaOfEffect = 2f;

    public float maxDistance;
    float distance;

    bool isInitialized = false;
    bool isMaxDistance = false;

    NetPlayer owner = null;

    public void Initialize(Vector3 _origin, float _rotation, NetPlayer _owner)
    {
        // Set start position and rotation
        sourcePosition = _origin;
        transform.localPosition = _origin;
        transform.localRotation = Quaternion.Euler(Vector3.up * _rotation);

        owner = _owner;
        isInitialized = true;
    }

    void Start()
    {
        AudioSource audioSource = SoundManager.instance.PlaySoundAttached(sourcePosition, SoundManager.SoundEffect.fireball01, gameObject);
        SoundManager.instance.RandomPitch(audioSource, 0.9f, 1.1f);
        SoundManager.instance.RandomVolume(audioSource, 0.2f, 0.25f);
        SoundManager.instance.RandomVariation(audioSource, "fireball0", 3);
    }

    void FixedUpdate()
    {
        if (!isMaxDistance)
        {
            if (BoltNetwork.isServer)
            {
                foreach (NetPlayer netPlayer in GameLogic.instance.GetNetPlayerList())
                {
                    //if (netPlayer == null)
                    //    return;

                    ////if (netPlayer.playerController == null)
                    ////    return;

                    Ray ray = FZTools.GetDirectionRay(transform.localPosition, netPlayer.playerController.transform.localPosition);
                    //FZTools.DebugLine(ray, 0.5f, Color.gray, 3f);

                    using (BoltPhysicsHits hits = BoltNetwork.RaycastAll(ray))
                    {
                        for (int i = 0; i < hits.count; ++i)
                        {
                            BoltPhysicsHit hit = hits.GetHit(i);

                            if (hit.distance <= areaOfEffect)
                            {
                                var serializer = hit.body.GetComponent<PlayerController>();
                                print("Hit player: " + serializer.controllingPlayer + " | Owner: " + owner);

                                if (owner != serializer.controllingPlayer)
                                {
                                    FZTools.DebugLine(ray, 0.5f, Color.magenta, 4f);
                                    Vector3 direction = FZTools.GetDirectionNormalized(transform.localPosition, netPlayer.playerController.transform.localPosition);
                                    serializer.motor.Move(direction*50f);
                                    BoltNetwork.Destroy(entity.gameObject);
                                }
                                else
                                {
                                    FZTools.DebugLine(ray, 0.5f, Color.white, 2f);
                                }
                            }
                            else
                            {
                                FZTools.DebugLine(ray, 0.5f, Color.gray, 2f);
                            }

                            //var serializer = hit.body.GetComponent<PlayerController>();

                            //if ((serializer != null) && (serializer.state.team != state.team))
                            //{
                            //    serializer.ApplyDamage(controller.activeWeapon.damagePerBullet);
                            //}
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set up BoltEntity.
    /// </summary>
    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
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
        {
            BoltNetwork.Destroy(entity.gameObject);
            return;
        }

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
