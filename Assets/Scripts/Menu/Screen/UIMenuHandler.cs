using UnityEngine;
using System.Collections;

public class UIMenuHandler : MonoBehaviour 
{
    // Singleton
    static public UIMenuHandler instance;
    void Awake() { instance = this; }
    void OnDestroy() { instance = null; }

    // The menu screens
    public UIScreenMain screenMain;
    public UIScreenLoadOut screenLoadOut;
    public UIScreenPlay screenPlay;
    public UIScreenLobby screenLobby;
    public UIScreenServer screenServer;

    // Current active screen
    IMenuScreen activeScreen;

	void Start () 
    {
        // Set default screen
        GameObject mainScreen = GameObject.Find("MainScreen");
        if (mainScreen != null)
            activeScreen = mainScreen.GetComponent<UIScreenMain>();
	}

    public void OnButton_CloseScreen()
    {
        if (activeScreen != null)
        {
            activeScreen.Hide();
            activeScreen = screenMain;
            activeScreen.Show();
        }
    }

    public void OnButton_LoadOut()
    {
        if (activeScreen != null)
        {
            activeScreen.Hide();
            activeScreen = screenLoadOut;
            activeScreen.Show();
        }
    }

    public void OnButton_Play()
    {
        if (activeScreen != null)
        {
            activeScreen.Hide();
            activeScreen = screenPlay;
            activeScreen.Show();
        }
    }

    public void EnterLobby()
    {
        if (activeScreen != null)
        {
            activeScreen.Hide();
            activeScreen = screenLobby;
            activeScreen.Show();
        }
    }
}
