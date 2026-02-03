using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public bool[] LevelCompleted;

    public PlayerData (bool[] levelCompleted)
    {
        LevelCompleted = levelCompleted;
    }
}
