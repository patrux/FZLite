using UnityEngine;
using System.Collections;

public class UIScreenLobby : MonoBehaviour, IMenuScreen
{
    public UIChatInput uiChatInput;

    // 0 1 2 = red
    // 3 4 5 = blue
    public UILabel[] labelPlayerSlots;

    public Color emptyColor;
    public Color takenColor;
    public Color redColor;
    public Color blueColor;

    public void Show()
    {
        gameObject.SetActive(true);

        GameLogic.instance.gameState = GameLogic.GameState.LOBBY;

        LobbyHandler.instance.InitializeLobbySlots();
    }

    public void Hide()
    {
        GameLogic.instance.gameState = GameLogic.GameState.UNCONNECTED;

        uiChatInput.ClearChat(true);

        if (BoltNetwork.isRunning)
            BoltLauncher.Shutdown();

        gameObject.SetActive(false);
    }
}
