using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour("TestMap")]
public class TestMapLogic : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string map)
    {
        if (BoltNetwork.isServer)
        {
            // Create observer for the server
            GameObject go = (GameObject)Instantiate(Resources.Load("Debug/DebugPlayer"), Vector3.zero, Quaternion.identity);
        }
        else
        {
            // Switch to GAME UI
            GameLogic.instance.menuHandler.SetScreenGameUI();
        }
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        if (BoltNetwork.isServer)
        {
            // Instantiate a BoltEntity character for each player
            NetPlayer p = NetPlayer.GetNetPlayer(connection);

            BoltEntity be = BoltNetwork.Instantiate(BoltPrefabs.Player, GetStartingPosition(p), Quaternion.identity);
            be.gameObject.name = "Player(" + p.playerName + ")";

            ControlToken ct = new ControlToken();
            ct.playerID = p.playerID;

            be.AssignControl(connection, ct);
        }
    }

    Vector3 GetStartingPosition(NetPlayer _player)
    {
        return new Vector3((_player.IsTeamRed() ? -3 : 3), 0f, (_player.slotID * 3));
    }
}
