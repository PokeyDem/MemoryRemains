using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVisualsController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    
    public void SetSliderValue(float value)
    {
        slider.value = value;
    }
}
