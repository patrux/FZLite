using UnityEngine;
using System.Collections;

public class UIScreenServer : MonoBehaviour, IMenuScreen
{
    public UIChatInput uiChatInput;

    public void Show() 
    {
        GameLogic.instance.gameState = GameLogic.GameState.LOBBY;
        //LobbyHandler.instance.Initialize();
    }

    public void Hide() 
    {
        GameLogic.instance.gameState = GameLogic.GameState.UNCONNECTED;
    }
}
