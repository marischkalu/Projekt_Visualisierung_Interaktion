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
    }

    public void LevelCompleted()
    {
        bool[] gameState = LoadLevel();

        gameState[_thisLevelNormalized] = true;
        SaveLevel(gameState);

        LoadGalleryScene();
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
        SceneManager.LoadScene("Gallery");
    }
}
