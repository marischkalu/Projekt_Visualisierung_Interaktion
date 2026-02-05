using UnityEngine;
using UnityEngine.SceneManagement;
using System;
[Obsolete("Not used any more", true)]
public class ButtonNewGame : MonoBehaviour
{
    public void OpenBookScene()
    {
        SceneManager.LoadScene("Book");
    }
}
