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
    [SerializeField] private CurveGenerator curveGenerator;
    [SerializeField] private ScoreManager scoreManager;

    [SerializeField] private float cylinderLength = 100f;

    [SerializeField] private Vector2 targetVisualSize;
    [SerializeField] private Vector2 playerVisualSize;
    
    [SerializeField] private int targetLineResolution = 100;
    [SerializeField] private float recordStep = 0.5f;
    
    [Header("Composite Scoring Settings")]
    [SerializeField] private float shapeWeight = 0.6f;
    [SerializeField] private float anchorsWeight = 0.4f;
    [SerializeField] private float slopeDeadzone = 0.02f;
    [SerializeField] private float maxAnchorError = 0.4f;
    [SerializeField] private float minVarianceThreshold = 0.05f;
    
    
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

    public void GenerateNewPattern()
    {
        targetCurve = curveGenerator.GenerateRandomPattern();
        GenerateTargetLine();
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
        if (_playerRecord.Count < 2) return;

        int pointCount = _playerRecord.Count;
        float shapeScore = 0f;
        int validSlopes = 0;
        
        float minPlayerY = _playerRecord[0].y;
        float maxPlayerY = _playerRecord[0].y;
        
        for (int i = 1; i < pointCount; i++)
        {
            float currentY = _playerRecord[i].y;
            if (currentY < minPlayerY) minPlayerY = currentY;
            if (currentY > maxPlayerY) maxPlayerY = currentY;

            float targetDelta = targetCurve.Evaluate(_playerRecord[i].x) - targetCurve.Evaluate(_playerRecord[i - 1].x);
            float playerDelta = currentY - _playerRecord[i - 1].y;

            if (Mathf.Abs(targetDelta) < slopeDeadzone) targetDelta = 0f;
            if (Mathf.Abs(playerDelta) < slopeDeadzone) playerDelta = 0f;

            int targetSign = targetDelta == 0f ? 0 : (int)Mathf.Sign(targetDelta);
            int playerSign = playerDelta == 0f ? 0 : (int)Mathf.Sign(playerDelta);

            if (targetSign == playerSign)
            {
                shapeScore += 1f; 
            }
            else if (targetSign != 0 && playerSign == 0)
            {
                shapeScore += 0.3f; 
            }
            
            validSlopes++;
        }
        
        float finalShapeScore = validSlopes > 0 ? shapeScore / validSlopes : 0f;
        
        float startTarget = targetCurve.Evaluate(_playerRecord[0].x);
        float startError = Mathf.Clamp01(Mathf.Abs(startTarget - _playerRecord[0].y) / maxAnchorError);
        float startScore = 1f - startError;

        float endTarget = targetCurve.Evaluate(_playerRecord[pointCount - 1].x);
        float endError = Mathf.Clamp01(Mathf.Abs(endTarget - _playerRecord[pointCount - 1].y) / maxAnchorError);
        float endScore = 1f - endError;

        float finalAnchorScore = (startScore + endScore) / 2f;

    
        float totalScore = (finalShapeScore * shapeWeight) + (finalAnchorScore * anchorsWeight);
        
        float playerVariance = maxPlayerY - minPlayerY;
        if (playerVariance < minVarianceThreshold)
        {
            totalScore = 0f;
        }

        float finalPercentage = Mathf.Clamp01(totalScore) * 100f;
        
        int displayScore = Mathf.RoundToInt(finalPercentage);
        
        if (scoreManager != null)
        {
            scoreManager.ShowScore(Math.Round(finalPercentage, 2));
        }
        
        Debug.Log($"Shape: {finalShapeScore:F2} | Anchor: {finalAnchorScore:F2} | Variance: {playerVariance:F3} | Final: {displayScore}%");
    }

    public void Reset()
    {
        _playerRecord.Clear();
        playerLine.positionCount = 0;
        
        _lastRecordedX = 0f;
        _isRecording = true;
    }
    
    public Vector2 GetPlayerVisualSize(){
        return playerVisualSize;
    }
    
    public void UpdateDynamicLineEnd(float normX, float normY)
    {
        if (!_isRecording || _playerRecord.Count == 0) return;
        
        if (playerLine.positionCount == _playerRecord.Count)
        {
            playerLine.positionCount = _playerRecord.Count + 1;
        }

        float localX = normX * playerVisualSize.x;
        
        float localY = normY * playerVisualSize.y; 
        
        playerLine.SetPosition(_playerRecord.Count, new Vector3(localX, localY, 0f));
    }

    public bool IsRecording()
    {
        return _isRecording;
    }
}
