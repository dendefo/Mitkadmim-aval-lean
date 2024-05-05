using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;


public interface ISavable
{
    public static bool WantsToLoad { get; set; }
    public static Dictionary<String, object> GameData;
    public static event Action<Dictionary<String, object>> SaveEvent;
    public static event Action<Dictionary<String, object>> LoadEvent;
    static void Save()
    {
        if (GameData == null)
        {
            GameData = new Dictionary<String, object>();
        }

        SaveEvent?.Invoke(GameData);
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        string JsonOutput = JsonConvert.SerializeObject(GameData, Formatting.Indented,settings);
        string path = Application.persistentDataPath + "/Save.txt";
        Debug.Log(path);
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

        GameData = JsonConvert.DeserializeObject<Dictionary<String, object>>(JsonData);

        LoadEvent?.Invoke(GameData);
    }
}
