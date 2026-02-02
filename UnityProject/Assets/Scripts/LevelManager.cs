using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    [SerializeField] private int _thisLevel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUGGING: Keys (AVE+S) -> SAVE GAME (LEVELUP)
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.V) && Input.GetKey(KeyCode.E))
        {
            if (LoadLevel() <= _thisLevel) SaveLevel(_thisLevel + 1);
            LoadGalleryScene();
        }

        // DEBUGGING: Keys (EST+R) -> DELETE GAME (RESET TO LEVEL 1)
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.T))
        {
            SaveLevel(1);
            LoadGalleryScene();
        }
    }

    int LoadLevel()
    {
        PlayerData data = SaveSystem.LoadLevel();
        return data.Level;
    }

    void SaveLevel(int levelToSave)
    {
        SaveSystem.SaveLevel(levelToSave);
    }

    void LoadGalleryScene()
    {
        SceneManager.LoadScene(0);
    }
}
