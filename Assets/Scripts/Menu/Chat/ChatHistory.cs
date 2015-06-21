using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChatHistory
{
    ChatHandler chatHandler;

    // Input History
    List<string> inputHistory = new List<string>();
    int historyCap = 15;
    int historyIndex = 0;

    UIInput activeInput;

    /// <summary>
    /// Initialization.
    /// </summary>
    public ChatHistory(ChatHandler _chatHandler)
    {
        chatHandler = _chatHandler;
    }

    /// <summary>
    /// The UIInput label to target.
    /// </summary>
    public void RegisterChatInput(UIInput _input) 
    {
        activeInput = _input;
        historyIndex = 0;
    }

    /// <summary>
    /// Check for Up/Down keys while the chat is
    /// active to move in the sent history.
    /// </summary>
    void Update() { CheckForKeys(); }

    /// <summary>
    /// Adds a text to history. Cap the size
    /// and resets the index once added.
    /// </summary>
    public void AddMessage(string _text)
    {
        if (inputHistory.Count >= historyCap)
            inputHistory.RemoveAt(0);

        inputHistory.Add(_text);
        historyIndex = 0;
    }

    /// <summary>
    /// Clears the ChatHistory.
    /// </summary>
    public void ClearHistory()
    {
        inputHistory.Clear();
        historyIndex = 0;
    }

    /// <summary>
    /// Allows UP/DOWN arrow presses while the input field is
    /// in focus to toggle between recent messages.
    /// </summary>
    void CheckForKeys()
    {
        // Check for keys if there is any history
        if (inputHistory.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (historyIndex >= inputHistory.Count)
                    historyIndex = 0;

                activeInput.value = inputHistory[historyIndex];
                historyIndex++;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                historyIndex--;
                if (historyIndex < 0)
                    historyIndex = inputHistory.Count - 1;
                activeInput.value = inputHistory[historyIndex];
            }
        }

        // Reset history index if the input field is cleared
        if (activeInput.value.Length <= 0)
            historyIndex = 0;
    }
}
