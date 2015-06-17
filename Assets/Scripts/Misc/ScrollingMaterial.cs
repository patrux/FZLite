using UnityEngine;
using System.Collections;

public class ScrollingMaterial : MonoBehaviour
{
    Renderer renderer;

    public float scrollSpeedX;
    public float scrollSpeedY;

    // Use this for initialization
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float offsetX = scrollSpeedX * Time.time;
        float offsetY = scrollSpeedY * Time.time;

        renderer.material.mainTextureOffset = new Vector2(offsetX, -offsetY);
    }
}
