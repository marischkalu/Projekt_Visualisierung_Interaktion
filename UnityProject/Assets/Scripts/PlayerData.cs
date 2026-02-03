using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public bool[] LevelCompleted;

    public PlayerData (bool levelOneCompleted, bool levelTwoCompleted, bool levelThreeCompleted)
    {
        LevelCompleted = new bool[3];
        LevelCompleted[0] = levelOneCompleted;
        LevelCompleted[1] = levelTwoCompleted;
        LevelCompleted[2] = levelThreeCompleted;
    }
}
