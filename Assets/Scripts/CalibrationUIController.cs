using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalibrationUIController : MonoBehaviour
{
    [SerializeField] private GameObject minFreqCalibrationUIGroup;
    [SerializeField] private GameObject maxFreqCalibrationUIGroup;
    [SerializeField] private GameObject resultUIGroup;
    [SerializeField] private TextMeshProUGUI resultText;

    private void Start()
    {
        minFreqCalibrationUIGroup.SetActive(true);
        maxFreqCalibrationUIGroup.SetActive(false);
    }
    
    public void SwitchToMaxFreqCalibration()
    {
        minFreqCalibrationUIGroup.SetActive(false);
        maxFreqCalibrationUIGroup.SetActive(true);
    }

    public void ShowResult()
    {
        maxFreqCalibrationUIGroup.SetActive(false);
        resultUIGroup.SetActive(true);
        resultText.text = $"Result:\n Min. Pitch: {GameManager.Instance.GetMinFreq()} Hz\n Max. Pitch: {GameManager.Instance.GetMaxFreq()} Hz";
    }

    public void Restart()
    {
        resultUIGroup.SetActive(false);
        minFreqCalibrationUIGroup.SetActive(true);
    }

    public void BackToMenu()
    {
        GameManager.Instance.SwitchToMenu();
    }
}
