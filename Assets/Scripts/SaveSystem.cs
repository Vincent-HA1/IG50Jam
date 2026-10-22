using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem 
{
    public static void SaveGame(string locationName, List<Evidence> evidence)
    {
        SaveClass save = new SaveClass(locationName, evidence);
        string savePath = Application.persistentDataPath + $".json";

        JsonUtility.ToJson(save);
        string json = JsonUtility.ToJson(save, true);
        File.WriteAllText(savePath, json);
    }

    public static SaveClass LoadGame()
    {
        string savePath = Application.persistentDataPath + $".json";
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SaveClass>(json);
        }
        return null; // No save file found
    }

    public static void DeleteSave()
    {
        string savePath = Application.persistentDataPath + $".json";
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }
}

[System.Serializable]
public class SaveClass
{
    public string locationName;
    public List<Evidence> evidence;

    public SaveClass(string locationName, List<Evidence> evidence)
    {
        this.locationName = locationName;
        this.evidence = evidence;
    }
}
