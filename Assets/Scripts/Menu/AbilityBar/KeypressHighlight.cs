using UnityEngine;
using System.Collections;

public class KeypressHighlight : MonoBehaviour 
{
    public UILabel label;

    public string key;

    public Color pressColor;
    public float maxColorTime;

    Color originalColor;

    float keyDownTime = 0f;

	void Start () 
    {
        originalColor = label.color;
	}
	
	void Update () 
    {
        KeyCode kc = (KeyCode)System.Enum.Parse(typeof(KeyCode), key);

        if (Input.GetKeyDown(kc))
        {
            keyDownTime = Time.time;
        }

        if (Input.GetKey(kc))
        {
            float deltaTime = Time.time - keyDownTime;
            float t = Mathf.Min((deltaTime / maxColorTime), 1f);

            label.color = Color.Lerp(originalColor, pressColor, t);
        }

        if (Input.GetKeyUp(kc))
        {
            label.color = originalColor;
        }
	}
}
