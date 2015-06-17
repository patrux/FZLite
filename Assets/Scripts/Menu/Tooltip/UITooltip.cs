using UnityEngine;
using System.Collections;

public class UITooltip : MonoBehaviour
{
    static protected UITooltip instance;

    public Camera uiCamera;
    public UISprite background;

    protected UISprite hoverSprite;
    protected Transform selfTransform;
    protected Vector3 selfPosition;
    protected Vector3 selfSize = Vector3.zero;

    protected UIWidget[] childWidgets;
    protected bool isVisible = false;

    protected TooltipType tooltipType;

    public UITooltipTextbox ttTextbox;
    public UITooltipAbility ttAbility;

    public int baseHeight;

    // Temporary solution to get the NGUI width of the screen (set to a sprite encompassing the entire screen).
    public UISprite screenSprite;

    public enum TooltipType
    {
        Ability,
        Textbox
    }

    /// <summary>
    /// Whether the tooltip is currently visible.
    /// </summary>
    static public bool IsVisible { get { return (instance != null && instance.isVisible); } }

    void Awake() { instance = this; }
    void OnDestroy() { instance = null; }

    /// <summary>
    /// Get a list of widgets underneath the tooltip.
    /// </summary>
    protected virtual void Start()
    {
        selfTransform = transform;
        childWidgets = GetComponentsInChildren<UIWidget>();
        selfPosition = selfTransform.localPosition;
        if (uiCamera == null)
            uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
        SetAlpha(0f);
        ttTextbox.gameObject.SetActive(false);
        ttAbility.gameObject.SetActive(false);
    }

    /// <summary>
    /// Set the alpha of all widgets.
    /// </summary>
    protected virtual void SetAlpha(float val)
    {
        for (int i = 0, imax = childWidgets.Length; i < imax; ++i)
        {
            UIWidget w = childWidgets[i];
            Color c = w.color;
            c.a = val;
            w.color = c;
        }
    }

    /// <summary>
    /// Set the tooltip's text to the specified string.
    /// </summary>
    protected virtual void ShowAux(int _stringID, UISprite _sprite, TooltipType _type, int _offsetHeight)
    {
        UILabel mainText;
        tooltipType = _type;
        hoverSprite = _sprite;

        if (_type == TooltipType.Textbox)
        {
            ttTextbox.gameObject.SetActive(true);
            UITooltipXmlHandler.ReadTooltipTextbox(_stringID, ttTextbox);

            // Get the bulk text to determine window scaling
            mainText = ttTextbox.labelText;
        }
        else if (_type == TooltipType.Ability)
        {
            ttAbility.gameObject.SetActive(true);
            UITooltipXmlHandler.ReadTooltipAbility(_stringID, ttAbility);

            // Get the bulk text to determine window scaling
            mainText = ttAbility.labelText;
        }
        else
        {
            Debug.Log("[UITooltip::ShowAux] Unknown type.");
            return;
        }

        int height = Mathf.RoundToInt(baseHeight + _offsetHeight);

        // Make sure the height is divisible by 2 to get crisp font
        background.height = (height % 2 == 0) ? height : height + 1;

        // Offset anchor of target sprite, top left corner
        Vector3 spriteOffset = new Vector3(
            hoverSprite.transform.localPosition.x - (hoverSprite.width / 2f),
            hoverSprite.transform.localPosition.y + (hoverSprite.height / 2f) - (hoverSprite.border.y / 2f),
            0f);

        // Offset anchor of own background, bottom left corner
        Vector3 selfOffset = new Vector3(
            (background.width / 2f) + background.border.x,
            background.height / 2f - background.border.y,
            0f);

        // New position with offsets
        selfPosition = spriteOffset + selfOffset;

        // Check and adjust for the tooltip sticking out to the right of screen
        float lhsX = spriteOffset.x + background.width + background.border.x;
        float rhsX = screenSprite.width / 2f;

        if (lhsX > rhsX)
        {
            // Move the tooltip to the left by the amount its sticking outside the screen
            float offsetX = (lhsX - rhsX);
            selfPosition.x -= offsetX;
        }

        // Check and adjust for the tooltip sticking out to the top of screen
        float lhsY = spriteOffset.y + background.height + background.border.y;
        float rhsY = screenSprite.height / 2f;

        if (lhsY > rhsY)
        {
            // Move the tooltip to the left by the amount its sticking outside the screen
            float offsetY = (lhsY - rhsY);
            selfPosition.y -= offsetY;
        }

        // Set new position
        if (uiCamera != null)
        {
            selfPosition.x = Mathf.Round(selfPosition.x);
            selfPosition.y = Mathf.Round(selfPosition.y);
            selfTransform.localPosition = selfPosition;
        }

        SetAlpha(1f);
    }

    /// <summary>
    /// Set the tooltip's text to the specified string.
    /// </summary>
    protected virtual void HideAux()
    {
        SetAlpha(0f);

        if (tooltipType == TooltipType.Textbox)
        {
            ttTextbox.ResetValues();
            ttTextbox.gameObject.SetActive(false);
        }
        else if (tooltipType == TooltipType.Ability)
        {
            ttAbility.ResetValues();
            ttAbility.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("[UITooltip::HideAux] Unknown type.");
            return;
        }
    }

    /// <summary>
    /// Show the tooltip.
    /// </summary>
    static public void Show(int _stringID, UISprite _sprite, TooltipType _type, int _offsetHeight)
    {
        if (instance != null)
        {
            instance.ShowAux(_stringID, _sprite, _type, _offsetHeight);
            instance.isVisible = true;
        }
    }

    /// <summary>
    /// Hide the tooltip.
    /// </summary>
    static public void Hide()
    {
        if (instance != null)
        {
            instance.HideAux();
            instance.hoverSprite = null;
            instance.isVisible = false;
        }
    }
}
