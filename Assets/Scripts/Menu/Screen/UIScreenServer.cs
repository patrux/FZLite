using UnityEngine;
using System.Collections;

public class UIScreenServer : MonoBehaviour, IMenuScreen
{
    public void Show() 
    {
        GameLogic.instance.gameState = GameLogic.GameState.LOBBY;
        GameObject.Find("UIPanel (MENU)").SetActive(false);
        gameObject.SetActive(true);
    }

    public void Hide() 
    {
        GameLogic.instance.gameState = GameLogic.GameState.UNCONNECTED;
        gameObject.SetActive(false);
        GameObject.Find("UIPanel (MENU)").SetActive(true);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
