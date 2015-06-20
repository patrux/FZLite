using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bolt;

public class GameLogic : MonoBehaviour 
{
    static public GameLogic instance;
    void Awake() { instance = this; }
    void OnDestroy() { instance = null; }

    // Max players can't exceed 10 players, cause of
    // id assignment which caps at byte size
    public readonly int MAX_PLAYERS = 6;

    // Current state of the game
    public GameState gameState = GameState.UNCONNECTED;

    // References
    public UIMenuHandler menuHandler;

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
        menuHandler = GameObject.Find("MenuScripts").GetComponent<UIMenuHandler>();
    }
}
