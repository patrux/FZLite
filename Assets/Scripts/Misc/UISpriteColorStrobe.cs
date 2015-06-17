using UnityEngine;
using System.Collections;

public class UISpriteColorStrobe : MonoBehaviour 
{
    UISprite sprite;

    [Range(0f, 60f)]
    public float strobeDuration;

    [Range(0f, 1f)]
    public float strobeAlphaMax;

    [Range(0f, 1f)]
    public float strobeAlphaMin;

    float timer = 0f;
    bool isFadingIn = true;

	void Start () 
    {
        sprite = GetComponent<UISprite>();
	}
	
	void Update () 
    {
        timer += Time.deltaTime;
        FadeAlpha();
        CheckFadeValue();
	}

    void FadeAlpha()
    {
        if (isFadingIn)
            // Fade from alphaMin to alphaMax over time
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(strobeAlphaMin, strobeAlphaMax, timer/strobeDuration));
        else
            // Fade from alphaMax to alphaMin over time
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(strobeAlphaMax, strobeAlphaMin, timer/strobeDuration));
    }

    void CheckFadeValue()
    {
        if (timer > strobeDuration) // Flip fade target
        {
            timer = 0f;
            isFadingIn = isFadingIn ? false : true;
        }
    }
}
