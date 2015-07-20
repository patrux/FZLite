using UnityEngine;
using System.Collections;

/// <summary>
/// String look-up for names and colors.
/// </summary>
public static class FZLang 
{
    /// <summary>
    /// Names
    /// </summary>
    public static string lobby_EmptySlotName = "-";

    /// <summary>
    /// Colors
    /// </summary>
    public static string chat_ColorConsole = "[F1A242]";

    public static string chat_ColorText = "[FFFFFF]";

    public static string chat_ColorLocalName = "[FB845C]";
    public static string chat_ColorLocalTimestamp = "[FB845C]";

    public static string chat_ColorRemoteName = "[FCCE8E]";
    public static string chat_ColorRemoteTimestamp = "[FCCE8E]";

    public static string chat_ColorServerName = "[F14266]";
    public static string chat_ColorServerTimestamp = "[F14266]";

    public static string[] GameUI_AbilityBar_keybindNames = new string[] {"M1", "M2", "Spc", "Q", "E", "R", "F"};
}
