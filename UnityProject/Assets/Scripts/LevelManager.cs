using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    [SerializeField] private int _thisLevel;
    private int _thisLevelNormalized;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _thisLevelNormalized = _thisLevel - 1;
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUGGING: Keys (AVE+S) -> SAVE GAME (LEVELUP)
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.V) && Input.GetKey(KeyCode.E))
        {
            bool[] gameState = LoadLevel();

            gameState[_thisLevelNormalized] = true;
            SaveLevel(gameState);

            LoadGalleryScene();
        }

        // DEBUGGING: Keys (EST+R) -> DELETE GAME (RESET TO LEVEL 1)
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.T))
        {
            bool[] levelCompleted = new bool[SaveSystem.TotalLevelCount];
            SaveLevel(levelCompleted);
            LoadGalleryScene();
        }
    }

    bool[] LoadLevel()
    {
        PlayerData data = SaveSystem.LoadLevel();
        return data.LevelCompleted;
    }

    void SaveLevel(bool[] levelCompleted)
    {
        SaveSystem.SaveLevel(levelCompleted);
    }

    void LoadGalleryScene()
    {
        SceneManager.LoadScene(0);
    }
}
