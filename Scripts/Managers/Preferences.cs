using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public partial class Preferences : Node
{
    public bool IsLevelsFolderSet()
    {
        if (GetLevelsFolder() is null) return false;
        else return true;
    }

    public void SetLevelsFolder(string folder)
    {
        ConfigFile config = new();

        config.Load("user://config.cfg");

        config.SetValue("Preferences", "levels_folder", folder);

        config.Save("user://config.cfg");
    }

    public string GetLevelsFolder()
    {
        string folder = (string)GetPreference("levels_folder");

        //catch if folder is an empty string
        if (folder is null) return null;

        //catch if folder is not a valid path to a real folder
        try
        {
            Path.GetDirectoryName(folder);
        }
        catch
        {
            return null;
        }

        return folder;
    }

    public async Task<List<Level>> GetLevelsFromFolder()
    {
        if (!IsLevelsFolderSet()) return null;

        List<Level> levels = [];

        foreach (string path in Directory.GetFiles(GetLevelsFolder()))
        {
            if (Path.GetExtension(path) != ".json") continue;

            //read and deserialize the level json
            Level level = await JsonSerializer.DeserializeAsync<Level>(File.OpenRead(path));

            levels.Add(level);
        }

        return levels;
    }

    public async Task SaveLevelAsFile(Level level, string path)
    {
        var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, level);
        await stream.DisposeAsync();


    }

    public void SetPreference(string key, Variant value)
    {
        ConfigFile config = new();

        config.Load("user://config.cfg");

        config.SetValue("Preferences", key, value);

        config.Save("user://config.cfg");
    }

    public Variant? GetPreference(string key)
    {
        ConfigFile config = new();

        Error error = config.Load("user://config.cfg");

        //catch if the config file didn't get fetched
        if (error != Error.Ok) return null;

        if (config.HasSectionKey("Preferences", key))
        {
            return config.GetValue("Preferences", key);
        }
        else return null;
    }
}
