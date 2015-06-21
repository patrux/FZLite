using UnityEngine;
using System.Collections;

public class ChatCommand
{
    string command;
    bool serverCanExecute;
    bool clientCanExecute;
    public delegate void ExecuteMethod();
    event ExecuteMethod executeMethod;

    /// <summary>
    /// Registers a new chat command.
    /// </summary>
    /// <param name="_command">The string which will trigger this command.</param>
    /// <param name="_serverCanExecute">If this method can be executed on the Server.</param>
    /// <param name="_clientCanExecute">If this method can be executed on the Client.</param>
    /// <param name="_executeMethod">The method to execute.</param>
    public ChatCommand(string _command, bool _serverCanExecute, bool _clientCanExecute, ExecuteMethod _executeMethod)
    {
        command = _command;
        serverCanExecute = _serverCanExecute;
        clientCanExecute = _clientCanExecute;
        executeMethod += _executeMethod;
    }

    /// <summary>
    /// Get the string command.
    /// </summary>
    public string GetCommand()
    {
        return command;
    }

    /// <summary>
    /// Check if string matches command.
    /// </summary>
    public bool Match(string _checkCommandMatch)
    {
        return (command == _checkCommandMatch);
    }

    /// <summary>
    /// Does the player have permission to execute this command.
    /// </summary>
    /// <returns></returns>
    public bool CanExecute()
    {
        if (serverCanExecute)
            if (BoltNetwork.isServer)
                return true;

        if (clientCanExecute)
            if (BoltNetwork.isClient)
                return true;

        return false;
    }

    /// <summary>
    /// Execute the command if permissions match.
    /// </summary>
    public void ExecuteCommand()
    {
        if (CanExecute())
            executeMethod();
    }
}
