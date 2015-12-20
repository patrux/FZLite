using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// http://forum.unity3d.com/threads/skill-casting-method-and-secondary-effects.279190/#post-1844525

public class Ability
{
    private AbilityInfo abilityInfo;
    private List<AbilityBehaviors> behaviours;
    //private GameObject particleEffect; // assign on ability creation

    private bool requiresTarget; // barrier, tos, ns etc
    private bool canSelfCast;
    private float cooldown;
    private float castTime;
    private float energyCost;
    private AbilityType abilityType;

    public enum AbilityType
    {
        Spell,
        Melee,
        Buff,
        Debuff
    }

    public Ability(AbilityInfo _abilityInfo, List<AbilityBehaviors> _behaviors)
    {
        abilityInfo = _abilityInfo;
        behaviours = new List<AbilityBehaviors>();
        behaviours = _behaviors;
        cooldown = 0f;
        castTime = 1f;

        requiresTarget = false;
        canSelfCast = false;
    }

    public Ability(AbilityInfo _abilityInfo, List<AbilityBehaviors> _behaviors, bool _requiresTarget, bool _canSelfCast, float _cooldown, float _castTime, float _energyCost)
    {
        abilityInfo = _abilityInfo;
        behaviours = new List<AbilityBehaviors>();
        behaviours = _behaviors;
        cooldown = _cooldown;
        castTime = _castTime;
        energyCost = _energyCost;
        requiresTarget = _requiresTarget;
        canSelfCast = _canSelfCast;
    }

    public float AbilityCooldown
    {
        get { return cooldown; }
    }

    public float AbilityCastTime
    {
        get { return castTime; }
    }

    public float AbilityEnergyCost
    {
        get { return energyCost; }
    }

    public AbilityInfo AbilityInfo
    {
        get { return abilityInfo; }
    }

    public List<AbilityBehaviors> AbilityBehaviours
    {
        get { return behaviours; }
    }

    /// <summary>
    /// Call OnUse
    /// </summary>
    public void UseAbility()
    {

    }
}
