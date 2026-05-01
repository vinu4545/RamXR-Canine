using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialStepButton : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public Image background;

    public Color activeColor;
    public Color inactiveColor;

    private int index;
    private System.Action<int> onClick;

    public void Setup(int index, System.Action<int> callback)
    {
        this.index = index;
        this.onClick = callback;

        numberText.text = (index + 1).ToString();
    }

    public void SetActive(bool isActive)
    {
        background.color = isActive ? activeColor : inactiveColor;
    }

    public void OnClick()
    {
        onClick?.Invoke(index);
    }
}