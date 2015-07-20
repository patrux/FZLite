using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Client)]
public class ClientCallbacks : Bolt.GlobalEventListener
{
    /// <summary>
    /// Called for local player when connected to server.
    /// </summary>
    public override void Connected(BoltConnection connection)
    {
        ConnectToken ctr = (ConnectToken)connection.AcceptToken;

        // Update info and enter lobby
        NetPlayer localNetPlayer = GameLogic.instance.localNetPlayer;
        localNetPlayer.slotID = ctr.slotID;
        localNetPlayer.SetConnection(connection);

        GameLogic.instance.menuHandler.SetScreenLobby();
        LobbyHandler.instance.SetLobbySlot(localNetPlayer.slotID, localNetPlayer);

        Debug.Log("[Connected] Assigned slotID[" + localNetPlayer.slotID + "] playerID[" + localNetPlayer.playerID + "]");
    }

    /// <summary>
    /// Recieved data about a new other NetPlayer.
    /// </summary>
    public override void OnEvent(evNewNetPlayer _ev)
    {
        NetPlayer netPlayer = NetPlayer.CreateFromNewNetPlayerEvent(_ev);

        // If netPlayer didn't already exist
        if (netPlayer != null)
        {
            GameLogic.instance.GetNetPlayerList().Add(netPlayer);
            LobbyHandler.instance.SetLobbySlot(netPlayer.slotID, netPlayer);

            Debug.Log("[NewPlayer] Created and added: " + netPlayer.ToString());
        }
    }

    public override void OnEvent(evJoinTeam _ev)
    {
        // Get LobbySlot from slotID
        LobbySlot fromSlot = LobbyHandler.instance.GetLobbySlotBySlotID((byte)_ev.slotID);

        // Move to the selected team
        LobbySlot toSlot = LobbyHandler.instance.MoveTeam(fromSlot, _ev.teamRed);
    }

    public override void OnEvent(evReadyUp _ev)
    {
        // Set ready status of NetPlayer
        NetPlayer netPlayer = NetPlayer.GetNetPlayer((uint)_ev.playerID);
        netPlayer.SetReadyStatus(_ev.readyStatus);
    }

    public override void OnEvent(evChatMessage _ev)
    {
        GameLogic.instance.chatHandler.AddChatMessage(_ev.timeStamp + _ev.senderName + _ev.message);
    }
}
