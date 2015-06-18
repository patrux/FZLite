using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Linq;

public class PlayerSettings : MonoBehaviour
{
    static public PlayerSettings instance;
    void Awake() { instance = this; }
    void OnDestroy() { instance = null; }

    // Class for each value field
    class SettingsValue
    {
        public SettingsValue(string _field, string _value, string _defaultValue) { field = _field; value = _value; defaultValue = _defaultValue; }
        string field; // The value name to look-up
        string value; // The current value
        string defaultValue; // The default value

        public string GetField() { return field; }
        public string GetValue() { return value; }
        public string GetDefaultValue() { return defaultValue; }
        public void LoadValue(XDocument _xdoc)
        {
            XElement xe = _xdoc.Root.Element(field);

            if (xe != null)
            {
                string v = xe.Value;

                if (v != null)
                    SetValue(v);
                else
                    SetDefaultValue();
            }
            else
                SetDefaultValue();
        }
        public void SaveValue(XDocument _xdoc)
        {
            _xdoc.Root.Element(field).SetValue(value);
        }
        public void SetValue(string _value) { value = _value; }
        public void SetDefaultValue() { value = defaultValue; }
    }

    // General settings
    SettingsValue playerName = new SettingsValue("name", "player", "player");
    SettingsValue sensitivity = new SettingsValue("sensitivity", "1.0", "1.0");
    SettingsValue ip = new SettingsValue("ip", "127.0.0.1", "127.0.0.1");
    SettingsValue port = new SettingsValue("port", "1337", "1337");

    // Keybinds
    string keyBindPrefix = "keybind_";
    public static KeyCode keyMoveUp;
    public static KeyCode keyMoveDown;
    public static KeyCode keyMoveLeft;
    public static KeyCode keyMoveRight;
    public static KeyCode keyAttack1;
    public static KeyCode keyAttack2;
    public static KeyCode keyCancelCast;

    // Unchangeable keys
    public static KeyCode keyEscape = KeyCode.Escape;
    public static KeyCode keyReloadSettings = KeyCode.F9;

    // Dev hotkeys
    public static KeyCode devKeyQuickStart = KeyCode.F5;

    void Start()
    {
        LoadSettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(keyReloadSettings))
        {
            LoadSettings();
            Debug.Log("[Settings Loaded]");
        }
    }

    /// <summary>
    /// Loads the default values.
    /// </summary>
    void DefaultValues()
    {
        // General settings
        playerName.SetDefaultValue();
        sensitivity.SetDefaultValue();
        ip.SetDefaultValue();
        port.SetDefaultValue();

        // Keybinds
        keyMoveUp = KeyCode.W;
        keyMoveDown = KeyCode.S;
        keyMoveLeft = KeyCode.A;
        keyMoveRight = KeyCode.D;
        keyAttack1 = KeyCode.Mouse0;
        keyAttack2 = KeyCode.Mouse1;
        keyCancelCast = KeyCode.LeftControl;
    }

    /// <summary>
    /// Reloads (or loads) the latest settings from the file.
    /// </summary>
    public void LoadSettings()
    {
        string path = Application.dataPath + "/StreamingAssets/PlayerSettings.xml";
        XDocument xdoc = XDocument.Load(path);

        // General settings
        playerName.LoadValue(xdoc);
        sensitivity.LoadValue(xdoc);
        ip.LoadValue(xdoc);
        port.LoadValue(xdoc);

        // Keybinds
        keyMoveUp = GetKeybind("moveup", xdoc);
        keyMoveDown = GetKeybind("movedown", xdoc);
        keyMoveLeft = GetKeybind("moveleft", xdoc);
        keyMoveRight = GetKeybind("moveright", xdoc);
        keyAttack1 = GetKeybind("attack1", xdoc);
        keyAttack2 = GetKeybind("attack2", xdoc);
        keyCancelCast = GetKeybind("cancelcast", xdoc);
    }

    /// <summary>
    /// Reads XML value as string and converts it to KeyCode.
    /// </summary>
    KeyCode GetKeybind(string _keyName, XDocument _xdoc)
    {
        return (KeyCode)System.Enum.Parse(
            typeof(KeyCode),
            _xdoc.Root.Element(keyBindPrefix + _keyName).Value);
    }

    /// <summary>
    /// Front-end save settings.
    /// </summary>
    static public void SaveSettingsToFile()
    {
        if (instance != null)
            instance.SaveSettingsToFileAux();
    }

    /// <summary>
    /// Saves the loaded data to the file.
    /// </summary>
    void SaveSettingsToFileAux()
    {
        string path = Application.dataPath + "/StreamingAssets/PlayerSettings.xml";
        XDocument xdoc = XDocument.Load(path);

        playerName.SaveValue(xdoc);
        sensitivity.SaveValue(xdoc);
        ip.SaveValue(xdoc);
        port.SaveValue(xdoc);

        xdoc.Save(path);
    }

    // -------------- GET

    /// <summary>
    /// Player
    /// </summary>
    static public void SetPlayerName(string _value)
    {
        if (instance != null)
            instance.playerName.SetValue(_value);
    }

    static public string GetPlayerName()
    {
        if (instance != null)
            return instance.playerName.GetValue();
        else
            return "NULL";
    }

    /// <summary>
    /// Sensitivity
    /// </summary>
    static public float GetSensitivity()
    {
        if (instance != null)
        {
            float value;
            bool success = float.TryParse(instance.sensitivity.GetValue(), out value);

            if (!success)
                value = float.Parse(instance.sensitivity.GetDefaultValue());

            return value;
        }
        else
            return 1.0f;
    }

    /// <summary>
    /// IP
    /// </summary>
    static public void SetIP(string _value)
    {
        if (instance != null)
            instance.ip.SetValue(_value);
    }

    static public string GetIP()
    {
        if (instance != null)
            return instance.ip.GetValue();
        else
            return "NULL";
    }

    /// <summary>
    /// Port
    /// </summary>
    static public void SetPort(int _value)
    {
        if (instance != null)
            instance.port.SetValue("" + _value);
    }

    /// <summary>
    /// This may return an error if the port couldn't be parsed.
    /// </summary>
    static public int GetPort()
    {
        if (instance != null)
                return int.Parse(instance.port.GetValue());
        else
            return 0;
    }
}