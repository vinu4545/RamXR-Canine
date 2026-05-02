using UnityEngine;
using TMPro;
using Meta.WitAi.TTS.Utilities; // Essential for the Meta Quest TTS Agent

public class audio : MonoBehaviour
{
    [Header("UI & Audio References")]
    public TextMeshProUGUI infoText;
    public TTSSpeaker ttsSpeaker; // Drag the [BuildingBlock] Text To Speech here

    private int index = 0;

    // Content from your panel in image_b4675c.png
    private string[] data = {
        "Dog anatomy refers to the structure of a dog's body, including its external features and internal systems. Externally, it includes parts like the head, ears, legs, tail, and coat.",
        "Internally, a dog's body is made up of systems such as the skeletal system for support, and the muscular system for movement.",
        "The circulatory system manages heart and blood flow, while the respiratory system uses lungs for breathing.",
        "The digestive system processes food, and the nervous system uses the brain and nerves for control. All these systems work together to help the dog move and maintain health."
    };

    void Start()
    {
        // Set the initial text
        if (infoText != null) infoText.text = data[0];
    }

    public void Next()
    {
        index = (index + 1) % data.Length;
        UpdateUI(true); // Automatically speaks when you change page
    }

    public void Previous()
    {
        index = (index - 1 + data.Length) % data.Length;
        UpdateUI(true);
    }

    // Function for your Speaker Button
    public void PlayCurrentAudio()
    {
        if (ttsSpeaker != null)
        {
            ttsSpeaker.Stop(); 
            ttsSpeaker.Speak(data[index]);
        }
    }

    void UpdateUI(bool shouldSpeak)
    {
        if (infoText != null) infoText.text = data[index];
        if (shouldSpeak) PlayCurrentAudio();
    }
}