using UnityEngine;
using System.Collections;

public class LobbyHandler : MonoBehaviour
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
    public void Button_JoinRedTeam() { JoinTeam(true); }
    public void Button_JoinBlueTeam() { JoinTeam(false); }

    /// <summary>
    /// Triggered when the local player tries to join a team.
    /// </summary>
    public void JoinTeam(bool _teamRed)
    {
        NetPlayer localPlayer = GameLogic.instance.localNetPlayer;

        // If already on that team, do nothing
        if (_teamRed == localPlayer.IsTeamRed())
            return;

        // Get the slots
        LobbySlot oldSlot = lobbySlots[localPlayer.slotID];
        LobbySlot newSlot = GetFirstAvailableTeamLobbySlot(_teamRed);

        // Set the slots if they don't return null,
        // meaning the team was full.
        if (oldSlot != null && newSlot != null)
        {
            SetLobbySlot(oldSlot, null);
            SetLobbySlot(newSlot, localPlayer);
        }
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
            endInterval = GameLogic.instance.MAX_PLAYERS / 2;
        }
        else
        {
            startInterval = GameLogic.instance.MAX_PLAYERS / 2;
            endInterval = GameLogic.instance.MAX_PLAYERS;
        }

        for (int i = startInterval; i < endInterval; i++)
        {
            if (lobbySlots[i].IsEmpty())
                return lobbySlots[i];
        }
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
}
