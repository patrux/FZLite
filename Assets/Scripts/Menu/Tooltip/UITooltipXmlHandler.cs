using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Linq;

// TODO, load the xml data into a dictionary of <stringID, dataHolder> for much quicker lookups

public class UITooltipXmlHandler : MonoBehaviour 
{
    static protected UITooltipXmlHandler instance;

    void Awake() { instance = this; }
    void OnDestroy() { instance = null; }

    /// <summary>
    /// Front-end reader for Textboxes.
    /// </summary>
    /// <param name="_stringID">The ID of the requested textbox in the xml file.</param>
    /// <returns>The string.</returns>
    static public void ReadTooltipTextbox(int _stringID, UITooltipTextbox _container)
    {
        if (instance != null)
            instance.XmlReadTooltipTextbox(_stringID, _container);
    }

    /// <summary>
    /// The actual read logic.
    /// </summary>
    protected virtual void XmlReadTooltipTextbox(int _stringID, UITooltipTextbox _container)
    {
        string path = "Assets/Resources/Xml/TTTextbox.xml";
        XDocument doc = XDocument.Load(path);

        foreach (var t in doc.Descendants("Item"))
        {
            if (t.Element("id").Value == _stringID.ToString())
            {
                _container.SetTitle(t.Element("heading").Value);
                _container.SetText(t.Element("text").Value);
            }
        }
    }

    /// <summary>
    /// Front-end reader for Abilities.
    /// </summary>
    /// <param name="_stringID">The ID of the requested ability in the xml file.</param>
    /// <returns></returns>
    static public void ReadTooltipAbility(int _stringID, UITooltipAbility _container)
    {
        if (instance != null)
            instance.XmlReadTooltipAbility(_stringID, _container);
    }

    /// <summary>
    /// The actual read logic.
    /// </summary>
    protected virtual void XmlReadTooltipAbility(int _stringID, UITooltipAbility _container)
    {
        string path = "Assets/Resources/Xml/TTAbility.xml";
        XDocument doc = XDocument.Load(path);

        foreach (var t in doc.Descendants("Item"))
        {
            if (t.Element("id").Value == _stringID.ToString())
            {
                _container.SetTitle(t.Element("heading").Value);
                _container.SetType(t.Element("type").Value);
                _container.SetText(t.Element("text").Value);
                _container.SetCooldown(t.Element("cooldown").Value);
                _container.SetCasttime(t.Element("casttime").Value);
                _container.SetVelocity(t.Element("velocity").Value);
                _container.SetAoE(t.Element("aoe").Value);
            }
        }
    }
}
