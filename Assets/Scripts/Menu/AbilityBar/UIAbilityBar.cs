using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIAbilityBar : MonoBehaviour
{
    // The ability prefab
    public GameObject abilityPrefab;

    // List of current abilities
    List<UIAbility> abilityIconList = new List<UIAbility>();

    // Abilities
    readonly int maxSlots = 7;

    // Test Autostart
    void Start()
    {
        Initialize(7);
    }

    /// <summary>
    /// Initializes this UI component.
    /// </summary>
    public void Initialize(int _targetSlots)
    {
        Reset();

        if (_targetSlots > maxSlots)
            _targetSlots = maxSlots;

        for (int i = 0; i < _targetSlots; i++)
        {
            UIAbility ability = InstantiateAbilityIcon(i, FZLang.GameUI_AbilityBar_keybindNames[i]);
            abilityIconList.Add(ability);
        }
    }

    /// <summary>
    /// Initializes an ability.
    /// </summary>
    UIAbility InstantiateAbilityIcon(int _slot, string _keybind)
    {
        // Instantiate from ability prefab
        GameObject go = (GameObject)Instantiate(abilityPrefab, Vector3.zero, Quaternion.identity);
        go.name = "Ability" + _slot + " (" + _keybind + ")";
        go.transform.parent = gameObject.transform;
        go.transform.localScale = Vector3.one;

        // Get ability component
        UIAbility ability = go.GetComponent<UIAbility>();

        // Set keybind label
        ability.labelKeybind.text = _keybind;

        string keybind;
        switch (_keybind)
        {
            default:
                keybind = _keybind;
                break;
            case "M1":
                keybind = "Mouse0";
                break;
            case "M2":
                keybind = "Mouse1";
                break;
            case "Spc":
                keybind = "Space";
                break;
        }

        ability.kpHighlight.Initialize(keybind);

        // Set tooltip
        //ability.tooltip.stringID = 0;
        //ability.tooltip.offsetHeight = 0;
        //ability.tooltip.sprite.spriteName = "";
        //ability.tooltip.tooltipType = UITooltip.TooltipType.Ability;

        // Set anchor
        UIAbility prevAbility = null;

        if (_slot > 0)
            prevAbility = abilityIconList[_slot - 1];

        SetAnchor(prevAbility, ability);

        return ability;
    }

    /// <summary>
    /// Sets the UI anchors.
    /// Using "Set(transform, side, width)".
    /// </summary>
    void SetAnchor(UIAbility _target, UIAbility _source)
    {
        if (_target == null)
        {
            // Attach the first icon to the AbilityBarAnchor
            GameObject target = GameObject.Find("AB_Anchor");
            _source.spriteBorder.leftAnchor.Set(target.transform, 0f, 0f);
            _source.spriteBorder.rightAnchor.Set(target.transform, 0f, _source.spriteBorder.width);
            _source.spriteBorder.bottomAnchor.Set(target.transform, 0f, 0f);
            _source.spriteBorder.topAnchor.Set(target.transform, 1f, 0f);
        }
        else
        {
            // Stack subsequent icons on top of the previous one in the list
            _source.spriteBorder.leftAnchor.Set(_target.spriteBorder.transform, 1f, 0f);
            _source.spriteBorder.rightAnchor.Set(_target.spriteBorder.transform, 1f, _source.spriteBorder.width);
            _source.spriteBorder.bottomAnchor.Set(_target.spriteBorder.transform, 0f, 0f);
            _source.spriteBorder.topAnchor.Set(_target.spriteBorder.transform, 1f, 0f);
        }

        _source.spriteBorder.UpdateAnchors();

        // Update BoxCollider to match the new anchor
        _source.collider.center = new Vector3(
            _source.spriteBorder.gameObject.transform.localPosition.x,
            _source.spriteBorder.gameObject.transform.localPosition.y,
            _source.spriteBorder.gameObject.transform.localPosition.z);
    }

    /// <summary>
    /// Clears the ability list.
    /// </summary>
    void Reset()
    {
        foreach (UIAbility ability in abilityIconList)
            Destroy(ability.gameObject);

        abilityIconList.Clear();
    }
}
