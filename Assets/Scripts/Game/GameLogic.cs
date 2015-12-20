using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bolt;

public class GameLogic : MonoBehaviour //ASDF
{
    public static GameLogic instance;
    void Awake() { instance = this; }
    void OnDestroy() { instance = null; }

    /// <summary>
    /// Max allowed players.
    /// </summary>
    [HideInInspector]
    public readonly int MAX_PLAYERS = 6;

    /// <summary>
    /// Current state of the game.
    /// </summary>
    [HideInInspector]
    public GameState gameState = GameState.UNCONNECTED;
    
    // References
    [HideInInspector]
    public UIMenuHandler menuHandler;

    [HideInInspector]
    public ChatHandler chatHandler;

    /// <summary>
    /// List of all possible game states.
    /// </summary>
    public enum GameState
    {
        UNCONNECTED,
        LOBBY,
        INGAME
    }

    // Local NetPlayer
    public NetPlayer localNetPlayer = null;

    // List of active clients
    List<NetPlayer> netPlayerList = new List<NetPlayer>();

    /// <summary>
    /// Returns the list containing all connected NetPlayers.
    /// </summary>
    public List<NetPlayer> GetNetPlayerList() { return netPlayerList; }

    void Start()
    {
        menuHandler = GameObject.Find("UIRoot").GetComponent<UIMenuHandler>();
        chatHandler = GameObject.Find("GlobalScripts").GetComponent<ChatHandler>();
    }

    /// <summary>
    /// Outputs all the LobbySlots to chat log.
    /// </summary>
    public void PrintNetPlayers()
    {
        chatHandler.AddLocalMessage("----- Listing NetPlayers (Count=" + netPlayerList.Count + ") -----");
        foreach (NetPlayer np in netPlayerList)
        {
            chatHandler.AddLocalMessage(np.ToString());
        }
        chatHandler.AddLocalMessage("----- End Listing -----");
    }
}
