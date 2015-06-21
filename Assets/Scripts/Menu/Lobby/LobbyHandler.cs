using UnityEngine;
using System.Collections;

public class LobbyHandler : Bolt.GlobalEventListener
{
    static public LobbyHandler instance;
    void Awake() { instance = this; }
    void OnDestroy() { instance = null; }

    // Player lobby slots
    LobbySlot[] lobbySlots;

    void Start()
    {
        lobbySlots = new LobbySlot[GameLogic.instance.MAX_PLAYERS];

        for (int i = 0; i < lobbySlots.Length; i++)
        {
            lobbySlots[i] = new LobbySlot((byte)i);
        }
    }

    /// <summary>
    /// Call on client upon entering lobby.
    /// Note: Only client.
    /// </summary>
    public void InitializeLobbySlots()
    {
        UIScreenLobby screenLobby = GameObject.Find("LobbyScreen").GetComponent<UIScreenLobby>();

        for (int i = 0; i < lobbySlots.Length; i++)
            lobbySlots[i].SetUILabel(screenLobby.labelPlayerSlots[i]);
    }

    // UI button click events
    //public void Button_JoinRedTeam() { JoinTeam(true); }
    //public void Button_JoinBlueTeam() { JoinTeam(false); }

    /// <summary>
    /// Triggered when the local player tries to join a team.
    /// Note: Only local client.
    /// </summary>
    public void JoinTeam(bool _teamRed)
    {
        NetPlayer localPlayer = GameLogic.instance.localNetPlayer;

        //// If already on that team, do nothing
        if (_teamRed == localPlayer.IsTeamRed())
            return;

        // Send event to server
        evJoinTeam jt = evJoinTeam.Create(Bolt.GlobalTargets.OnlyServer);
        jt.slotID = localPlayer.slotID;
        jt.teamRed = _teamRed;
        jt.Send();
    }

    /// <summary>
    /// Set the occupying NetPlayer of target slot.
    /// Note: Called by server
    /// </summary>
    public void SetLobbySlot(LobbySlot _lobbySlot, NetPlayer _netPlayer)
    {
        _lobbySlot.SetNetPlayer(_netPlayer);
    }

    /// <summary>
    /// Set the occupying NetPlayer of target slot.
    /// Note: Called by server
    /// </summary>
    public void SetLobbySlot(byte _slotID, NetPlayer _netPlayer)
    {
        for (int i = 0; i < GameLogic.instance.MAX_PLAYERS; i++)
        {
            if (lobbySlots[i].GetSlotID() == _slotID)
            {
                lobbySlots[i].SetNetPlayer(_netPlayer);
                break;
            }
        }
    }

    /// <summary>
    /// Moves the NetPlayer to the selected team.
    /// Note: Only client.
    /// </summary>
    public LobbySlot MoveTeam(LobbySlot _fromLobbySlot, bool _teamRed)
    {
        LobbySlot lobbySlot = null;
        NetPlayer netPlayer = _fromLobbySlot.GetNetPlayer();

        // Get the slots
        LobbySlot newSlot = GetFirstAvailableTeamLobbySlot(_teamRed);

        // If new slot isn't null (taken/full), move NetPlayer to slot
        if (newSlot != null)
        {
            SetLobbySlot(_fromLobbySlot, null);
            SetLobbySlot(newSlot, netPlayer);
            lobbySlot = newSlot;
        }
        return lobbySlot;
    }

    /// <summary>
    /// Tries to move the NetPlayer to the selected team.
    /// Note: Only server.
    /// </summary>
    public LobbySlot TryMoveTeam(LobbySlot _fromLobbySlot, bool _teamRed)
    {
        LobbySlot lobbySlot = null;
        NetPlayer netPlayer = _fromLobbySlot.GetNetPlayer();

        //if (netPlayer == null)
        //    return lobbySlot;

        // If already on selected team
        if (_teamRed == netPlayer.IsTeamRed())
            return lobbySlot;

        // Get the slots
        LobbySlot newSlot = GetFirstAvailableTeamLobbySlot(_teamRed);

        // If new slot isn't null (taken/full), move NetPlayer to slot
        if (newSlot != null)
        {
            SetLobbySlot(_fromLobbySlot, null);
            SetLobbySlot(newSlot, netPlayer);
            lobbySlot = newSlot;
        }
        return lobbySlot;
    }

    /// <summary>
    /// Returns a LobbySlot from slotID.
    /// </summary>
    public LobbySlot GetLobbySlotBySlotID(byte _slotID)
    {
        for (int i = 0; i < GameLogic.instance.MAX_PLAYERS; i++)
            if (lobbySlots[i].GetSlotID() == _slotID)
                return lobbySlots[i];
        return null;
    }

    /// <summary>
    /// Get the first available lobbySlot on selected team.
    /// Returns null if the team is full.
    /// </summary>
    public LobbySlot GetFirstAvailableTeamLobbySlot(bool _teamRed)
    {
        int startInterval;
        int endInterval;

        if (_teamRed)
        {
            startInterval = 0;
            endInterval = lobbySlots.Length / 2;
        }
        else
        {
            startInterval = lobbySlots.Length / 2;
            endInterval = lobbySlots.Length;
        }

        for (int i = startInterval; i < endInterval; i++)
            if (lobbySlots[i].IsEmpty())
                return lobbySlots[i];
        return null;
    }

    /// <summary>
    /// Get the first available lobbySlot (team-independent).
    /// Returns null if there are no slots.
    /// </summary>
    public LobbySlot GetFirstAvailableLobbySlot()
    {
        for (int i = 0; i < lobbySlots.Length; i++)
        {
            if (lobbySlots[i].IsEmpty())
                return lobbySlots[i];
        }
        return null;
    }

    /// <summary>
    /// Outputs all the LobbySlots to chat log.
    /// </summary>
    public void PrintLobbySlots()
    {
        GameLogic.instance.chatHandler.AddLocalMessage("----- Listing LobbySlots (Length=" + lobbySlots.Length + ") -----");
        for (int i = 0; i < lobbySlots.Length; i++)
        {
                GameLogic.instance.chatHandler.AddLocalMessage(lobbySlots[i].ToString());
        }
        GameLogic.instance.chatHandler.AddLocalMessage("----- End Listing -----");
    }
}
