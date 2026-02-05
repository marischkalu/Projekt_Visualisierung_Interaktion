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
        Debug.Log($"{LoadLevel()[0]}, {LoadLevel()[1]}, {LoadLevel()[2]}");
        _isGameStateEmpty = Array.IndexOf(LoadLevel(), true) < 0;

        if (!_isGameStateEmpty) return;

        Image buttonImage = _loadGameButton.GetComponent<Image>();
        Color buttonColor = buttonImage.color;
        buttonColor.a = 0.3f;
        buttonImage.color = buttonColor;
        
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
        if (_isGameStateEmpty) return;
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
