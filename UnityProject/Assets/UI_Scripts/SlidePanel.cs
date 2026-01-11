using UnityEngine;

public class SlidePanel : MonoBehaviour
{
    public RectTransform panel; 
    public RectTransform arrow; 
    public float hiddenY = -200f; 
    public float shownY = 0f;
    private bool isHidden = false;

    public void TogglePanel()
    {
        // Move panel
        float newY = isHidden ? shownY : hiddenY;
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, newY);

        // Flip arrow
        if (arrow != null)
        {
            arrow.localEulerAngles = isHidden ? new Vector3(0, 0, 180) : new Vector3(0, 0, 0);
        }

        // Toggle state
        isHidden = !isHidden;
    }
}
