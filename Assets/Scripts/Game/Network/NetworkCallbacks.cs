using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{
    //public override void OnEvent(evSetPlayerName _ev)
    //{
        //UIScreenLobby lobbyScreen = GameObject.Find("LobbyScreen").GetComponent<UIScreenLobby>();
        //lobbyScreen.SetLobbyName(_ev.playerName);
    //}

    public override void OnEvent(evReadyUp _ev)
    {
        UIScreenLobby lobbyScreen = GameObject.Find("LobbyScreen").GetComponent<UIScreenLobby>();
        lobbyScreen.SetLobbyReadyStatus(!BoltNetwork.isServer, _ev.readyStatus);

        //if (BoltNetwork.isServer)
        //{
        //    foreach (BoltConnection c in BoltNetwork.connections)
        //    {
        //        ConnectToken ct = (ConnectToken)c.AcceptToken;

        //        Debug.Log("[event] player[" + ct.playerName + "] tokenReady[" + ct.readyStatus + "] eventReady[" + _ev.readyStatus + "]");

        //        if (ct.readyStatus && NetworkHandler.lobbyReady)
        //            Debug.Log("START GAME!!!");
        //    }
        //}
    }

    public override void OnEvent(evCountdownStart _ev)
    {
        //UIMenuHandler uiMenuHandler = GameObject.Find("MenuScripts").GetComponent<UIMenuHandler>();
        UIMenuHandler.instance.screenLobby.BeginCountdown();
    }

}
