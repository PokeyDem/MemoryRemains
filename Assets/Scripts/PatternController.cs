using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PatternController : MonoBehaviour
{
    [SerializeField] private AnimationCurve targetCurve;
    [SerializeField] private LineRenderer targetLine;
    [SerializeField] private LineRenderer playerLine;

    [SerializeField] private float cylinderLength = 100f;

    [SerializeField] private Vector2 targetVisualSize;
    [SerializeField] private Vector2 playerVisualSize;
    
    [SerializeField] private int targetLineResolution = 100;
    [SerializeField] private float recordStep = 0.5f;
    
    [Header("Temp")]
    [SerializeField] private TextMeshProUGUI scoreField;
    
    private List<Vector2> _playerRecord = new List<Vector2>();
    private float _lastRecordedX = 0f;
    private bool _isRecording = false;

    private void Start()
    {
        GenerateTargetLine();

        playerLine.positionCount = 0;
        _playerRecord.Clear();
        _lastRecordedX = 0f;
        _isRecording = true;
    }

    private void GenerateTargetLine()
    {
        targetLine.positionCount = targetLineResolution;

        for (int i = 0; i < targetLineResolution; i++)
        {
            float normalizedTime = i / (float)(targetLineResolution - 1); 
            
            float x = normalizedTime * cylinderLength; 
            
            float y = targetCurve.Evaluate(normalizedTime); 
            
            targetLine.SetPosition(i, new Vector3(x * targetVisualSize.x, y * targetVisualSize.y, 0f));
        }
    }

    public void RecordPlayerInput(float currentX, float currentPitch)
    {
        if (!_isRecording) return;

        if (currentX - _lastRecordedX >= recordStep)
        {
            Vector2 newPoint = new Vector2(currentX, currentPitch);
            _playerRecord.Add(newPoint);
            
            playerLine.positionCount = _playerRecord.Count;
            playerLine.SetPosition(_playerRecord.Count - 1, new Vector3(currentX * playerVisualSize.x, currentPitch * playerVisualSize.y, 0f));
            _lastRecordedX = currentX;

            if (currentX >= cylinderLength)
            {
                _isRecording = false;
                CalculateScore();
            }
        }
    }

    public void CalculateScore()
    {
        if (_playerRecord.Count == 0) return;

        float totalScore = 0f;
        
        float perfectTolerance = 0.15f; 
        
      
        float maxError = 0.7f; 

        foreach (Vector2 point in _playerRecord)
        {
            float idealY = targetCurve.Evaluate(point.x);
            float error = Mathf.Abs(idealY - point.y);

            float pointScore = 0f;

            if (error <= perfectTolerance)
            {
                pointScore = 1f;
            }
            else if (error < maxError)
            {
                float errorRange = maxError - perfectTolerance;
                float currentError = error - perfectTolerance;
                pointScore = 1f - (currentError / errorRange); 
            }

            totalScore += pointScore;
        }
        
        float averageScore = totalScore / _playerRecord.Count;
        float finalScore = Mathf.Clamp01(averageScore) * 100f;
        
        int displayScore = Mathf.RoundToInt(finalScore);
        
        scoreField.text = $"Match: {displayScore}%";
        Debug.Log($"Final score: {displayScore}");
    }
    
    public void Reset()
    {
        _playerRecord.Clear();
        playerLine.positionCount = 0;
        
        _lastRecordedX = 0f;
        _isRecording = true;
        scoreField.text = "Match:";
    }
    
    public Vector2 GetPlayerVisualSize(){
        return playerVisualSize;
    }
}
