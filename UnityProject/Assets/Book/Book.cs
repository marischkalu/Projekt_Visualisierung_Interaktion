using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
    [SerializeField] private float _pageSpeed = 0.1f;
    [SerializeField] private List<Transform> _bookPages;
    private int _pageIndex = 0;
    private bool _isRotating = false;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _forwardButton;

    [SerializeField] private Button _finishButton;
    [SerializeField] private Button _skipButton;

    private bool _skipButtonDisappeared = false;
    void Start()
    {
        InitialState();
        _finishButton.gameObject.SetActive(false);
    }

    void InitialState()
    {
        for (int i = 0; i < _bookPages.Count; i++)
        {
            _bookPages[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        _bookPages[0].SetAsLastSibling();
        _backButton.gameObject.SetActive(false);
    }

    public void RotateForward()
    {
        if (_isRotating) return;
        _pageIndex++;
        float angle = -180;
        ButtonVisibility();
        _bookPages[_pageIndex - 1].SetAsLastSibling();
        StartCoroutine(Rotate(angle, true));
    }

    public void RotateBack()
    {
        if (_isRotating) return;
        _pageIndex--;
        float angle = 0;
        ButtonVisibility();
        _bookPages[_pageIndex].SetAsLastSibling();
        StartCoroutine(Rotate(angle, false));
    }

    void ButtonVisibility()
    {
        Debug.Log(_pageIndex);

        if (_pageIndex == _bookPages.Count - 1)
        {
            _forwardButton.gameObject.SetActive(false);
            _finishButton.gameObject.SetActive(true);

            if (!_skipButtonDisappeared)
            {
                _skipButton.gameObject.SetActive(false);
                _skipButtonDisappeared = true;
            }
        }
        else
        {
            _forwardButton.gameObject.SetActive(true);
            _finishButton.gameObject.SetActive(false);
        }

        if (_pageIndex == 0)
        {
            _backButton.gameObject.SetActive(false);
        }
        else
        {
            _backButton.gameObject.SetActive(true);
        }
    }


    IEnumerator Rotate(float angle, bool forward)
    {
        float value = 0f;
        while (true)
        {
            _isRotating = true;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            value += Time.deltaTime * _pageSpeed;
            _bookPages[forward? _pageIndex - 1 : _pageIndex].localRotation = Quaternion.Slerp(_bookPages[forward ? _pageIndex - 1 : _pageIndex].localRotation, targetRotation, value);
            float angle1 = Quaternion.Angle(_bookPages[forward ? _pageIndex - 1 : _pageIndex].localRotation, targetRotation);
            if (angle1 < 0.1f)
            {
                _isRotating = false;
                break;
            }
            yield return null;
        }
    }
}
