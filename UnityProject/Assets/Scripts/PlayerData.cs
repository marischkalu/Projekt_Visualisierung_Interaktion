using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public bool[] LevelCompleted;
    public int CurrentLevel;

    public PlayerData (bool[] levelCompleted, int currentLevel)
    {
        LevelCompleted = levelCompleted; // [true, false, false] -> Level 1 has been completed; [false, false, false] -> No Level completed
        CurrentLevel = currentLevel; // Always 0 when Player is not inside a Level Scene --- 0 -> Gallery / Menu / Not inside a Level; 1, 2, 3... -> Player is inside said Level 

    }
}
