using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{
    public override void OnEvent(evReadyUp _ev)
    {
        UIScreenLobby lobbyScreen = GameObject.Find("LobbyScreen").GetComponent<UIScreenLobby>();
        lobbyScreen.SetLobbyReadyStatus(!BoltNetwork.isServer, _ev.readyStatus);
        // todo: if both are ready, trigger stargame
    }

    /// <summary>
    /// This starts the countdown for everybody.
    /// </summary>
    public override void OnEvent(evCountdownStart _ev)
    {
        UIMenuHandler.instance.screenLobby.BeginCountdown();
    }

}
