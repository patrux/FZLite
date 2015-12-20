using UnityEngine;
using System.Collections;
using XInputDotNetPure; // Required in C#

public class ControllerVibration : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GamePad.SetVibration(0, 0f, 10000f);
	}
}
