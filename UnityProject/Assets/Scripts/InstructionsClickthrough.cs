using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class InstructionsClickthrough : MonoBehaviour
{

    public static event Action InstructionsStartEvent;
    public static event Action InstructionsEndEvent;

    private int _currentIndex= 0;
    [SerializeField] private Transform _textsParent;
    [SerializeField] private TMP_Text[] _texts;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _skipButton;


    void Start()
    {
        InstructionsStartEvent?.Invoke();
        _texts = _textsParent.GetComponentsInChildren<TMP_Text>(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Next()
    {
        _currentIndex = _texts.Length - 1 > _currentIndex ? _currentIndex + 1 : _currentIndex;
        UpdateImageVisibility();
        UpdateButtonVisibility();
    }

    public void Previous()
    {
        _currentIndex = 0 < _currentIndex ? _currentIndex - 1 : _currentIndex;
        UpdateImageVisibility();
        UpdateButtonVisibility();
    }

    public void Skip()
    {
        gameObject.SetActive(false);
        InstructionsEndEvent?.Invoke();
    }

    void UpdateImageVisibility()
    {
        for (int i = 0; i < _texts.Length; i++)
        {
            if (i != _currentIndex) _texts[i].gameObject.SetActive(false);
            else _texts[i].gameObject.SetActive(true);

        }
    }

    void UpdateButtonVisibility()
    {
        if (_nextButton != null)
        {
            if ( _currentIndex == _texts.Length - 1) _nextButton.interactable = false;
            else _nextButton.interactable = true;
        }
    }

}
