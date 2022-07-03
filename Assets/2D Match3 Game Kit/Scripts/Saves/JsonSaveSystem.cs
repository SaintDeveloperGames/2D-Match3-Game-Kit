using System.IO;
using UnityEngine;

public class JsonSaveSystem : ISaveSystem
{
    private readonly string _filePath;

    public JsonSaveSystem()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        _filePath = Path.Combine(Application.persistentDataPath, "Save.json");
#else
        _filePath = Path.Combine(Application.dataPath, "Save.json");
#endif
    }

    public void Save(SaveData data)
    {
        File.WriteAllText(_filePath, JsonUtility.ToJson(data));
    }

    public SaveData Load()
    {
        if (File.Exists(_filePath))
        {
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(_filePath));
        }
        return new SaveData();
    }
}