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
        BoltNetwork.RegisterTokenClass<ControlToken>();
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
            PlayerSettings.SetPlayerName("Server");
            GameLogic.instance.menuHandler.SetScreenServer();
            GameLogic.instance.chatHandler.AddLocalMessage("Waiting for players to join.");
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

    /// <summary>
    /// Called on server after a client tries to connect.
    /// </summary>
    public override void ConnectRequest(UdpKit.UdpEndPoint _endPoint, IProtocolToken _connectToken)
    {
        if (GameLogic.instance.gameState == GameLogic.GameState.LOBBY)
        {
            if (GameLogic.instance.GetNetPlayerList().Count <= GameLogic.instance.MAX_PLAYERS)
            {
                ConnectToken ct = (ConnectToken)_connectToken;

                // Create and store this NetPlayer
                NetPlayer netPlayer = NetPlayer.CreateFromConnectToken(ct);

                // Get an available slotID (lobbySlot) and assign it to this player
                LobbySlot lobbySlot = LobbyHandler.instance.GetFirstAvailableLobbySlot();
                LobbyHandler.instance.SetLobbySlot(lobbySlot, netPlayer);

                // Update this players slotID create a new ConnectToken
                netPlayer.slotID = lobbySlot.GetSlotID();
                ConnectToken ctr = netPlayer.CreateConnectToken(); // response Token

                Debug.Log("[Server::ConnectRequest] Assigned " + lobbySlot.ToString());

                BoltNetwork.Accept(_endPoint, ctr);
            }
            else
            {
                WriteLine("A player [" + _endPoint.Address + "] tried to connect, but the server is full.");
                BoltNetwork.Refuse(_endPoint);
            }
        }
        else
        {
            WriteLine("A player [" + _endPoint.Address + "] tried to connect, but the server only accept connections when in lobby.");
            BoltNetwork.Refuse(_endPoint);
        }
    }

    public override void Disconnected(BoltConnection connection)
    {
        if (!BoltNetwork.isServer)
            return;

        NetPlayer np = NetPlayer.GetNetPlayer(connection);

        if (np == null)
        {
            WriteLine("Disconnected, failed to find player.");
            return;
        }

        if (GameLogic.instance.gameState == GameLogic.GameState.LOBBY)
        {
            WriteLine("Disconnected[" + np.playerName + "]");
            // update lobby slots
            GameLogic.instance.GetNetPlayerList().Remove(np);
        }
        else if (GameLogic.instance.gameState == GameLogic.GameState.INGAME)
        {
            WriteLine("Disconnected[" + np.playerName + "]");
            // delete player entity gracefully
            GameLogic.instance.GetNetPlayerList().Remove(np);
        }
    }

    void WriteLine(string _text)
    {
        GameLogic.instance.chatHandler.AddLocalMessage(_text);
        UIMenuConsole.WriteConsole(_text);
    }
}
