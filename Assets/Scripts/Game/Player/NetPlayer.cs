using UnityEngine;
using System.Collections;
using Bolt;

public class NetPlayer
{
    /// <summary>
    /// Player display name
    /// </summary>
    public string playerName;

    /// <summary>
    /// ID which is used to track the NetPlayer.
    /// </summary>
    public uint playerID;

    /// <summary>
    /// Current game slot
    /// </summary>
    public byte slotID;
    
    /// <summary>
    /// The NetPlayers connection with the server.
    /// Note: Ued by server.
    /// </summary>
    public BoltConnection connection = null;

    /// <summary>
    /// Lobby ready status.
    /// </summary>
    public bool isReady;

    /// <summary>
    /// Set isReady through this method to also trigger the OnIsReadyChanged event.
    /// </summary>
    public void SetReadyStatus(bool _isReady)
    {
        isReady = _isReady;

        if (GameLogic.instance.gameState == GameLogic.GameState.LOBBY && BoltNetwork.isClient)
            LobbyHandler.instance.GetLobbySlotBySlotID(slotID).UpdateSlot();
    }

    /// <summary>
    /// Is NetPlayer on TeamRed.
    /// </summary>
    public bool IsTeamRed() { return (slotID < (GameLogic.instance.MAX_PLAYERS / 2)); }

    /// <summary>
    /// Sets BoltConnection and playerID.
    /// </summary>
    public void SetConnection(BoltConnection _connection) { connection = _connection; playerID = connection.ConnectionId; }

    /// <summary>
    /// Creates a NetPlayer with local settings.
    /// </summary>
    public static NetPlayer CreateLocalNetPlayer()
    {
        NetPlayer netPlayer = new NetPlayer();
        netPlayer.playerName = PlayerSettings.GetPlayerName();
        netPlayer.isReady = false;
        netPlayer.slotID = 0;
        netPlayer.playerID = 0;

        GameLogic.instance.GetNetPlayerList().Add(netPlayer);
        return netPlayer;
    }

    #region TokenCreation
    /// <summary>
    /// Creates a ConnectToken from NetPlayer settings.
    /// </summary>
    public ConnectToken CreateConnectToken()
    {
        ConnectToken ct = new ConnectToken();
        ct.playerName = playerName;
        ct.isReady = isReady;
        ct.slotID = slotID;
        ct.playerID = playerID;
        return ct;
    }

    /// <summary>
    /// Creates a NetPlayer from ConnectionToken settings.
    /// Note: Server only.
    /// </summary>
    public static NetPlayer CreateFromConnectToken(ConnectToken _connectToken)
    {
        NetPlayer p = new NetPlayer();
        p.playerName = _connectToken.playerName;
        p.isReady = _connectToken.isReady;
        p.slotID = _connectToken.slotID;
        p.playerID = _connectToken.playerID;

        GameLogic.instance.GetNetPlayerList().Add(p);
        return p;
    }
    #endregion

    #region EventCreation
    /// <summary>
    /// Sends this NetPlayer to connection.
    /// Note: Server only.
    /// </summary>
    public void CreateNewNetPlayerEvent(BoltConnection _connection)
    {
        if (!BoltNetwork.isServer)
            return;

        if (_connection == null)
        {
            Debug.Log("CreateNewNetPlayerEvent _connection was null, returning");
            return;
        }

        evNewNetPlayer newNetPlayer = evNewNetPlayer.Create(_connection);
        newNetPlayer.playerName = playerName;
        newNetPlayer.isReady = isReady;
        newNetPlayer.slotID = slotID;
        newNetPlayer.playerID = (int)playerID;
        newNetPlayer.Send();
    }

    /// <summary>
    /// Creates a NewNetPlayerEvent. Returns null if the NetPlayer already exist.
    /// Note: Client only.
    /// </summary>
    public static NetPlayer CreateFromNewNetPlayerEvent(evNewNetPlayer _ev)
    {
        NetPlayer p = new NetPlayer();
        p.playerName = _ev.playerName;
        p.isReady = _ev.isReady;
        p.slotID = (byte)_ev.slotID;
        p.playerID = (uint)_ev.playerID;

        if (AlreadyExist(p))
            return null;
        else
            return p;
    }
    #endregion

    #region GetNetPlayer
    /// <summary>
    /// Get NetPlayer from slotID.
    /// Note: Compares slotIDs.
    /// </summary>
    public static NetPlayer GetNetPlayer(byte _slotID)
    {
        foreach (NetPlayer np in GameLogic.instance.GetNetPlayerList())
            if (np.slotID == _slotID)
                return np;
        return null;
    }

    /// <summary>
    /// Get NetPlayer from playerID.
    /// Note: Compares playerIDs.
    /// </summary>
    public static NetPlayer GetNetPlayer(uint _playerID)
    {
        foreach (NetPlayer np in GameLogic.instance.GetNetPlayerList())
            if (_playerID == np.playerID)
                return np;
        return null;
    }

    /// <summary>
    /// Get NetPlayer from BoltConnection.
    /// Note: Compares playerIDs.
    /// </summary>
    public static NetPlayer GetNetPlayer(BoltConnection _connection)
    {
        uint playerID = _connection.ConnectionId;

        foreach (NetPlayer np in GameLogic.instance.GetNetPlayerList())
            if (playerID == np.playerID)
                return np;
        Debug.Log("[GetNetPlayer::Connection] Return null.");
        return null;
    }
#endregion

    /// <summary>
    /// Returns whether the NetPlayer already exists in NetPlayerList.
    /// </summary>
    public static bool AlreadyExist(NetPlayer _netPlayer)
    {
        NetPlayer netPlayerMatch = null;

        foreach (NetPlayer np in GameLogic.instance.GetNetPlayerList())
        {
            if (np.playerID == _netPlayer.playerID)
            {
                netPlayerMatch = _netPlayer;
                break;
            }
        }
        Debug.Log("[AlreadyExist::Exists] Tried to create: " + _netPlayer.ToString());
        return (netPlayerMatch != null);
    }

    public string ToString() { return "playerName[" + playerName + "] slotID[" + slotID + "] playerID[" + playerID + "] connection[" + connection + "]"; }
}
