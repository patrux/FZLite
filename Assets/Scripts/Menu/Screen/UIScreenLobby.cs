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

    int countdownIndex = 5;
    string mapName = "TestMap";

    public void BeginCountdown()
    {
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
            BoltNetwork.LoadScene(mapName);
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

        NetworkHandler.gameState = NetworkHandler.GameState.LOBBY;
        NetworkHandler.lobbyReady = false;

        SetLobbyName(BoltNetwork.isServer, PlayerSettings.GetPlayerName());
    }

    public void Hide()
    {
        NetworkHandler.gameState = NetworkHandler.GameState.UNCONNECTED;
        NetworkHandler.lobbyReady = false;

        labelPlayerRed.text = "";
        labelPlayerBlue.text = "";

        uiChatInput.ClearChat(true);

        if (BoltNetwork.isRunning)
            BoltLauncher.Shutdown();

        gameObject.SetActive(false);
    }
}
