using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using Bolt;

// Contains the events for server and client
public class NetworkHandler : Bolt.GlobalEventListener
{
    UIMenuHandler menuHandler;
    
    // The current gamestate of the player
    public static GameState gameState = GameState.UNCONNECTED;

    public enum GameState
    {
        UNCONNECTED,
        LOBBY,
        INGAME
    }
    
    ushort port = 0;

    void Start()
    {
        menuHandler = GameObject.Find("MenuScripts").GetComponent<UIMenuHandler>();
    }

    /// <summary>
    /// Create new server and enter lobby.
    /// </summary>
    public void CreateServer(int _port)
    {
        PlayerSettings.SetPlayerName(("ptxServer")); // set debug name

        port = (ushort)_port;
        BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, (ushort)_port));

        WriteLine("Creating server on port " + _port);
    }

    /// <summary>
    /// Try to connect to server.
    /// </summary>
    public void JoinServer(string _ip, int _port)
    {
        PlayerSettings.SetPlayerName(("ptxClient")); // set debug name

        port = (ushort)_port;

        BoltLauncher.StartClient(UdpEndPoint.Any);

        WriteLine("Attempting to join " + _ip + ":" + _port);
    }

    public override void BoltStartBegin()
    {
        // Register token classes with Bolt
        BoltNetwork.RegisterTokenClass<ConnectToken>();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.isServer)
        {
            menuHandler.EnterLobby();
        }
        else if (BoltNetwork.isClient) // CONNECT TO THE SERVER
        {
            // Store loadout etc here
            ConnectToken ct = new ConnectToken(PlayerSettings.GetPlayerName(), UIScreenLobby.lobbyReady);

            // Enter lobby on client screen
            menuHandler.EnterLobby();

            // Try to connect to the server
            BoltNetwork.Connect(new UdpEndPoint(UdpIPv4Address.Parse("127.0.0.1"), (ushort)port), ct);
        }
    }

    /// <summary>
    /// Called on server after a client tries to connect.
    /// </summary>
    public override void ConnectRequest(UdpKit.UdpEndPoint _endPoint, IProtocolToken _token)
    {
        ConnectToken cct = (ConnectToken)_token;

        // Update lobby name for the client
        UIScreenLobby lobbyScreen = GameObject.Find("LobbyScreen").GetComponent<UIScreenLobby>();
        lobbyScreen.SetLobbyName(false, cct.playerName);

        // Create a token for listenserver and send to client as a token
        ConnectToken sct = new ConnectToken(PlayerSettings.GetPlayerName(), UIScreenLobby.lobbyReady);

        BoltNetwork.Accept(_endPoint, sct);
    }

    /// <summary>
    /// Called on both client and server after an accepted connection.
    /// </summary>
    public override void Connected(BoltConnection connection)
    {
        ConnectToken sct = (ConnectToken)connection.AcceptToken;
        Debug.Log("[Connected] player[" + sct.playerName + "]");

        if (BoltNetwork.isClient)
        {
            // Update lobby name for the server player
            UIScreenLobby lobbyScreen = GameObject.Find("LobbyScreen").GetComponent<UIScreenLobby>();
            lobbyScreen.SetLobbyName(true, sct.playerName);
            lobbyScreen.SetLobbyReadyStatus(true, sct.readyStatus);
        }
    }

    void WriteLine(string _text)
    {
        UIMenuConsole.WriteConsole(_text);
    }
}
