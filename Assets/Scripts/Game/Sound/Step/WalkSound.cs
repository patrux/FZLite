using UnityEngine;
using System.Collections;

public class WalkSound : MonoBehaviour 
{
    // Only trigger if isWalking and not KB'd or stunned etc

    [Tooltip("How often (in seconds) to recheck distance travelled.")]
    public float recheckTime;
    float recheckTimer;

    [Tooltip("Distance to travel before a Step sound is triggered.")]
    public float distanceLimit;

    Vector3 lastPosition;

    void Start()
    {
        lastPosition = gameObject.transform.position;
    }

    void FixedUpdate()
    {
        recheckTimer += Time.fixedDeltaTime;

        if (recheckTimer >= recheckTime)
        {
            recheckTimer = 0;

            if (Vector3.Distance(lastPosition, gameObject.transform.position) >= distanceLimit)
            {
                lastPosition = gameObject.transform.position;
                GameObject go = (GameObject)Instantiate(Resources.Load("Sound/Step"), new Vector3(gameObject.transform.position.x, 0f, gameObject.transform.position.z), Quaternion.identity);
            }
        }
    }
}
