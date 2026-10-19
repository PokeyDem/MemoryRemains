using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button calibrationButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button creditsButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(() => { GameManager.Instance.SwitchToGameplay();});
        calibrationButton.onClick.AddListener(() => { GameManager.Instance.SwitchToCalibration();});
        tutorialButton.onClick.AddListener(() => { GameManager.Instance.SwitchToTutorial();});
        creditsButton.onClick.AddListener(() => { GameManager.Instance.SwitchToCredits();});
        exitButton.onClick.AddListener(ExitGame);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
