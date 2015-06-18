using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour 
{
    static public GameLogic instance;
    void Awake() { instance = this; }
    void OnDestroy() { instance = null; }

    // State of the game
    public GameState gameState = GameState.UNCONNECTED;

    public enum GameState
    {
        UNCONNECTED,
        LOBBY,
        INGAME
    }

    public List<NetPlayer> playerList = new List<NetPlayer>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
