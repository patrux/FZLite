using UnityEngine;
using System.Collections;
using System.Diagnostics;

[RequireComponent(typeof(SphereCollider))]
public class AreaOfEffect : AbilityBehaviors
{
    private const string behavoirName = "Area Of Effect";
    private const string behavoirDescription = "AoE";
    private const BehaviorStartTimes startTime = BehaviorStartTimes.End;

    private SphereCollider collider;
    private float radius;
    private float duration;
    private Stopwatch durationTimer = new Stopwatch();
    private float damage; // per tick
    private float tickDuration; // make a global value and sync ticks?

    public AreaOfEffect(float _radius, float _duration, float _damage, float _tickDuration)
        : base(new AbilityInfo(behavoirName, behavoirDescription), startTime)
    {
        radius = _radius;
        duration = _duration;
        damage = _damage;
        tickDuration = _tickDuration;

        collider = gameObject.GetComponent<SphereCollider>();
    }

    public override void ExecuteBehavior(Vector3 _startPosition)
    {
        collider.radius = radius;
        StartCoroutine(CheckAoE());
    }

    private IEnumerator CheckAoE()
    {
        durationTimer.Start();

        while (durationTimer.Elapsed.TotalSeconds <= duration)
        {
            // do damage
            yield return new WaitForSeconds(tickDuration);
        }

        durationTimer.Stop();
        durationTimer.Reset();
        yield return null;
    }

    private void OnTriggerEnter(Collider _other)
    {

    }
}
