using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private LevelManager _levelManager;
    private Canvas _canvas;
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
        if (Input.GetKey(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Leave()
    {
        Time.timeScale = 1.0f;
        _canvas.enabled = false;
        PauseMenuClosedEvent?.Invoke();
        _levelManager.LevelEscape();
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        _canvas.enabled = false;
        PauseMenuClosedEvent?.Invoke();
    }
    void Pause()
    {
        Time.timeScale = 0f;
        _canvas.enabled = true;
        PauseMenuOpenedEvent?.Invoke();
    }
}
