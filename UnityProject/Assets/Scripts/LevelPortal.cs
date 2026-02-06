using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelPortal : MonoBehaviour
{
    [SerializeField] private int _thisLevel;
    private int _thisLevelNormalized;
    [SerializeField] private GameObject _rippleEffectObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _thisLevelNormalized = _thisLevel - 1;
        if (_rippleEffectObject != null) ChangeAvailability(_rippleEffectObject, IsLevelAvailable());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsLevelAvailable()) return;
        SaveLevel();
        SceneManager.LoadScene(_thisLevel + 2);
    }

    bool[] LoadLevel()
    {
        PlayerData data = SaveSystem.LoadLevel();
        return data.LevelCompleted;
    }

    void SaveLevel()
    {
        SaveSystem.SaveLevel(LoadLevel(), _thisLevel);
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

    void ChangeAvailability(GameObject rippleGameObject, bool isAvailable)
    {
        //renderer.material.color = Color.red;
        rippleGameObject.SetActive(false);
        if (!isAvailable) return;
        rippleGameObject.SetActive(true);
        //renderer.material.color = Color.green;
    }
}
