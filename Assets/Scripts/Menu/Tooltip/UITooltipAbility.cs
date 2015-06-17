using UnityEngine;
using System.Collections;

public class UITooltipAbility : MonoBehaviour
{
    public UILabel labelTitle;
    public UILabel labelType;
    public UILabel labelText;
    public UILabel labelCooldown;
    public UILabel labelCasttime;
    public UILabel labelVelocity;
    public UILabel labelAoE;

    public string titleColor;
    public string typeColor;
    public string textColor;
    public string statNameColor;
    public string valueColor;

    public void SetTitle(string _text)
    {
        labelTitle.text = "[" + titleColor + "]" + _text + "[-]";
    }

    public void SetType(string _text)
    {
        labelType.text = "[" + typeColor + "]" + _text + "[-]";
    }

    public void SetText(string _text)
    {
        labelText.text = "[" + textColor + "]" + _text.Replace("\\n", "\n") + "[-]";
    }

    // Stats
    public void SetCooldown(string _text)
    {
        labelCooldown.text = "[" + statNameColor + "]Cooldown: [-][" + valueColor + "]" + _text + "s[-]";
    }

    public void SetCasttime(string _text)
    {
        labelCasttime.text = "[" + statNameColor + "]Cast Time: [-][" + valueColor + "]" + _text + "s[-]";
    }

    public void SetVelocity(string _text)
    {
        labelVelocity.text = "[" + statNameColor + "]Velocity: [-][" + valueColor + "]" + _text + "[-]";
    }

    public void SetAoE(string _text)
    {
        labelAoE.text = "[" + statNameColor + "]Area of Effect: [-][" + valueColor + "]" + _text + "[-]";
    }

    public void ResetValues()
    {
        labelTitle.text = "";
        labelType.text = "";
        labelText.text = "";
        labelCooldown.text = "";
        labelCasttime.text = "";
        labelVelocity.text = "";
        labelAoE.text = "";
    }
}
