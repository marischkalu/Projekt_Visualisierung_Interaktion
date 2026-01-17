using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Minigame : MonoBehaviour
{
    public static event Action MinigameStartEvent;
    public static event Action MinigameEndEvent;
    public static event Action GainedBrushEvent;
    //private bool _playerInMinigameArea;
    private bool _minigameIsActive;
    private bool _minigameIsCompleted = false;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _hint;
    [SerializeField] private Image _resultPositive;
    [SerializeField] private Image _resultNegative;
    [SerializeField] private List<ColorBottle> _colorBottles;
    [SerializeField] private KeyCode _minigameKey = KeyCode.F;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ColorBottle.FinalizeEvent += OnFinalize;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.E) && _playerInMinigameArea && !_minigameIsActive && !_minigameIsCompleted)
        //{
        //    ResetMinigame();
        //    MinigameStartEvent?.Invoke();
        //    _minigameIsActive = true;
        //}

        if (Input.GetKeyDown(_minigameKey))
        {
            OnMinigameButtonPressed();
        }

    }

    public void OnMinigameButtonPressed()
    {
        if (!_minigameIsActive)
        {
            if (!_minigameIsCompleted)
            {
                ResetMinigame();
                MinigameStartEvent?.Invoke();
                _minigameIsActive = true;
            }
                
        }

        else
        {
            _canvas.enabled = false;
            MinigameEnd();
        }
    }

    void OnFinalize()
    {
        foreach (ColorBottle bottle in _colorBottles)
        {
            if (!bottle.IsFinalized) return;
        }
        MinigameResult();
    }

    void MinigameResult()
    {
        bool gainedBrush = CheckCombination();
        if (gainedBrush) GainedBrushEvent?.Invoke();
        _resultPositive.enabled = gainedBrush;
        _resultNegative.enabled = !gainedBrush;
        _minigameIsCompleted = gainedBrush;
    }

    public void MinigameEnd()
    {
        MinigameEndEvent?.Invoke();
        _minigameIsActive = false;
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag != "Player") return;
    //    _playerInMinigameArea = true;
    //}
    //void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag != "Player") return;
    //    _playerInMinigameArea = false;
    //}

    void ResetMinigame()
    {
        foreach (ColorBottle bottle in _colorBottles)
        {
            bottle.ResetBottle();
        }

        _hint.enabled = true;
        _resultPositive.enabled = false;
        _resultNegative.enabled = false;
        _canvas.enabled = true;
    }

    bool CheckCombination()
    {
        foreach (ColorBottle bottle in _colorBottles)
        {
            if (!CheckFlask(bottle)) return false;
        }
        return true;
    }

    bool CheckFlask(ColorBottle bottle)
    {
        if (bottle.ColorContent.Count == 0) return false;

        foreach (Color content in bottle.ColorContent)
        {
            if (content.r < bottle.CorrectColor.r - 0.02f || content.r > bottle.CorrectColor.r + 0.02f) return false;
            if (content.g < bottle.CorrectColor.g - 0.02f || content.g > bottle.CorrectColor.g + 0.02f) return false;
            if (content.b < bottle.CorrectColor.b - 0.02f || content.b > bottle.CorrectColor.b + 0.02f) return false;
        }

        return true;
    }

    void OnDestroy()
    {
        ColorBottle.FinalizeEvent -= OnFinalize;
    }
}
