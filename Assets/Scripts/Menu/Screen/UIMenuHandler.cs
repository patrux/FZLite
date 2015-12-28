using UnityEngine;
using System.Collections;

public class UIMenuHandler : MonoBehaviour
{
    // Singleton
    static public UIMenuHandler instance;
    void Awake() { instance = this; }
    void OnDestroy() { instance = null; }

    // The menu screens
    public UIScreenPlay screenPlay;
    public UIScreenLoadOut screenLoadOut;
    public UIScreenLobby screenLobby;
    public UIScreenServer screenServer;
    public UIScreenGameUI screenGameUI;

    // Current active screen
    IMenuScreen activeScreen;

    void Start()
    {
        // Find the currently active screen
        if (activeScreen == null)
        {
            if (CheckDefaultActiveScreen(screenPlay)) { }
            else if (CheckDefaultActiveScreen(screenLoadOut)) { }
            else if (CheckDefaultActiveScreen(screenLobby)) { }
            else if (CheckDefaultActiveScreen(screenServer)) { }
            else if (CheckDefaultActiveScreen(screenGameUI)) { }
        }

        print("Set Default screen to " + activeScreen);
    }

    public void OnButton_Play()
    {
        SwitchScreen(screenPlay);
    }

    public void OnButton_LoadOut()
    {
        SwitchScreen(screenLoadOut);
    }

    public void OnButton_Options()
    {
        // Reload settings
        PlayerSettings.instance.LoadSettings();

        // Opens the options file in the default text editor
        string playerSettingsPath = Application.streamingAssetsPath + "\\" + "PlayerSettings.xml";
        System.Diagnostics.Process.Start(playerSettingsPath);
    }

    public void OnButton_Exit()
    {
        // Discnnect and go to main menu if in a game
        Application.Quit();
    }

    public void SetScreenLobby()
    {
        SwitchScreen(screenLobby);
    }

    public void SetScreenServer()
    {
        SwitchScreen(screenServer);
    }

    public void SetScreenGameUI()
    {
        SwitchScreen(screenGameUI);
    }

    void SwitchScreen(IMenuScreen _menuScreen)
    {
        if (activeScreen != null)
            activeScreen.Hide();

        activeScreen = _menuScreen;

        if (activeScreen != null)
            activeScreen.Show();
    }

    bool CheckDefaultActiveScreen(IMenuScreen _menuScreen)
    {
        if (_menuScreen.GetGameObject().activeSelf)
        {
            activeScreen = _menuScreen;
            return true;
        }
        return false;
    }
}
