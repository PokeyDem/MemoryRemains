using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName;
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private string calibrationSceneName;
    [SerializeField] private string tutorialSceneName;
    [SerializeField] private string creditsSceneName;
    
    [SerializeField] private int difficultyLevel;
    [SerializeField] private int completedLevels;

    [SerializeField] private float calibratedFrequencyCap = 10f;
    public static GameManager Instance;

    private float _minUserFreq = 80f;
    private float _maxUserFreq = 300f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchToGameplay()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void SwitchToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void SwitchToCalibration()
    {
        SceneManager.LoadScene(calibrationSceneName);
    }

    public void SwitchToTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void SwitchToCredits()
    {
        SceneManager.LoadScene(creditsSceneName);   
    }

    public void SetMinFreq(float freq)
    {
        _minUserFreq = freq - calibratedFrequencyCap;
    }

    public void SetMaxFreq(float freq)
    {
        _maxUserFreq = freq + calibratedFrequencyCap;
    }

    public float GetMinFreq()
    {
        return _minUserFreq;
    }

    public float GetMaxFreq()
    {
        return _maxUserFreq;
    }

    public void SetCompletedLevels()
    {
        completedLevels++;

        if (completedLevels % 4 == 0)
        {
            difficultyLevel++;
        }
    }

    public int GetDifficultyLevel()
    {
        return difficultyLevel;
    }
}
