using UnityEngine;
using System.Collections;

public class AbilityInfo
{
    private string name;
    private string description; // tooltip desc?
    private UISprite icon;

    public AbilityInfo(string _name, string _description)
    {
        name = _name;
        description = _description;
    }

    public AbilityInfo(string _name, string _description, UISprite _icon)
    {
        name = _name;
        description = _description;
        icon = _icon;
    }

    public string AbilityName
    {
        get { return name; }
    }

    public string AbilityDescription
    {
        get { return description; }
    }

    public UISprite AbilityIcon
    {
        get { return icon; }
    }
}
