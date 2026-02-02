using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelPortal : MonoBehaviour
{
    [SerializeField] private int _thisLevel;
    private Renderer _renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null) ChangeAvailability(_renderer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (LoadLevel() < _thisLevel) return;
        SceneManager.LoadScene(_thisLevel);
    }

    int LoadLevel()
    {
        PlayerData data = SaveSystem.LoadLevel();
        return data.Level;
    }
    
    // DEBUGGING COLOR CHANGE OF PORTALS
    void ChangeAvailability(Renderer renderer)
    {
        _renderer.material.color = Color.red;
        if (LoadLevel() < _thisLevel) return;
        _renderer.material.color = Color.green;
    }
}
