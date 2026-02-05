using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipIntro : MonoBehaviour
{
    public void GoToGallery()
    {
        SceneManager.LoadScene("Gallery");
    }
}
