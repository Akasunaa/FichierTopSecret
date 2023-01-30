using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private const string AudioDataFileName = "/audio_settings.tnr";
    private const string ScreenDataFileName = "/screen_settings.tnr";

    public static void SaveAudioSettings(AudioMixerManager audioMixerManager)
    {
        BinaryFormatter formatter = new();
        var path = Application.persistentDataPath + AudioDataFileName;
        
        FileStream stream;
        try
        {
            stream = new FileStream(path, FileMode.Create);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] {e}");
            return;
        }
        
        var data = new AudioSettingsData(audioMixerManager);
        
        try
        {
            formatter.Serialize(stream, data);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] {e}");
            stream.Close();
            return;
        }
        
        stream.Close();
    }
    public static AudioSettingsData LoadAudioSettingsData()
    {
        var path = Application.persistentDataPath + AudioDataFileName;
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] Save file not found : {path}");
            return null;
        }

        BinaryFormatter formatter = new();
        FileStream stream;
        try 
        {
            stream = new FileStream(path, FileMode.Open);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] {e}");
            return null;
        }

        AudioSettingsData data;
        try
        {
            data = formatter.Deserialize(stream) as AudioSettingsData;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] {e}");
            stream.Close();
            return null;
        }
        
        stream.Close();
        return data;
    }

    public static void SaveScreenSettingsData(int width, int height, int refreshRate)
    {
        BinaryFormatter formatter = new();
        var path = Application.persistentDataPath + ScreenDataFileName;
        
        FileStream stream;
        try
        {
            stream = new FileStream(path, FileMode.Create);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] {e}");
            return;
        }

        var data = new ScreenSettingsData(width, height, refreshRate);
        
        try
        {
            formatter.Serialize(stream, data);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] {e}");
            stream.Close();
            return;
        }
        
        stream.Close();
    }

    public static ScreenSettingsData LoadScreenSettingsData()
    {
        var path = Application.persistentDataPath + ScreenDataFileName;
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] Save file not found : {path}");
            return null;
        }

        BinaryFormatter formatter = new();
        FileStream stream;
        try
        {
            stream = new FileStream(path, FileMode.Open);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] {e}");
            return null;
        }

        ScreenSettingsData data;
        try
        {
            data = formatter.Deserialize(stream) as ScreenSettingsData;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] {e}");
            stream.Close();
            return null;
        }
        
        stream.Close();
        return data;
    }
}