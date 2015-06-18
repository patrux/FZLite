using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class UIScreenPlay : MonoBehaviour, IMenuScreen
{
    NetworkHandler networkHandler;

    public UIInput inputIP;
    public UIInput inputPort;

    void Start()
    {
        networkHandler = GameObject.Find("GlobalScripts").GetComponent<NetworkHandler>();

        inputIP.value = PlayerSettings.GetIP();
        inputPort.value = "" + PlayerSettings.GetPort();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Parses string to int, could lead to crashes.
    /// </summary>
    public void Hide()
    {
        PlayerSettings.SetIP(inputIP.value);
        PlayerSettings.SetPort(int.Parse(inputPort.value));
        PlayerSettings.SaveSettingsToFile();
        gameObject.SetActive(false);
    }

    public void OnButton_Host()
    {
        if (CheckValidPort())
        {
            networkHandler.CreateServer(int.Parse(inputPort.value));
        }
        else
        {
            UIMenuConsole.WriteConsole("Invalid Port.");
        }
    }

    public void OnButton_Join()
    {
        if (CheckValidIP() && CheckValidPort())
        {
            networkHandler.JoinServer(inputIP.value, int.Parse(inputPort.value));
        }
        else
        {
            UIMenuConsole.WriteConsole("Invalid IP or Port.");
        }
    }

    bool CheckValidIP()
    {
        // Regex pattern matching standard IP adresses
        string regexPattern = @"\b(([01]?\d?\d|2[0-4]\d|25[0-5])\.){3}([01]?\d?\d|2[0-4]\d|25[0-5])\b";
        Match match = Regex.Match(inputIP.value, regexPattern);
        return match.Success;
    }

    bool CheckValidPort()
    {
        // Regex pattern matching standard Ports
        string regexPattern = @"^\s*-?[0-9]{2,5}\s*$";
        Match match = Regex.Match(inputPort.value, regexPattern);
        return match.Success;
    }
}
