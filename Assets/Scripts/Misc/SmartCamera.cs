using UnityEngine;
using System.Collections;

// Could very easily make the "area" circle shaped instead of rectangle (based on screen)
// Maybe make two modes
[RequireComponent(typeof(Camera))]
public class SmartCamera : MonoBehaviour
{
    // Name of the target object to look for
    public string targetName = "Player";

    // The root/origin gameObject
    private GameObject targetObject;

    // Offset between the target and camera
    public Vector3 cameraOffset = new Vector3(0f, -10f, 0f);

    // Camera smoothing vector
    Vector3 cameraSmoothVelocity = Vector3.zero;

    // Smoothing time
    public float cameraSmoothTime;

    // The max distance the camera will offset based on mouse position
    public Vector2 targetMaxDistance;

    // The amount of screen space needed to cover by the mouse
    // To reach maxTargetDistance, 0.8 would translate to 80% of screenspace
    public Vector2 targetDistanceScale;

    // The scale of the screen in which the mouse will reach distance cap
    Vector3 screenScale;

    void Start()
    {
        Initialize();
    }

    // Initialize before calling update
    bool isInitialized = false;

    void Initialize()
    {
        // Get player
        targetObject = GameObject.Find(targetName);

        // Initialize by instantly moving to the target position so the camera won't pan due to smoothdamp
        gameObject.transform.position = targetObject.transform.position;
        //gameObject.transform.parent = targetObject.transform;
        //gameObject.transform.localScale = Vector3.zero;

        UpdateSettings();
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized)
            return;

        // Move to player position
        //gameObject.transform.position = targetObject.transform.position;

        // Update logic
        UpdateCamera();
    }

    /// <summary>
    /// This has to be called after changing screen resolution/size,
    /// or after changing the distanceScale.
    /// </summary>
    public void UpdateSettings()
    {
        // Store screen scale
        screenScale = new Vector3(
            (Screen.width / 2f) * targetDistanceScale.x,
            0f,
            (Screen.height / 2f) * targetDistanceScale.y);
    }

    void UpdateCamera()
    {
        // Make the middle of the screen the center
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, 0f, Input.mousePosition.y);
        Vector3 screenOrigin = GetScreenCenter() - mousePosition;

        // Scale between mouse and screen
        Vector3 scale = GetScreenScale(true, screenOrigin, screenScale);

        // Set offset by a scale of the max target distance
        Vector3 offsetPosition = new Vector3(
            targetMaxDistance.x * scale.x,
            0f,
            targetMaxDistance.y * scale.z);

        // Base position with offset of the camera
        Vector3 baseWorldOffset = targetObject.gameObject.transform.position + cameraOffset;

        // Get the final position for the camera
        Vector3 cameraTargetPosition = baseWorldOffset + offsetPosition;

        // Set the final position after smoothing it
        transform.position = Vector3.SmoothDamp(transform.position, cameraTargetPosition, ref cameraSmoothVelocity, cameraSmoothTime);
    }

    Vector3 GetScreenScale(bool _limitValues, Vector3 _screenOrigin, Vector3 _screenScale)
    {
        // Check which side of the middle we're calculating against,
        // Set to a scale of the distance from middle to mouse or cap the value
        Vector3 scale = new Vector3(_screenOrigin.x / _screenScale.x, 0f, _screenOrigin.z / _screenScale.z); // error on this line? y and z

        // Limit the value
        if (_limitValues)
        {
            // X
            if (_screenOrigin.x > 0f)
                scale.x = Mathf.Min(scale.x, 1f);
            else if (_screenOrigin.x < 0f)
                scale.x = Mathf.Max(scale.x, -1f);

            // Y
            if (_screenOrigin.z > 0f)
                scale.z = Mathf.Min(scale.z, 1f);
            else if (_screenOrigin.z < 0f)
                scale.z = Mathf.Max(scale.z, -1f);
        }

        // Invert variable
        scale *= -1;

        return scale;
    }

    Vector3 GetScreenCenter()
    {
        return new Vector3(Screen.width / 2f, 0f, Screen.height / 2f);
    }
}