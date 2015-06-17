using UnityEngine;
using System.Collections;

public class UITooltipTextbox : MonoBehaviour 
{
    public UILabel labelTitle;
    public UILabel labelText;

    public string titleColor;
    public string textColor;

    public void SetTitle(string _text)
    {
        labelTitle.text = "[" + titleColor + "]" + _text + "[-]";
    }

    public void SetText(string _text)
    {
        labelText.text = "[" + textColor + "]" + _text.Replace("\\n", "\n") + "[-]";
    }

    public void ResetValues()
    {
        labelTitle.text = "";
        labelText.text = "";
    }
}
