using UnityEngine;
using System.Collections;

public class UIMenuConsole : MonoBehaviour
{
    static protected UIMenuConsole instance;
    void Awake() { instance = this; }
    void OnDestroy() { instance = null; }

    UILabel consoleLabel;

    bool hasText = false;

    float timerDisplay = 0f;
    float timerDisplayDuration = 0f;

    float timerDisplayDurationBase = 2f;
    float timerDisplayDurationPerLetter = 0.05f;

    float timerFade = 0f;
    float timerFadeDuration = 1f;
    bool fadeText = false;

    void Start()
    {
        consoleLabel = GetComponent<UILabel>();
        consoleLabel.text = "";
    }

    float GetDuration(string _text)
    {
        return timerDisplayDurationBase + (_text.Length * timerDisplayDurationPerLetter);
    }

    void Update()
    {
        if (hasText)
        {
            if (fadeText)
            {
                timerFade += Time.deltaTime;

                consoleLabel.color = SetColorAlpha(
                    consoleLabel.color,
                    Mathf.Lerp(1.0f, 0.0f, timerFade / timerFadeDuration));

                if (timerFade > timerFadeDuration)
                {
                    hasText = false;
                    fadeText = false;
                    timerFade = 0f;
                    consoleLabel.text = "";
                }
            }
            else
            {
                timerDisplay += Time.deltaTime;

                if (timerDisplay > timerDisplayDuration)
                {
                    fadeText = true;
                    timerDisplay = 0f;
                }
            }
        }
    }

    public static void WriteConsole(string _text)
    {
        if (instance != null)
            instance.WriteConsoleAux(_text);
    }

    void WriteConsoleAux(string _text)
    {
        fadeText = false;
        hasText = true;
        timerFade = 0f;
        timerDisplay = 0f;

        consoleLabel.text = _text;
        timerDisplayDuration = GetDuration(_text);

        consoleLabel.color = SetColorAlpha(consoleLabel.color, 1.0f);

        Debug.Log("[WriteMenuConsole] [" + _text + "]");
    }

    Color SetColorAlpha(Color _color, float _alpha)
    {
        return new Color(_color.r, _color.g, _color.g, _alpha);
    }
}
