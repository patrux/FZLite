using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    // The Target to follow
    public Rigidbody targetRigidBody;

    // Offset between Camera and Target
    public Vector3 cameraOffset = new Vector3(0f, 3f, -4f);

    // Camera smoothing vector
    Vector3 cameraSmoothVelocity = Vector3.zero;

    // Smoothing time
    public float cameraSmoothTime;

    // Velocity Z's impact on the distance
    public float forwardVelocityMod;
    public float forwardVelocityCap;

    void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, GetTargetVector(), ref cameraSmoothVelocity, cameraSmoothTime);
    }

    Vector3 GetTargetVector()
    {
        Vector3 targetVector = new Vector3(
            targetRigidBody.transform.position.x,
            targetRigidBody.transform.position.y,
            targetRigidBody.transform.position.z - (Mathf.Min(targetRigidBody.velocity.z * forwardVelocityMod, forwardVelocityCap)));

        return targetVector + cameraOffset;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("[CameraFollow] cameraOffset[" + cameraOffset + "] forwardVelocity[" + Mathf.Min(targetRigidBody.velocity.z * forwardVelocityMod, forwardVelocityCap) + "] targetRigidBody.velocity.z[" + targetRigidBody.velocity.z + "]");
        }
    }
}
