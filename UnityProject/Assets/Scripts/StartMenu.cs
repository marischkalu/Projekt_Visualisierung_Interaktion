using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Button _loadGameButton;
    private bool _isGameStateEmpty = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;

        _isGameStateEmpty = Array.IndexOf(LoadLevel(), true) < 0;

        if (!_isGameStateEmpty) return;

        _loadGameButton.interactable = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        EraseGameState();
        SceneManager.LoadScene("Book");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Gallery");
    }

    void EraseGameState()
    {
        bool[] levelCompleted = new bool[SaveSystem.TotalLevelCount];
        SaveSystem.SaveLevel(levelCompleted);
    }

    bool[] LoadLevel()
    {
        PlayerData data = SaveSystem.LoadLevel();
        return data.LevelCompleted;
    }
}
