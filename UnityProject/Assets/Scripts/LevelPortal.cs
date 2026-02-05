using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelPortal : MonoBehaviour
{
    [SerializeField] private int _thisLevel;
    private int _thisLevelNormalized;
    private Renderer _renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _thisLevelNormalized = _thisLevel - 1;
        _renderer = GetComponent<Renderer>();
        if (_renderer != null) ChangeAvailability(_renderer, IsLevelAvailable());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsLevelAvailable()) return;
        SceneManager.LoadScene(_thisLevel + 2);
    }

    bool[] LoadLevel()
    {
        PlayerData data = SaveSystem.LoadLevel();
        return data.LevelCompleted;
    }
    
    // DEBUGGING COLOR CHANGE OF PORTALS
    bool IsLevelAvailable()
    {

        int previousLevel = _thisLevelNormalized - 1;

        bool isFirstLevel = _thisLevelNormalized == 0;

        if (!isFirstLevel)
        {
            if (LoadLevel()[previousLevel] == false) return false;
        }
        return true;
    }

    void ChangeAvailability(Renderer renderer, bool isAvailable)
    {
        renderer.material.color = Color.red;
        if (!isAvailable) return;
        renderer.material.color = Color.green;
    }
}
