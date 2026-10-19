using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CrankBehaviour : MonoBehaviour
{
    [SerializeField] private VoiceProcessor voiceProcessor;
    [SerializeField] private PatternController patternController;
    
    [SerializeField] private Transform crankCenter; 
    [SerializeField] private float cylinderLength = 100f;
    [SerializeField] private float spinsToComplete = 3f;
    [SerializeField] private Transform needleTransform;

    [SerializeField] private float needleYOffset;
    [SerializeField] private float needleXOffset;
    
    [SerializeField] private int movingAverageWindow = 6;
    [SerializeField] private float deadzone = 0.01f;
    [SerializeField] private float spikeThreshold = 0.4f;
    [SerializeField] private float smoothSpeed = 15f;
    
 
    private Queue<float> _yBuffer = new Queue<float>();
    private float _lastRawY;
    private float _lastValidY;
    private float _smoothedY;

    private float _dynamicMultiplier;
    private float _lastAngle;
    private float _currentX;

    private void OnMouseDown()
    {
        _dynamicMultiplier = cylinderLength / (spinsToComplete * 360f);
        Vector2 centerScreenPos = Camera.main.WorldToScreenPoint(crankCenter.position);
        
        Vector2 direction = (Vector2)Input.mousePosition - centerScreenPos;
        _lastAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
    
    private void OnMouseDrag()
    {
        Vector2 centerScreenPos = Camera.main.WorldToScreenPoint(crankCenter.position);
        Vector2 direction = (Vector2)Input.mousePosition - centerScreenPos;
        float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        float deltaAngle = Mathf.DeltaAngle(_lastAngle, currentAngle);
        float clockwiseStep = -deltaAngle;

        if (clockwiseStep > 0)
        {
            _currentX += clockwiseStep * _dynamicMultiplier; 
        
            patternController.RecordPlayerInput(_currentX, _smoothedY);
            crankCenter.Rotate(0, 0, -clockwiseStep);
        }
        
        _lastAngle = currentAngle;
    }
    
    public void ResetCrank()
    {
        _currentX = 0f;
        _yBuffer.Clear();
        _lastRawY = 0f;
        _lastValidY = 0f;
        _smoothedY = 0f;
        crankCenter.localRotation = Quaternion.identity;
    }

    private void Update()
    {
        
        if (!patternController.IsRecording()) return;
        
        float realVoiceY = voiceProcessor.GetTargetValue();
        float processedY = realVoiceY;
        
        if (Mathf.Abs(realVoiceY - _lastRawY) > spikeThreshold)
        {
            processedY = _lastValidY; 
        }
        _lastRawY = realVoiceY; 
        
        if (Mathf.Abs(processedY - _lastValidY) < deadzone)
        {
            processedY = _lastValidY; 
        }
        else
        {
            _lastValidY = processedY; 
        }
        
        _yBuffer.Enqueue(processedY);
        if (_yBuffer.Count > movingAverageWindow)
        {
            _yBuffer.Dequeue(); 
        }

        float averageY = 0f;
        foreach (float val in _yBuffer)
        {
            averageY += val;
        }
        averageY /= _yBuffer.Count;
        
        _smoothedY = Mathf.Lerp(_smoothedY, averageY, Time.deltaTime * smoothSpeed);
      
        if (needleTransform != null)
        {
            Vector2 pSize = patternController.GetPlayerVisualSize();
            needleTransform.localPosition = new Vector3(_currentX * pSize.x, _smoothedY * pSize.y, 0f);
            patternController.UpdateDynamicLineEnd(_currentX, _smoothedY);
        }
    }
}
