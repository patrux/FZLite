using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandleChatCommand
{
    CommandList commandList;

    public HandleChatCommand()
    {
        commandList = new CommandList();
    }

    /// <summary>
    /// Handles a given command.
    /// </summary>
    public void HandleCommand(string _text)
    {
        // Split command into arguments
        string[] args = _text.Split(' ');

        // Execute the command, returns success status
        bool executeSuccess = commandList.ExecuteCommand(args);

        if (!executeSuccess)
            WriteLine("Unknown command [" + _text + "]");
    }


    /// <summary>
    /// Returns whether the string starts with "/" or not, if so
    /// it should be treated as a command instead of a chat message.
    /// </summary>
    public bool CheckCommand(string _text)
    {
        return _text.Substring(0, 1) == "/";
    }

    /// <summary>
    /// Writes text to chat as a local console message.
    /// </summary>
    public void WriteLine(string _text)
    {
        GameLogic.instance.chatHandler.AddLocalMessage(_text);
    }
}
