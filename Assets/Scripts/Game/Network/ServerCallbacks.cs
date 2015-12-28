using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Host)]
public class ServerCallbacks : Bolt.GlobalEventListener
{
    // Get sender NetPlayer
    //NetPlayer netPlayer = NetPlayer.GetNetPlayerFromConnection(_ev.RaisedBy);

    public override void Connected(BoltConnection connection)
    {
        ConnectToken ct = (ConnectToken)connection.AcceptToken;

        // Get the correct NetPlayer by the connection
        NetPlayer netPlayer = NetPlayer.GetNetPlayer(ct.slotID);

        // Set connection (and playerID) for this NetPlayer
        netPlayer.SetConnection(connection);

        // Send this NetPlayer to other NetPlayers (send to all, excluding self)
        foreach (NetPlayer np in GameLogic.instance.GetNetPlayerList())
            if (netPlayer.playerID != np.playerID) // if not self
                netPlayer.CreateNewNetPlayerEvent(np.connection);

        // Send other NetPlayers to this NetPlayer
        foreach (NetPlayer np in GameLogic.instance.GetNetPlayerList())
            if (netPlayer.playerID != np.playerID) // if not self
                np.CreateNewNetPlayerEvent(netPlayer.connection);

        GameLogic.instance.chatHandler.AddLocalMessage(netPlayer.playerName + " has connected.");

        // Check for dev auto-start
        if (DevQuickStart.AutoStart())
            if (GameLogic.instance.GetNetPlayerList().Count >= DevQuickStart.AutoStartCount())
            {
                GameLogic.instance.chatHandler.AddLocalMessage("Auto-Starting.");
                BoltNetwork.LoadScene("TestMap");
            }
    }

    public override void OnEvent(evJoinTeam _ev)
    {
        // Get LobbySlot from slotID
        LobbySlot fromSlot = LobbyHandler.instance.GetLobbySlotBySlotID((byte)_ev.slotID);

        // Try to move to the selected team and return outcome
        LobbySlot toSlot = LobbyHandler.instance.TryMoveTeam(fromSlot, _ev.teamRed);

        // If the new slot was available, broadcast the move to all NetPlayers (including self)
        if (toSlot != null)
        {
            foreach (NetPlayer np in GameLogic.instance.GetNetPlayerList())
            {
                evJoinTeam jt = evJoinTeam.Create(Bolt.GlobalTargets.AllClients);
                jt.slotID = _ev.slotID;
                jt.teamRed = _ev.teamRed;
                jt.Send();
            }
        }
    }

    public override void OnEvent(evReadyUp _ev)
    {
        // Set ready status of NetPlayer
        NetPlayer netPlayer = NetPlayer.GetNetPlayer((uint)_ev.playerID);
        netPlayer.SetReadyStatus(_ev.readyStatus);

        // Create ready up event and send to all NetPlayers (including self)
        evReadyUp readyUp = evReadyUp.Create(Bolt.GlobalTargets.AllClients);
        readyUp.readyStatus = _ev.readyStatus;
        readyUp.playerID = (int)_ev.playerID;
        readyUp.Send();

        // Check if game should start
        int readyCount = 0;
        foreach (NetPlayer p in GameLogic.instance.GetNetPlayerList())
            if (p.isReady)
                readyCount++;

        if (readyCount == GameLogic.instance.GetNetPlayerList().Count)
            BoltNetwork.LoadScene("TestMap"); // Everyone is ready, load map
    }

    public override void OnEvent(evChatMessage _ev)
    {
        GameLogic.instance.chatHandler.AddChatMessage(_ev.timeStamp + _ev.senderName + _ev.message);
    }
}
