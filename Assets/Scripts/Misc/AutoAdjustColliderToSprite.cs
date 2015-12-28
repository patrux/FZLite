using UnityEngine;
using System.Collections;

public class AutoAdjustColliderToSprite : MonoBehaviour 
{
	void Awake () 
	{
        BoxCollider collider = GetComponent<BoxCollider>();
        UISprite sprite = gameObject.transform.Find("Sprite").GetComponent<UISprite>();
        collider.center = new Vector3(sprite.transform.localPosition.x, sprite.transform.localPosition.y, 0f);
        collider.size = new Vector3(sprite.localSize.x, sprite.localSize.y, 1f);
        Destroy(this);
    }
}
