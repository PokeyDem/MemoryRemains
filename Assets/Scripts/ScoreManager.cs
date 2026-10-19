using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private GameObject scoreUI;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button nextLevelButton;

    [SerializeField] private int passLevel = 60;

    public void ShowScore(double score)
    {
        scoreUI.SetActive(true);
        nextLevelButton.gameObject.SetActive(false);
        scoreText.text = $"Match: {score}%";

        if (score > passLevel)
        {
            nextLevelButton.gameObject.SetActive(true);
        }
        else
        {
            scoreText.text += "\n Too Low!";
        }
    }

    public void HideScore()
    {
        scoreUI.SetActive(false);
    }
}
