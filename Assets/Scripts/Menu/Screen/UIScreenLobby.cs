using UnityEngine;
using System.Collections;

public class UIScreenLobby : MonoBehaviour, IMenuScreen
{
    public ChatHistory uiChatInput;

    public UILabel[] labelPlayerSlots;

    public Color emptyColor;
    public Color takenColor;
    public Color redColor;
    public Color blueColor;

    /// <summary>
    /// Stores the button click value.
    /// </summary>
    [HideInInspector]
    public bool teamRed = true;

    public void Show()
    {
        gameObject.SetActive(true);

        GameLogic.instance.gameState = GameLogic.GameState.LOBBY;

        LobbyHandler.instance.InitializeLobbySlots();
    }

    public void Hide() // needs logic to determine if we are leaving the lobby or just entering the game
    {
        GameLogic.instance.gameState = GameLogic.GameState.INGAME;

        //if (BoltNetwork.isRunning)
        //    BoltLauncher.Shutdown();

        gameObject.SetActive(false);
    }

    public void Button_JoinRedTeam() { teamRed = true; OnJoinTeam(); }
    public void Button_JoinBlueTeam() { teamRed = false; OnJoinTeam(); }

    // Called when a join team button is pressed
    public void OnJoinTeam()
    {
        LobbyHandler.instance.JoinTeam(teamRed);
    }
}
