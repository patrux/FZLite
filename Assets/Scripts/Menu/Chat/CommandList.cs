using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandList
{
    List<ChatCommand> commandList = new List<ChatCommand>();

    public CommandList()
    {
        // Initialize commands
        commandList.Add(new ChatCommand("help", true, true, Help));
        //commandList.Add(new ChatCommand("reload", true, true, ReloadPlayerSettings));
        commandList.Add(new ChatCommand("ping", true, true, CheckPing));
        commandList.Add(new ChatCommand("clients", true, false, CountClients));
        commandList.Add(new ChatCommand("ready", false, true, LobbyReadyUp));
        commandList.Add(new ChatCommand("lobbyslots", true, true, PrintLobbySlots));
        commandList.Add(new ChatCommand("netplayers", true, true, PrintNetPlayers));
        commandList.Add(new ChatCommand("start", true, false, LoadMap));
    }

    /// <summary>
    /// Takes a command, looks for it in the list, if found execute and then report back.
    /// </summary>
    /// <returns>If the command was executed or not.</returns>
    public bool ExecuteCommand(string[] _args)
    {
        // Look for command
        ChatCommand chatCommand = null;

        foreach (ChatCommand cc in commandList)
        {
            // Check for match
            if (cc.Match(_args[0]))
            {
                chatCommand = cc;
                break;
            }
        }

        // If there was a match, execute the command
        if (chatCommand != null)
        {
            chatCommand.ExecuteCommand();
            return true;
        }
        else
            return false;

    }

    /// <summary>
    /// Counts the number of connections.
    /// </summary>
    int CountConnections(System.Collections.Generic.IEnumerable<BoltConnection> _connections)
    {
        int i = 0;
        foreach (BoltConnection bc in _connections)
        {
            i++;
        }
        return i;
    }

    /// <summary>
    /// Writes text to chat as a local console message.
    /// </summary>
    public void WriteLine(string _text)
    {
        GameLogic.instance.chatHandler.AddLocalMessage(_text);
    }

    #region Commands
    /// <summary>
    /// Lists all the commands that are available to the player.
    /// </summary>
    void Help()
    {
        WriteLine("Available commands:");
        foreach (ChatCommand cc in commandList)
        {
            // Check if the player has permission to execute the command
            if (cc.CanExecute())
                WriteLine("/" + cc.GetCommand());
        }
    }

    /// <summary>
    /// Check ping of self, if server check all clients ping.
    /// </summary>
    void CheckPing()
    {
        if (BoltNetwork.isClient)
            WriteLine("Ping to server " + (int)(BoltNetwork.server.PingNetwork * 1000) + "ms.");
        else
        {
            if (CountConnections(BoltNetwork.clients) > 0)
            {
                WriteLine("--- Client Ping ---");
                foreach (BoltConnection bc in BoltNetwork.clients)
                {
                    ConnectToken ct = (ConnectToken)bc.ConnectToken;
                    WriteLine("Client[" + ct.playerName + "] Ping[" + (int)(bc.PingNetwork * 1000) + "ms]");
                }
            }
            else
                WriteLine("There are no clients connected.");
        }
    }

    /// <summary>
    /// Toggles the ready status of the local NetPlayer while in a lobby.
    /// </summary>
    void LobbyReadyUp()
    {
        if (GameLogic.instance.gameState == GameLogic.GameState.LOBBY)
        {
            // Get local NetPlayer and toggle ready status
            NetPlayer localPlayer = GameLogic.instance.localNetPlayer;
            localPlayer.isReady = localPlayer.isReady ? false : true;

            // Send local message
            if (localPlayer.isReady)
                GameLogic.instance.chatHandler.AddLocalMessage("You are ready.");
            else
                GameLogic.instance.chatHandler.AddLocalMessage("You are not ready.");

            // Create ready up event
            evReadyUp readyUp = evReadyUp.Create(Bolt.GlobalTargets.OnlyServer);
            readyUp.readyStatus = localPlayer.isReady;
            readyUp.playerID = (int)localPlayer.playerID;
            readyUp.Send();
        }
        else
            WriteLine("You are not in a lobby.");
    }

    /// <summary>
    /// Loads the selected map, bypassing ready status.
    /// </summary>
    void LoadMap()
    {
        string mapName = "TestMap";
        WriteLine("Loading '" + mapName + "' ...");
        BoltNetwork.LoadScene(mapName);
    }

    /// <summary>
    /// Lists all the LobbySlots.
    /// </summary>
    void PrintLobbySlots()
    {
        if (GameLogic.instance.gameState == GameLogic.GameState.LOBBY)
        {
            LobbyHandler.instance.PrintLobbySlots();
        }
        else
            WriteLine("You are not in a lobby.");
    }

    /// <summary>
    /// Lists all the NetPlayers.
    /// </summary>
    void PrintNetPlayers()
    {
        if (GameLogic.instance.gameState == GameLogic.GameState.LOBBY)
        {
            GameLogic.instance.PrintNetPlayers();
        }
        else
            WriteLine("You are not in a lobby.");
    }

    /// <summary>
    /// Reloads player settings.
    /// Note: doesn't update UI etc, just refreshes settings.
    /// </summary>
    void ReloadPlayerSettings()
    {
        PlayerSettings.instance.LoadSettings();
        WriteLine("Reloaded local settings.");
    }

    /// <summary>
    /// Count connected clients.
    /// </summary>
    void CountClients()
    {
        WriteLine("ClientCount[" + CountConnections(BoltNetwork.clients) + "]");
    }
    #endregion
}
