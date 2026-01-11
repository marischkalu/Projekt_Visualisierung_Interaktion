using UnityEngine;
using UnityEngine.UI;

public class BallUI : MonoBehaviour
{
    public Transform ballsParent;
    private Image[] ballDots;
    public Color fullColor = Color.green;
    public Color emptyColor = Color.gray;
    public int ballsLeft = 5;

    void Start()
    {
        // get all children images automatically
        ballDots = ballsParent.GetComponentsInChildren<Image>();
        UpdateDots();
    }

    public void UseBall()
    {
        if (ballsLeft > 0)
        {
            ballsLeft--;
            UpdateDots();
        }
    }

    void UpdateDots()
    {
        for (int i = 0; i < ballDots.Length; i++)
        {
            if (i < ballsLeft)
                ballDots[i].color = fullColor;
            else
                ballDots[i].color = emptyColor;
        }
    }
}
