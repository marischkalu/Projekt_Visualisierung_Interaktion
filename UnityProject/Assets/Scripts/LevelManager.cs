using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private int _thisLevel;
    [SerializeField] ObjectColorSystem[] _objectsToPaint;
    private int _thisLevelNormalized;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        _thisLevelNormalized = _thisLevel - 1;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LevelCompleted()
    {
        if (!AreAllPaintedObjectsCorrect()) return;
        bool[] gameState = LoadLevel();

        gameState[_thisLevelNormalized] = true;
        SaveLevel(gameState, 0);

        LoadGalleryScene();
    }

    bool[] LoadLevel()
    {
        PlayerData data = SaveSystem.LoadLevel();
        return data.LevelCompleted;
    }

    void SaveLevel(bool[] levelCompleted, int currentLevel)
    {
        SaveSystem.SaveLevel(levelCompleted, currentLevel);
    }

    void LoadGalleryScene()
    {
        SceneManager.LoadScene("Gallery");
    }

    bool AreAllPaintedObjectsCorrect()
    {
        for(int i = 0; i < _objectsToPaint.Length; i++)
        {
            if (!_objectsToPaint[i].AppliedColor.Equals(_objectsToPaint[i].CorrectColor)) return false;
        }
        return true;
    }
}
