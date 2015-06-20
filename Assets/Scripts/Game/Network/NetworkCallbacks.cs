using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{
    public override void OnEvent(evReadyUp _ev)
    {
        UIScreenLobby lobbyScreen = GameObject.Find("LobbyScreen").GetComponent<UIScreenLobby>();
        //lobbyScreen.SetLobbyReadyStatus(!BoltNetwork.isServer, _ev.readyStatus);
        // todo: if both are ready, trigger stargame
    }

    /// <summary>
    /// This starts the countdown for everybody.
    /// </summary>
    public override void OnEvent(evCountdownStart _ev)
    {
        //UIMenuHandler.instance.screenLobby.BeginCountdown();
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        if (BoltNetwork.isServer)
        {
            Vector3 startPosition = new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4));
            BoltEntity be = BoltNetwork.Instantiate(BoltPrefabs.Player, startPosition, Quaternion.identity);
            ConnectToken ct = (ConnectToken)connection.ConnectToken;
            //be.name = "Player(" + ct.playerName + ")";

            be.AssignControl(connection);
        }
    }

    /// <summary>
    /// Map has loaded for the server player.
    /// </summary>
    public override void SceneLoadLocalDone(string map)
    {
        if (BoltNetwork.isServer)
        {
            Vector3 startPosition = new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4));
            BoltEntity be = BoltNetwork.Instantiate(BoltPrefabs.Player, startPosition, Quaternion.identity);
            //be.name = "Player(" + PlayerSettings.GetPlayerName() + ")";

            be.TakeControl();
        }
    }
}
