using UnityEngine;
using System.Collections;

public class AbilityBehaviors : MonoBehaviour
{
    private AbilityInfo abilityInfo;
    private BehaviorStartTimes startTime;

    // BehaviourStartTimes
    public enum BehaviorStartTimes
    {
        Beginning,
        Middle,
        End
    }

    public AbilityBehaviors(AbilityInfo _abilityInfo, BehaviorStartTimes _startTime)
    {
        abilityInfo = _abilityInfo;
        startTime = _startTime;
    }

    public AbilityBehaviors(BehaviorStartTimes _startTime)
    {
        startTime = _startTime;
    }

    // Call when you want to "extract" this behaviour
    public virtual void ExecuteBehavior(Vector3 _startPosition)
    {
        Debug.LogWarning("Missing behavior");
    }

    public AbilityInfo AbilityInfo
    {
        get { return abilityInfo; }
    }

    public BehaviorStartTimes AbilityBehaviorStartTime
    {
        get { return startTime; }
    }
}
