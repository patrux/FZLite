using UnityEngine;
using System.Collections;

public class UIScreenLobby : MonoBehaviour, IMenuScreen
{
    public UIChatInput uiChatInput;

    public UILabel labelPlayerRed;
    public UILabel labelPlayerBlue;

    public Color neutralColor;
    public Color redColor;
    public Color blueColor;

    public static bool lobbyReady = false;
    public static bool isCountingDown = false;

    int countdownIndex = 5;
    public static string mapName = "TestMap";

    public void BeginCountdown()
    {
        if (isCountingDown)
            return;

        isCountingDown = true;
        countdownIndex = 5;
        uiChatInput.AddLocalMessage("Game beginning in " + countdownIndex + "...");
        InvokeRepeating("LoopCountdown", 1.0f, 1.0f);
    }

    void LoopCountdown()
    {
        countdownIndex--;

        if (countdownIndex < 1)
        {
            CountdownFinished();
            CancelInvoke();
        }
        else
            uiChatInput.AddLocalMessage(countdownIndex + "...");
    }

    void CountdownFinished()
    {
        uiChatInput.AddLocalMessage("Loading map [" + mapName + "]");

        if (BoltNetwork.isServer)
        {
            BoltNetwork.LoadScene(mapName);
            isCountingDown = false;
        }
    }

    /// <summary>
    /// Sets the color of the name text to indicate
    /// whether the player is ready or not.
    /// </summary>
    public void SetLobbyReadyStatus(bool _playerRed, bool _ready)
    {
        if (_playerRed)
            if (_ready)
                labelPlayerRed.color = redColor;
            else
                labelPlayerRed.color = neutralColor;
        else
            if (_ready)
                labelPlayerBlue.color = blueColor;
            else
                labelPlayerBlue.color = neutralColor;
    }

    public void SetLobbyName(bool _playerRed, string _name)
    {
        // Set the name
        if (_playerRed)
            labelPlayerRed.text = _name;
        else
            labelPlayerBlue.text = _name;

        // Reset ready status
        SetLobbyReadyStatus(_playerRed, false);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        GameLogic.instance.gameState = GameLogic.GameState.LOBBY;
        UIScreenLobby.lobbyReady = false;

        SetLobbyName(BoltNetwork.isServer, PlayerSettings.GetPlayerName());
    }

    public void Hide()
    {
        GameLogic.instance.gameState = GameLogic.GameState.UNCONNECTED;
        UIScreenLobby.lobbyReady = false;

        labelPlayerRed.text = "";
        labelPlayerBlue.text = "";

        uiChatInput.ClearChat(true);

        if (BoltNetwork.isRunning)
            BoltLauncher.Shutdown();

        gameObject.SetActive(false);
    }
}
