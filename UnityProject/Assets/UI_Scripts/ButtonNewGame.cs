using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonNewGame : MonoBehaviour
{
    public void OpenBookScene()
    {
        SceneManager.LoadScene("Book");
    }
}
