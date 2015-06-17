using UnityEngine;
using System.Collections;

public class UITooltipInvoker : MonoBehaviour 
{
    /// <summary>
    /// The type of the tooltip. Textbox will show a heading + subtext while ability has more advanced logic.
    /// </summary>
    public UITooltip.TooltipType tooltipType;

    /// <summary>
    /// If the type is a textbox the tooltip will lookup the matching string for this ID. Otherwise the ability.
    /// </summary>
    public int stringID;

    /// <summary>
    /// The sprite dimensions that the tooltip will anchor to.
    /// </summary>
    public UISprite sprite;

    /// <summary>
    /// The added height to the tooltip.
    /// </summary>
    public int offsetHeight;

    /// <summary>
    /// Show tooltip when hovering over the collider.
    /// </summary>
    void OnHover(bool _isHovering)
    {
        if (_isHovering)
            UITooltip.Show(stringID, sprite, tooltipType, offsetHeight);
        else
            UITooltip.Hide();
    }
}
