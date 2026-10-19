using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        GameManager.Instance.SwitchToMenu();
    }

    public void SetCompletedLevel()
    {
        GameManager.Instance.SetCompletedLevels();
    }
}
