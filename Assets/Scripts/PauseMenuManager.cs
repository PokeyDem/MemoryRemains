using System;
using UnityEngine;


public class PauseMenuManager : MonoBehaviour{

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameplayManager gameplayManager;
        
    private bool _isPaused = false;

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            EscPressed();
        } 
    }
    
    public void EscPressed(){
        if (_isPaused)
            Resume();
        else{
            Pause();
        }
    }

    public void Pause(){
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        _isPaused = true;
    }

    public void Resume(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;
    }

    public void OnExitToMenuButtonClick(){
        Resume();
        gameplayManager.ReturnToMainMenu();
    }
}