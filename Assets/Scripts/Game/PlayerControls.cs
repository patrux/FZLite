using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour
{
    Rigidbody rigidBody;
    public Animation anim;

    Vector3 velocity = Vector3.zero;

    [Range(0f, 100f)]
    public float velocityCap;

    float speed = 3f;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        float velocityX = 0f;
        float velocityZ = 0f;
        bool isMoving = false;

        if (Input.GetKey(KeyCode.W))
        {
            velocityZ = speed;
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            velocityZ = -speed;
            isMoving = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            velocityX = speed;
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            velocityX = -speed;
            isMoving = true;
        }

        if (isMoving)
        {
            anim["walk"].speed = 1.25f;
            anim.CrossFade("walk");
            transform.localRotation = Quaternion.LookRotation(new Vector3(velocityX, 0f, velocityZ));
        }
        else
        {
            anim["idle"].speed = 0.5f;
            anim.CrossFade("idle");
        }

        if (velocityX < 0f || velocityX > 0f) // if X is not 0
        {
            if (velocityZ < 0f || velocityZ > 0f) // if X is not 0
            {
                velocityX *= 0.7071f;
                velocityZ *= 0.7071f;
            }
        }

        Vector3 v = new Vector3(velocityX, 0f, velocityZ);

        velocity = v;

        //print("velocity[" + v + "] total[" + (float)(Mathf.Abs(velocityX) + Mathf.Abs(velocityZ)) + "]");

        rigidBody.velocity = v;
    }
}
