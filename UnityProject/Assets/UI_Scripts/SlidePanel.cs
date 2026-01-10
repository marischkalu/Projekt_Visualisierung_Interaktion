using UnityEngine;

public class SlidePanel : MonoBehaviour
{
    public RectTransform panel;
    public float hiddenY = -200f;
    public float shownY = 0f;
    private bool isHidden = false;

    public void TogglePanel()
    {
        float newY = isHidden ? shownY : hiddenY;
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, newY);
        isHidden = !isHidden;
    }
}
