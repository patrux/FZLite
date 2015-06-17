using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(UIInput))]
public class UIChatInput : Bolt.GlobalEventListener
{
    public UITextList textList;
    public UIScrollBar scrollBar;
    UIInput uiInput;

    // Input History
    List<string> inputHistory = new List<string>();

    int historyCap = 15;
    int historyIndex = 0;

    /// <summary>
    /// Add some dummy text to the text list.
    /// </summary>
    void Start()
    {
        uiInput = GetComponent<UIInput>();
        uiInput.label.maxLineCount = 1;
    }

    void Update()
    {
        HandleChatHistory();
    }

    /// <summary>
    /// Clears the chat.
    /// </summary>
    /// <param name="_fullClear">Set to true to remove chat history aswell.</param>
    public void ClearChat(bool _fullClear)
    {
        textList.Clear();

        if (_fullClear)
        {
            inputHistory.Clear();
            historyIndex = 0;
        }

        scrollBar.value = 1f;
    }

    /// <summary>
    /// Allows UP/DOWN arrow presses while the input field is
    /// in focus to toggle between recent messages.
    /// </summary>
    void HandleChatHistory()
    {
        // Check for keys if there is any history
        if (inputHistory.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (historyIndex >= inputHistory.Count)
                    historyIndex = 0;

                uiInput.value = inputHistory[historyIndex];
                historyIndex++;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                historyIndex--;
                if (historyIndex < 0)
                    historyIndex = inputHistory.Count - 1;
                uiInput.value = inputHistory[historyIndex];
            }
        }

        // Reset history index if the input field is cleared
        if (uiInput.value.Length <= 0)
            historyIndex = 0;
    }

    /// <summary>
    /// Adds a text to history. Cap the size
    /// and resets the index once added.
    /// </summary>
    void AddToHistory(string _text)
    {
        if (inputHistory.Count >= historyCap)
            inputHistory.RemoveAt(0);

        inputHistory.Add(_text);
        historyIndex = 0;
    }

    /// <summary>
    /// Submit notification is sent by UIInput when 'enter' is pressed or iOS/Android keyboard finalizes input.
    /// </summary>
    public void OnSubmit()
    {
        string input = uiInput.value.Trim();

        if (input.Length <= 0)
            return;

        if (textList != null)
        {
            // Check how to handle the input text
            if (HandleChatCommand.IsAChatCommand(input))
            {
                // The string started with "/" so treat it as a command
                HandleChatCommand.instance.HandleCommand(input.Substring(1, uiInput.value.Length - 1));
                AddToHistory(input);
                scrollBar.value = 1f;
            }
            else
            {
                // Treat it as a chat message
                // It's a good idea to strip out all symbols as we don't want user input to alter colors, add new lines, etc
                string text = NGUIText.StripSymbols(input);

                if (!string.IsNullOrEmpty(text))
                {
                    // Create the message
                    DateTime dateTime = System.DateTime.Now;

                    string localTimestamp = "[FB845C][" + dateTime.Hour + ":" + dateTime.Minute + "][-] ";
                    string localName = "[FB845C]<" + PlayerSettings.GetPlayerName() + ">[-] ";

                    string remoteTimestamp = "[FCCE8E][" + dateTime.Hour + ":" + dateTime.Minute + "][-] ";
                    string remoteName = "[FCCE8E]<" + PlayerSettings.GetPlayerName() + ">[-] ";

                    string message = localTimestamp + localName + text;

                    // Add the message locally
                    AddChatMessage(message);
                    AddToHistory(input);

                    // Send the message to all other remote players
                    evChatMessage ev = evChatMessage.Create(Bolt.GlobalTargets.Others);
                    ev.timeStamp = remoteTimestamp;
                    ev.senderName = remoteName;
                    ev.message = text;
                    ev.Send();
                }
            }

            uiInput.value = "";
        }
    }

    /// <summary>
    /// Adds a message in the chat for the local player only.
    /// </summary>
    public void AddLocalMessage(string _text)
    {
        string message = "[FCCE8E]<Console>[-] " + _text;
        AddChatMessage(message);
    }

    /// <summary>
    /// Add the chat message to the list.
    /// </summary>
    public void AddChatMessage(string _message)
    {
        textList.Add(_message);
        scrollBar.value = 1f;
    }

    /// <summary>
    /// Recieved a remote chat message from another player.
    /// </summary>
    public override void OnEvent(evChatMessage _ev)
    {
        AddChatMessage(_ev.timeStamp + _ev.senderName + _ev.message);
    }
}
