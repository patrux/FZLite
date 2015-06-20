using UnityEngine;
using System.Collections;
using Bolt;

public class NetPlayer
{
    // NetPlayer Events
    public delegate void EventHandler();
    public event EventHandler OnIsReadyChanged;

    /// <summary>
    /// Lobby ready status.
    /// </summary>
    public bool isReady;

    // Player name
    public string playerName;

    // Game slot
    public byte slotID;

    // Server only
    public BoltConnection connection = null;

    // Client only
    public uint playerID;

    /// <summary>
    /// Set isReady through this method to also trigger the OnIsReadyChanged event.
    /// </summary>
    public void SetReadyStatus(bool _isReady) { isReady = _isReady; OnIsReadyChanged(); }

    /// <summary>
    /// Is NetPlayer on TeamRed.
    /// </summary>
    public bool IsTeamRed() { return (slotID <= (GameLogic.instance.MAX_PLAYERS / 2)); }

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
    /// Sends this NetPlayer to all other connected NetPlayers.
    /// Note: Server only.
    /// </summary>
    public void CreateNewNetPlayerEvent()
    {
        if (!BoltNetwork.isServer)
            return;

        evNewNetPlayer newNetPlayer = evNewNetPlayer.Create();
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

    /// <summary>
    /// Returns whether the NetPlayer already exists in NetPlayerList.
    /// </summary>
    public static bool AlreadyExist(NetPlayer _netPlayer)
    {
        NetPlayer netPlayerMatch = null;

        foreach (NetPlayer np in GameLogic.instance.GetNetPlayerList())
        {
            if (np.playerName == _netPlayer.playerName)
            {
                netPlayerMatch = _netPlayer;
                break;
            }
        }
        Debug.Log("[NewPlayer::Exists] Tried to create: " + _netPlayer.ToString());
        return (netPlayerMatch != null);
    }

    public string ToString() { return "[NetPlayer] playerName[" + playerName + "] slotID[" + slotID + "] playerID[" + playerID + "]"; }
}
