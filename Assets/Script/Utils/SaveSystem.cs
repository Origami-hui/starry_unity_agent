using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class SaveSystem
{


    public static void SavePersistentDataByJson(string saveFileName, object data)
    {
        var json = JsonConvert.SerializeObject(data);
        var path = Path.Combine(Application.persistentDataPath, saveFileName);


        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"成功保存数据至{path}");
        }
        catch (System.Exception exception)
        {
            Debug.LogError(exception.Message);
        }
    }

    public static void SavePersistentDataByString(string saveFileName, string data)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);

        try
        {
            File.WriteAllText(path, data);
            Debug.Log($"成功保存数据至{path}");
        }
        catch (System.Exception exception)
        {
            Debug.LogError(exception.Message);
        }
    }

    public static (T, bool) LoadFromJson<T>(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<T>(json);
            //Debug.Log($"成功读取数据：{json}");
            //Debug.Log(111);
            return (data, true);
        }
        else
        {
            //Debug.Log(222);
            return (default, false);
        }
    }

    public static bool FileExists(string FileName)
    {
        var path = Path.Combine(Application.persistentDataPath, FileName);
        return File.Exists(path);
    }

    //加载数据已存在
    public static T LoadDataFromJson<T>(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        var json = File.ReadAllText(path);
        var data = JsonConvert.DeserializeObject<T>(json);
        return data;
    }

    public static T DeserializeObjectFromJson<T>(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        var json = File.ReadAllText(path);
        if (json.Equals(""))
        {
            return default;
        }
        var data = JsonConvert.DeserializeObject<T>(json);
        return data;
    }

    public static void DeleteSaveFile(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        try
        {
            File.Delete(path);
        }
        catch (System.Exception exception)
        {
            Debug.LogError(exception.Message);
        }

    }
}
