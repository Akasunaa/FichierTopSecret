using System.IO;
using System.Runtime.CompilerServices;
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
        var stream = new FileStream(path, FileMode.Create);

        var data = new AudioSettingsData(audioMixerManager);
        
        formatter.Serialize(stream, data);
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
        var stream = new FileStream(path, FileMode.Open);
        var data = formatter.Deserialize(stream) as AudioSettingsData;
        stream.Close();
        
        return data;
    }

    public static void SaveScreenSettingsData(int width, int height, int refreshRate)
    {
        BinaryFormatter formatter = new();
        var path = Application.persistentDataPath + ScreenDataFileName;
        var stream = new FileStream(path, FileMode.Create);

        var data = new ScreenSettingsData(width, height, refreshRate);
        
        formatter.Serialize(stream, data);
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
        var stream = new FileStream(path, FileMode.Open);
        var data = formatter.Deserialize(stream) as ScreenSettingsData;
        stream.Close();
        
        return data;
    }
}