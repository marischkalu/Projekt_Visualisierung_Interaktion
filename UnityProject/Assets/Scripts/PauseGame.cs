using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    private Canvas _canvas;
    private bool _isActive;
    public static event Action PauseMenuOpenedEvent;
    public static event Action PauseMenuClosedEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_isActive)
            {
                Pause();
                
            }
            else
            {
                ResumeGame();
            }

        }
    }

    public void BackToGallery()
    {
        Time.timeScale = 1.0f;
        _canvas.enabled = false;
        _isActive = false;
        PauseMenuClosedEvent?.Invoke();
        LoadGalleryScene();
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1.0f;
        _canvas.enabled = false;
        _isActive = false;
        PauseMenuClosedEvent?.Invoke();
        LoadMainMenu();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        _canvas.enabled = false;
        _isActive = false;
        PauseMenuClosedEvent?.Invoke();
    }
    void Pause()
    {
        Time.timeScale = 0f;
        _canvas.enabled = true;
        _isActive = true;
        PauseMenuOpenedEvent?.Invoke();
    }

    void LoadGalleryScene()
    {
        SceneManager.LoadScene("Gallery");
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
