using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Minigame : MonoBehaviour
{
    public static event Action MinigameStartEvent;
    public static event Action MinigameEndEvent;
    public static event Action<Color[]> GainedColorEvent;
    //private bool _playerInMinigameArea;
    private bool _minigameIsActive;
    private bool _minigameIsCompleted = false;
    private bool _isFirstPlay = true;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _hint;
    [SerializeField] private Image _resultPositive;
    [SerializeField] private Image _resultNegative;
    [SerializeField] private List<ColorBottle> _colorBottles;
    [SerializeField] private KeyCode _minigameKey = KeyCode.F;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetMinigame();
    }

    // Update is called once per frame
    void Update()
    {

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
                ActivateMinigame();
                MinigameStartEvent?.Invoke();
                _minigameIsActive = true;
                _isFirstPlay = false;
            }
                
        }

        else
        {
            _canvas.enabled = false;
            MinigameEnd();
        }
    }

    public void OnFinalize()
    {
        MinigameResult();
    }

    void MinigameResult()
    {
        bool gainedColor = CheckCombination();

        Color[] colors = new Color[_colorBottles.Count];
        for (int i = 0; i < _colorBottles.Count; i++)
        {
            colors[i] = _colorBottles[i].CorrectColor;
        }

        if (gainedColor) GainedColorEvent?.Invoke(colors);
        _resultPositive.enabled = gainedColor;
        _resultNegative.enabled = !gainedColor;
        _minigameIsCompleted = gainedColor;

        ResetMinigame();
    }

    public void MinigameEnd()
    {
        MinigameEndEvent?.Invoke();
        _minigameIsActive = false;
    }


    void ResetMinigame()
    {
        foreach (ColorBottle bottle in _colorBottles)
        {
            bottle.ResetBottle();
        }
    }

    void ActivateMinigame()
    {

        if (_isFirstPlay) _hint.enabled = true;
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
    }
}
