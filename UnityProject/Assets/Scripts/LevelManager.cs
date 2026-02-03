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
        Debug.Log($"{LoadLevel()[0]}, {LoadLevel()[1]}, {LoadLevel()[2]}");
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUGGING: Keys (AVE+S) -> SAVE GAME (LEVELUP)
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.V) && Input.GetKey(KeyCode.E))
        {
            bool[] gameState = LoadLevel();

            gameState[_thisLevelNormalized] = true;
            SaveLevel(gameState[0], gameState[1], gameState[2]);

            LoadGalleryScene();
        }

        // DEBUGGING: Keys (EST+R) -> DELETE GAME (RESET TO LEVEL 1)
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.T))
        {
            SaveLevel(false, false, false);
            LoadGalleryScene();
        }
    }

    bool[] LoadLevel()
    {
        PlayerData data = SaveSystem.LoadLevel();
        return data.LevelCompleted;
    }

    void SaveLevel(bool levelOneCompleted, bool levelTwoCompleted, bool levelThreeCompleted)
    {
        SaveSystem.SaveLevel(levelOneCompleted, levelTwoCompleted, levelThreeCompleted);
    }

    void LoadGalleryScene()
    {
        SceneManager.LoadScene(0);
    }
}
