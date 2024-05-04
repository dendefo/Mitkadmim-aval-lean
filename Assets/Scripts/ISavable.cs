using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;


public interface ISavable
{
    public static Dictionary<String, String> GameData;
    public static event Action<Dictionary<String, String>> SaveEvent;
    public static event Action<Dictionary<String, String>> LoadEvent;
    static void Save()
    {
        if (GameData == null)
        {
            GameData = new Dictionary<String, String>();
        }

        SaveEvent?.Invoke(GameData);
        
        string JsonOutput = JsonConvert.SerializeObject(GameData);
        string path = Application.persistentDataPath + "/Save.txt";

        using (StreamWriter outputFile = new StreamWriter(path,false))
        {
            outputFile.Write(JsonOutput);
            outputFile.Close();
        }
    }

    static void Load() 
    {
        string JsonData = File.ReadAllText(Application.persistentDataPath + "/Save.txt");
        if (JsonData == null)
        {
            return;
        }

        GameData = JsonConvert.DeserializeObject<Dictionary<String, String>>(JsonData);

        LoadEvent?.Invoke(GameData);
    }
}
