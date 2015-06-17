using UnityEngine;
using System.Collections;

public class UIScreenMain : MonoBehaviour, IMenuScreen 
{
    public UILabel labelPlayerName;
    public UIButton buttonCloseScreen;

	void Start () 
    {
        labelPlayerName.text = PlayerSettings.GetPlayerName();
        buttonCloseScreen.gameObject.SetActive(false);
	}

    public void Show()
    {
        gameObject.SetActive(true);
        labelPlayerName.text = PlayerSettings.GetPlayerName();
        buttonCloseScreen.gameObject.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        buttonCloseScreen.gameObject.SetActive(true);
    }
}
