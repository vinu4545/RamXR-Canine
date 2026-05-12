using UnityEngine;
using UnityEngine.UI;
using Meta.WitAi.TTS.Utilities;
using TMPro;

public class PanelAudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TTSSpeaker _ttsSpeaker;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Button _listenButton;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _nextButton;

    [Header("Panel Data")]
    [SerializeField] private PanelData[] _panels;

    private int _currentIndex = 0;

    private void Start()
    {
        _listenButton.onClick.AddListener(OnListenClicked);
        _previousButton.onClick.AddListener(OnPreviousClicked);
        _nextButton.onClick.AddListener(OnNextClicked);

        UpdatePanel();
    }

    private void OnNextClicked()
    {
        _currentIndex = (_currentIndex + 1) % _panels.Length;
        UpdatePanel();
    }

    private void OnPreviousClicked()
    {
        _currentIndex = (_currentIndex - 1 + _panels.Length) % _panels.Length;
        UpdatePanel();
    }

    private void UpdatePanel()
    {
        _ttsSpeaker.Stop();
        _titleText.text = _panels[_currentIndex].title;
        _descriptionText.text = _panels[_currentIndex].description;
        _listenButton.interactable = true;
    }

    private void OnListenClicked()
    {
        if (_ttsSpeaker == null || _descriptionText == null)
        {
            Debug.LogError("PanelAudioController: Missing reference!");
            return;
        }

        string textToSpeak = _descriptionText.text;
        if (string.IsNullOrWhiteSpace(textToSpeak)) return;

        _listenButton.interactable = false;
        _ttsSpeaker.Stop();
        _ttsSpeaker.SpeakQueued(textToSpeak);

        StartCoroutine(WaitForSpeakToFinish());
    }

    private System.Collections.IEnumerator WaitForSpeakToFinish()
    {
        yield return null;

        AudioSource audio = _ttsSpeaker.GetComponent<AudioSource>();

        if (audio != null)
        {
            yield return new WaitUntil(() => audio.isPlaying);
            yield return new WaitWhile(() => audio.isPlaying);
        }
        else
        {
            int wordCount = _descriptionText.text.Split(' ').Length;
            float estimatedDuration = wordCount * 0.4f;
            yield return new WaitForSeconds(estimatedDuration);
        }

        _listenButton.interactable = true;
    }

    private void OnDestroy()
    {
        _listenButton.onClick.RemoveListener(OnListenClicked);
        _previousButton.onClick.RemoveListener(OnPreviousClicked);
        _nextButton.onClick.RemoveListener(OnNextClicked);
        StopAllCoroutines();
    }
}

[System.Serializable]
public class PanelData
{
    public string title;
    [TextArea(3, 6)]
    public string description;
}