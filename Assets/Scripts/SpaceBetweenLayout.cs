using UnityEngine;

[ExecuteAlways]
public class SpaceBetweenLayout : MonoBehaviour
{
    public float paddingLeft = 0;
    public float paddingRight = 0;

    void Update()
    {
        int count = transform.childCount;
        if (count < 2) return;

        float parentWidth = ((RectTransform)transform).rect.width;

        float totalChildWidth = 0f;
        RectTransform[] children = new RectTransform[count];

        for (int i = 0; i < count; i++)
        {
            children[i] = transform.GetChild(i) as RectTransform;
            totalChildWidth += children[i].rect.width;
        }

        float availableSpace = parentWidth - paddingLeft - paddingRight - totalChildWidth;
        float spacing = availableSpace / (count - 1);

        float currentX = paddingLeft;

        for (int i = 0; i < count; i++)
        {
            children[i].anchoredPosition = new Vector2(currentX, children[i].anchoredPosition.y);
            currentX += children[i].rect.width + spacing;
        }
    }
}