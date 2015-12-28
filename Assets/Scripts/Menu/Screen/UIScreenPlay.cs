using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class UIScreenPlay : MonoBehaviour, IMenuScreen
{
    NetworkHandler networkHandler;

    public UIInput inputIP;
    public UIInput inputPort;
    public UIInput inputHostPort;

    void Start()
    {
        networkHandler = GameObject.Find("GlobalScripts").GetComponent<NetworkHandler>();

        inputIP.value = PlayerSettings.GetIP();
        inputPort.value = "" + PlayerSettings.GetPort();
        inputHostPort.value = "" + PlayerSettings.GetHostPort();
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
        PlayerSettings.SaveSettingsToFile(); // save entered ip and port
        gameObject.SetActive(false);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void OnButton_Host()
    {
        if (BoltNetwork.isRunning)
            BoltLauncher.Shutdown();

        if (CheckValidPort(int.Parse(inputHostPort.value).ToString()))
            networkHandler.CreateServer(int.Parse(inputHostPort.value));
        else
            UIMenuConsole.WriteConsole("Invalid Host Port.");
    }

    public void OnButton_Join()
    {
        if (BoltNetwork.isRunning)
            BoltLauncher.Shutdown();

        if (CheckValidIP(inputIP.value) && CheckValidPort(int.Parse(inputPort.value).ToString()))
            networkHandler.JoinServer(inputIP.value, int.Parse(inputPort.value));
        else
            UIMenuConsole.WriteConsole("Invalid IP or Port.");
    }

    bool CheckValidIP(string _value)
    {
        // Regex pattern matching standard IP adresses
        string regexPattern = @"\b(([01]?\d?\d|2[0-4]\d|25[0-5])\.){3}([01]?\d?\d|2[0-4]\d|25[0-5])\b";
        Match match = Regex.Match(_value, regexPattern);
        return match.Success;
    }

    bool CheckValidPort(string _value)
    {
        // Regex pattern matching standard Ports
        string regexPattern = @"^\s*-?[0-9]{2,5}\s*$";
        Match match = Regex.Match(_value, regexPattern);
        return match.Success;
    }
}
