using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveGenerator : MonoBehaviour
{
    [Header("Grid settings")]
    [SerializeField] private int columns = 5;
    [SerializeField] private int rows = 5;
    [SerializeField] private int maxJump = 2;
    
    public AnimationCurve GenerateRandomPattern()
    {
        int currentColumns = columns + GameManager.Instance.GetDifficultyLevel();
        int currentRows = rows + GameManager.Instance.GetDifficultyLevel();
        int currentMaxJump = maxJump + GameManager.Instance.GetDifficultyLevel();
        
        AnimationCurve generatedCurve = new AnimationCurve();
        
        int currentYLevel = Random.Range(-currentMaxJump, currentMaxJump); 

        for (int i = 0; i < currentColumns; i++)
        {
            float normalizedX = (float)i / (currentColumns - 1);
            float normalizedY = (float)currentYLevel / (currentRows - 1);
            
            Keyframe key = new Keyframe(normalizedX, normalizedY);
            generatedCurve.AddKey(key);
            
            int minPossibleLevel = Mathf.Max(0, currentYLevel - currentMaxJump);
            int maxPossibleLevel = Mathf.Min(currentRows - 1, currentYLevel + currentMaxJump);
            currentYLevel = Random.Range(minPossibleLevel, maxPossibleLevel + 1);
        }
        
        for (int i = 0; i < generatedCurve.keys.Length; i++)
        {
            generatedCurve.SmoothTangents(i, 0f);
        }

        return generatedCurve;
    }
}
