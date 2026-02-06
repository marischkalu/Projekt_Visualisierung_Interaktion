using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static int TotalLevelCount = 3;

    public static void SaveLevel (bool[] levelCompleted, int currentLevel)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/level.cnt";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(levelCompleted, currentLevel);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadLevel ()
    {
        string path = Application.persistentDataPath + "/level.cnt";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
