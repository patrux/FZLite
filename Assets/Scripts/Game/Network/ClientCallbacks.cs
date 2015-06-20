using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Client)]
public class ClientCallbacks : Bolt.GlobalEventListener
{
    public override void Connected(BoltConnection connection)
    {
        // This local NetPlayer received a ConnectTokenResponse
        // from the server with the correct slotID and playerID.
        ConnectToken ctr = (ConnectToken)connection.AcceptToken;

        // Update info and enter lobby
        NetPlayer localNetPlayer = GameLogic.instance.localNetPlayer;
        localNetPlayer.slotID = ctr.slotID;
        //localNetPlayer.playerID = ctr.playerID; set later?

        GameLogic.instance.menuHandler.EnterLobby();
        LobbyHandler.instance.SetLobbySlot(localNetPlayer.slotID, localNetPlayer);
    }

    /// <summary>
    /// Recieved data about a new NetPlayer.
    /// </summary>
    public override void OnEvent(evNewNetPlayer _ev)
    {
        NetPlayer netPlayer = NetPlayer.CreateFromNewNetPlayerEvent(_ev);

        if (netPlayer != null)
        {
            GameLogic.instance.GetNetPlayerList().Add(netPlayer);
            LobbyHandler.instance.SetLobbySlot(netPlayer.slotID, netPlayer);
            Debug.Log("[NewPlayer] Created and added: " + netPlayer.ToString());
        }
    }
}
