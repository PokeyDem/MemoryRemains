using TMPro;
using UnityEngine;

public class MicrophoneIndicatorUI : MonoBehaviour
{
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private TextMeshProUGUI micDataText;
    [SerializeField] private SpriteRenderer micIndicator;
    [SerializeField] private float threshold = 0.02f;

    private void Start()
    {
        micDataText.text = "Mic Ready"; 
    }

    private void Update()
    {
        if (WebGLMicrophone.Instance == null || !WebGLMicrophone.Instance.isInitialized)
        {
            micIndicator.color = inactiveColor;
            return;
        }

        float[] samples = WebGLMicrophone.Instance.GetAudioData();
        float sum = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        
        float rms = Mathf.Sqrt(sum / samples.Length);

        if (rms > threshold)
        {
            micIndicator.color = activeColor;
        }
        else
        {
            micIndicator.color = inactiveColor;
        }
    }
}