using UnityEngine;
using System.Collections;

/// <summary>
/// Representation of the lobby slots.
/// </summary>
public class LobbySlot
{
    // Set at instantiation
    byte slotID = 0;
    UILabel label = null;

    // The occupant of this slot
    NetPlayer netPlayer = null;

    /// <summary>
    /// Instantiation.
    /// </summary>
    public LobbySlot(byte _slotID)
    {
        slotID = _slotID;
    }

    /// <summary>
    /// Set the occupying NetPlayer of this slot.
    /// </summary>
    public void SetNetPlayer(NetPlayer _netPlayer)
    {
        // Handle ready event
        if (_netPlayer != null)
            _netPlayer.OnIsReadyChanged += UpdateSlotColor;
        else
            _netPlayer.OnIsReadyChanged -= UpdateSlotColor;

        netPlayer = _netPlayer;
        UpdateSlot();
    }

    /// <summary>
    /// Sets the label.
    /// Note: Call on clients upon entering lobby.
    /// </summary>
    public void SetUILabel(UILabel _label)
    {
        label = _label;
        UpdateSlot();
    }

    /// <summary>
    /// Update the name and color of this slot.
    /// </summary>
    void UpdateSlot()
    {
        if (IsEmpty()) // A player left the slot
            SetLabelText(FZLang.lobby_EmptySlotName);
        else // A player entered the slot
            SetLabelText(netPlayer.playerName);
        
        if (!BoltNetwork.isServer) // no need to update the color for the server
            UpdateSlotColor();
    }

    /// <summary>
    // Set the color of this slot based on which team 
    // it belongs to and the ready status of occupying NetPlayer.
    /// </summary>
    void UpdateSlotColor()
    {
        UIScreenLobby screenLobby = GameObject.Find("LobbyScreen").GetComponent<UIScreenLobby>();

        //GameObject go = GameObject.Find("LobbyScreen");
        //UIScreenLobby screenLobby;

        //if (go != null)
        //    screenLobby = go.GetComponent<UIScreenLobby>();
        //else
        //    return;

        if (IsEmpty())
            label.color = screenLobby.emptyColor;
        else
        {
            Color teamColor = IsTeamRed() ? screenLobby.redColor : screenLobby.blueColor;
            label.color = netPlayer.isReady ? teamColor : screenLobby.takenColor;
        }
    }

    void SetLabelText(string _text)
    {
        if (label != null)
            label.text = _text;
    }

    public bool IsTeamRed() { return (slotID <= (GameLogic.instance.MAX_PLAYERS / 2)); }
    public bool IsEmpty() { return (netPlayer == null); }
    public byte GetSlotID() { return slotID; }
    public UILabel GetLabel() { return label; }
    public string ToString() { return "[LobbySlot] slotID[" + slotID + "] netPlayer[" + netPlayer.ToString() + "]"; }
}
