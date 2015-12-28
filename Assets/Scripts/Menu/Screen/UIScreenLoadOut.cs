using UnityEngine;
using System.Collections;

public class UIScreenLoadOut : MonoBehaviour, IMenuScreen
{
    public UIInput inputPlayerName;

    void Start()
    {
        inputPlayerName.value = PlayerSettings.GetPlayerName();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        inputPlayerName.value = PlayerSettings.GetPlayerName();
    }

    public void Hide()
    {
        string name = NGUIText.StripSymbols(inputPlayerName.value); // Remove special NGUI text code

        if (!string.IsNullOrEmpty(name))
        {
            PlayerSettings.SetPlayerName(name);
            PlayerSettings.SaveSettingsToFile();
        }

        gameObject.SetActive(false);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
