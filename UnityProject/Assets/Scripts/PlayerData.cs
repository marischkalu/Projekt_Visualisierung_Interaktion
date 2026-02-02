using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int Level;

    public PlayerData (int levelCompleted)
    {
        Level = levelCompleted;
    }
}
