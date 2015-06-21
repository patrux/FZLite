using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this class to chat sources to register them with the ChatHandler.
/// Note: attach to the parent, "Chat Window".
/// </summary>
public class ChatSource : MonoBehaviour
{
    /// <summary>
    /// Register this struct with the ChatHandler.
    /// </summary>
    ChatSourceInfo csi;

    public struct ChatSourceInfo
    {
        public UIInput input;
        public UITextList textList;
        public UIScrollBar scrollBar;
    }

    void Start()
    {
        csi.input = GameObject.Find("Chat Input/").GetComponent<UIInput>();
        csi.textList = GameObject.Find("Chat Area/").GetComponent<UITextList>();
        csi.scrollBar = GameObject.Find("Scroll Bar/").GetComponent<UIScrollBar>();

        csi.input.label.maxLineCount = 1;
        EventDelegate.Add(csi.input.onSubmit, OnSubmit);
        GameLogic.instance.chatHandler.RegisterChatSource(csi);
    }

    /// <summary>
    /// When the local player submits a new message to this source.
    /// </summary>
    public void OnSubmit()
    {
        GameLogic.instance.chatHandler.OnSubmitChat(csi.input.value);
    }
}
