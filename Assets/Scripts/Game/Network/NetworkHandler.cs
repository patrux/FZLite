using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using Bolt;

/// <summary>
/// Shared callbacks for server and client.
/// </summary>
public class NetworkHandler : Bolt.GlobalEventListener
{
    // Connection settings
    string ip;
    int port;

    void Start()
    {
        ip = PlayerSettings.GetIP();
        port = PlayerSettings.GetPort();
    }

    /// <summary>
    /// Bolt start.
    /// </summary>
    public override void BoltStartBegin()
    {
        // Register token classes
        BoltNetwork.RegisterTokenClass<ConnectToken>();
        BoltNetwork.RegisterTokenClass<ControlGainedToken>();
    }

    /// <summary>
    /// Create new server.
    /// </summary>
    public void CreateServer(int _port)
    {
        port = _port;
        BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, (ushort)_port));

        WriteLine("Creating server on port " + _port);
    }

    /// <summary>
    /// Connect to server.
    /// </summary>
    public void JoinServer(string _ip, int _port)
    {
        ip = _ip;
        port = _port;
        BoltLauncher.StartClient(UdpEndPoint.Any);

        WriteLine("Connecting to " + _ip + ":" + _port);
    }

    /// <summary>
    /// Called after trying to join/host a game.
    /// </summary>
    public override void BoltStartDone()
    {
        // Reset previous session
        GameLogic.instance.localNetPlayer = null;

        if (BoltNetwork.isServer)
        {
            // Load server GUI screen here
            GameLogic.instance.menuHandler.EnterServerScreen();
        }
        else if (BoltNetwork.isClient)
        {
            // Create a local NetPlayer since this is a client
            GameLogic.instance.localNetPlayer = NetPlayer.CreateLocalNetPlayer();

            // Create a connect token for the player
            ConnectToken ct = GameLogic.instance.localNetPlayer.CreateConnectToken();

            // Try to connect to the server with the token
            BoltNetwork.Connect(new UdpEndPoint(UdpIPv4Address.Parse(ip), (ushort)port), ct);
        }
    }

    // todo:
    // track/compare clients by uint playerID instead of name
    // only send "all other players" to the new player instead of everyone
    // ready up & moving slots
    // leaving in lobby = quits game?



    /// <summary>
    /// Called on server after a client tries to connect.
    /// </summary>
    public override void ConnectRequest(UdpKit.UdpEndPoint _endPoint, IProtocolToken _connectToken)
    {
        ConnectToken connectToken = (ConnectToken)_connectToken;

        if (GameLogic.instance.gameState == GameLogic.GameState.LOBBY)
        {
            if (GameLogic.instance.GetNetPlayerList().Count <= GameLogic.instance.MAX_PLAYERS)
            {
                // Create and store this NetPlayer
                NetPlayer netPlayer = NetPlayer.CreateFromConnectToken(connectToken);

                // Assign slotID and playerID
                // Get an available slot and move this player to it
                LobbySlot lobbySlot = LobbyHandler.instance.GetFirstAvailableLobbySlot();
                LobbyHandler.instance.SetLobbySlot(lobbySlot, netPlayer);

                Debug.Log("[Server] Assigned " + lobbySlot.ToString());

                // Update this players slotID create a new ConnectToken
                netPlayer.slotID = lobbySlot.GetSlotID();
                ConnectToken ctr = netPlayer.CreateConnectToken();

                // Send this NetPlayer to other NetPlayers
                netPlayer.CreateNewNetPlayerEvent();

                // Send other NetPlayers to this NetPlayer
                foreach (NetPlayer np in GameLogic.instance.GetNetPlayerList())
                    np.CreateNewNetPlayerEvent();

                // Accept the connection and send back ConnectTokenResponse
                BoltNetwork.Accept(_endPoint, ctr);
            }
            else
            {
                WriteLine("A player named [" + connectToken.playerName + "] tried to connect, but the server is full.");
                BoltNetwork.Refuse(_endPoint);
            }
        }
        else
        {
            WriteLine("A player named [" + connectToken.playerName + "] tried to connect, but the server only accept connections when in lobby.");
            BoltNetwork.Refuse(_endPoint);
        }
    }

    void WriteLine(string _text)
    {
        UIMenuConsole.WriteConsole(_text);
    }
}
