using UnityEngine;
using System;

public class PaintingLevelEntry : MonoBehaviour
{
    [SerializeField] private int _thisLevel;
    private bool _canEnter = false;
    private bool _isCompleted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int levelUnlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);
        if (levelUnlocked == _thisLevel)
        {
            _canEnter = true;
        }

        if (levelUnlocked > _thisLevel)
        {
            _isCompleted = true;
        }

        if (_isCompleted)
        {
            // Change Appearance 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player") return;
        if (_canEnter)
        {
            Debug.Log(collision.ToString());
            // Load Scene
        }

    }
}
