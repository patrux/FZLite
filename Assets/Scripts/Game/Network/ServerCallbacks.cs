using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Host)]
public class ServerCallbacks : Bolt.GlobalEventListener 
{
    //public override void Connected(BoltConnection connection)
    //{
    //    ConnectToken sct = (ConnectToken)connection.AcceptToken;
    //    Debug.Log("[Connected] player[" + sct.playerName + "]");
    //}

    //public override void Disconnected(BoltConnection connection)
    //{
    //}
}
