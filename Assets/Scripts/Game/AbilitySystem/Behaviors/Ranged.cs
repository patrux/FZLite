using UnityEngine;
using System.Collections;

public class Ranged : AbilityBehaviors
{
    private const string behavoirName = "Ranged";
    private const string behavoirDescription = "Projectile";
    private const BehaviorStartTimes startTime = BehaviorStartTimes.Beginning;
    //private const UISprite icon = Resources.Load();

    private float range;

    public Ranged(float _range)
        : base(new AbilityInfo(behavoirName, behavoirDescription), startTime)
    {
        range = _range;
    }

    public override void ExecuteBehavior(Vector3 _startPosition)
    {
        StartCoroutine(CheckDistance(_startPosition));
    }

    private IEnumerator CheckDistance(Vector3 _startPosition)
    {
        float distanceTravelled = Vector3.Distance(_startPosition, gameObject.transform.position);

        while (distanceTravelled < range)
        {
            distanceTravelled = Vector3.Distance(_startPosition, gameObject.transform.position);
        }

        gameObject.SetActive(false); // Destroy or pool object
        yield return null;
    }

    public float Range
    {
        get { return range; }
    }
}
