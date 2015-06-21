using UnityEngine;
using System.Collections;

public class UIScreenServer : MonoBehaviour, IMenuScreen
{
    public GameObject closeWindowButton;

    public void Show() 
    {
        GameLogic.instance.gameState = GameLogic.GameState.LOBBY;
        closeWindowButton.SetActive(false);
        gameObject.SetActive(true);
    }

    public void Hide() 
    {
        GameLogic.instance.gameState = GameLogic.GameState.UNCONNECTED;
        closeWindowButton.SetActive(true);
        gameObject.SetActive(false);
    }
}
