using UnityEngine;
using System.Collections;

public class UIButtonCustomText : MonoBehaviour
{
    public UILabel label;

    Vector2 floatingDistance;

    [Range(0f, 100f)]
    public float baseDistance = 4f;

    [Range(0f, 100f)]
    public float cycleSpeed = 8f;

    [Range(0f, 100f)]
    public float cycleDistance = 0.5f;

    private float rotation = Mathf.PI;

    bool isHovering = false;
    bool isPressing = false;

    void Update()
    {
        if (isPressing)
        {
            label.effectDistance = new Vector2(baseDistance + (cycleDistance * 2f), baseDistance + (cycleDistance * 2f));
        }
        else
        {
            if (isHovering)
            {
                rotation += cycleSpeed * Time.deltaTime;

                label.effectDistance = new Vector2(baseDistance + Mathf.Sin(rotation) * cycleDistance, baseDistance + Mathf.Sin(rotation) * cycleDistance);
            }
            else
            {
                rotation = Mathf.PI;
                label.effectDistance = new Vector2(1f, 1f);
            }
        }
    }

    void OnHover(bool _isOver)
    {
        isHovering = _isOver;
    }

    void OnPress(bool _isPressing)
    {
        isPressing = _isPressing;
    }

    void OnEnable()
    {
        isPressing = false;
        isHovering = false;
    }
}
