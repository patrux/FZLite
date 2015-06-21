using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChatHandler : MonoBehaviour
{
    public static ChatHandler instance;
    void Awake() { instance = this; }
    void OnDestroy() { instance = null; }

    /// <summary>
    /// Chat text stored between chat windows.
    /// </summary>
    // List<UITextList.Paragraph> textList = new List<UITextList.Paragraph>();

    /// <summary>
    /// Handles chat input history.
    /// </summary>
    ChatHistory chatHistory;

    /// <summary>
    /// Handles commands.
    /// </summary>
    HandleChatCommand handleChatCommand;

    /// <summary>
    /// The active chat window.
    /// </summary>
    ChatSource.ChatSourceInfo activeChat;

    /// <summary>
    /// If the chat has focus, read keys like Up/Down arrow for history.
    /// Note: Unused for now, implement for ingame chat.
    /// </summary>
    bool hasFocus = true;

    void Start()
    {
        chatHistory = new ChatHistory(this);
        handleChatCommand = new HandleChatCommand();
        GameLogic.instance.chatHandler = this;
    }

    public void RegisterChatSource(ChatSource.ChatSourceInfo _csi)
    {
        // TODO: copy textList between chat windows when switching
        activeChat = _csi;
        chatHistory.RegisterChatInput(activeChat.input);
        activeChat.scrollBar.value = 1f;
    }

    /// <summary>
    /// Clears the chat of any input.
    /// </summary>
    /// <param name="_clearHistory">If true, it will clear ChatHistory aswell.</param>
    public void ClearChat(bool _clearHistory)
    {
        activeChat.textList.Clear();
        activeChat.scrollBar.value = 1f;

        if (_clearHistory)
            chatHistory.ClearHistory();
    }

    /// <summary>
    /// Called when a ChatSource has a new chat submission.
    /// </summary>
    /// <param name="_text"></param>
    public void OnSubmitChat(string _text)
    {
        string input = _text.Trim();

        if (input.Length <= 0)
            return;

        if (activeChat.textList != null)
        {
            // Check how to handle the input text
            if (handleChatCommand.CheckCommand(input))
            {
                // The string started with "/" so treat it as a command
                handleChatCommand.HandleCommand(input.Substring(1, input.Length - 1));

                chatHistory.AddMessage(input);
                activeChat.scrollBar.value = 1f;
            }
            else
            {
                // Treat it as a chat message
                string text = NGUIText.StripSymbols(input);

                if (!string.IsNullOrEmpty(text))
                {
                    // Create the message
                    DateTime dateTime = System.DateTime.Now;
                    string message;
                    string remoteTimestamp;
                    string remoteName;

                    text = FZLang.chat_ColorText + text + "[-]";

                    if (BoltNetwork.isClient)
                    {
                        string localTimestamp = FZLang.chat_ColorLocalTimestamp + "[" + dateTime.Hour + ":" + dateTime.Minute + "][-] ";
                        string localName = FZLang.chat_ColorLocalName + "<" + PlayerSettings.GetPlayerName() + ">[-] ";

                        remoteTimestamp = FZLang.chat_ColorRemoteTimestamp + "[" + dateTime.Hour + ":" + dateTime.Minute + "][-] ";
                        remoteName = FZLang.chat_ColorRemoteName + "<" + PlayerSettings.GetPlayerName() + ">[-] ";

                        message = localTimestamp + localName + text;
                    }
                    else
                    {
                        string localTimestamp = FZLang.chat_ColorServerTimestamp + "[" + dateTime.Hour + ":" + dateTime.Minute + "][-] ";
                        string localName = FZLang.chat_ColorServerName + "<" + PlayerSettings.GetPlayerName() + ">[-] ";

                        remoteTimestamp = FZLang.chat_ColorServerTimestamp + "[" + dateTime.Hour + ":" + dateTime.Minute + "][-] ";
                        remoteName = FZLang.chat_ColorServerName + "<" + PlayerSettings.GetPlayerName() + ">[-] ";

                        message = localTimestamp + localName + text;
                    }

                    // Add the message locally
                    chatHistory.AddMessage(input);
                    AddChatMessage(message);

                    // Send the message to server, which will forward it to all other players
                    evChatMessage ev = evChatMessage.Create(Bolt.GlobalTargets.Others);
                    ev.timeStamp = remoteTimestamp;
                    ev.senderName = remoteName;
                    ev.message = text;
                    ev.Send();
                }
            }
            activeChat.input.value = "";
        }
    }

    /// <summary>
    /// Adds a message in the chat for the local player only.
    /// </summary>
    public void AddLocalMessage(string _text)
    {
        string message = FZLang.chat_ColorConsole + "<Console>[-] " + _text;
        AddChatMessage(message);
    }

    /// <summary>
    /// Add the chat message to the list.
    /// </summary>
    public void AddChatMessage(string _message)
    {
        if (activeChat.textList != null)
        {
            activeChat.textList.Add(_message);
            activeChat.scrollBar.value = 1f;
        }
    }
}
