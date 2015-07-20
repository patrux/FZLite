using UnityEngine;
using System.Collections;

public class KeypressHighlight : MonoBehaviour
{
    public UILabel label;

    // Key
    KeyCode keyCode;

    // Colors
    public Color pressColor;
    Color originalColor;

    // Time
    public float maxColorTime;
    float keyDownTime = 0f;

    bool isInitialized = false;

    public void Initialize(string _key)
    {
        originalColor = label.color;
        keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), _key);
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized)
            return;

        if (Input.GetKeyDown(keyCode))
        {
            keyDownTime = Time.time;
        }

        if (Input.GetKey(keyCode))
        {
            float deltaTime = Time.time - keyDownTime;
            float t = Mathf.Min((deltaTime / maxColorTime), 1f);

            label.color = Color.Lerp(originalColor, pressColor, t);
        }

        if (Input.GetKeyUp(keyCode))
        {
            label.color = originalColor;
        }
    }
}
