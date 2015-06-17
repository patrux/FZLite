using UnityEngine;
using System.Collections;

public class DebugDisableButtons : MonoBehaviour 
{
    public UIButton[] buttonList;

	void Start () 
    {
        foreach (UIButton b in buttonList)
        {
            b.isEnabled = false;
        }
	}
}
