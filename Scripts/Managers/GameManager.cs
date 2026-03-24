using Godot;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using HttpClient = System.Net.Http.HttpClient;

public partial class GameManager : Node
{
    HttpClient client;
    
    public static GameManager Instance { get; private set; }
    public override void _Ready()
    {
        //singleton
        Instance = this;

        //initialize http client
        client = new();
    }
    


    //play level from parsed json
    public async void PlayLevel(Level level)
    {
        LevelManager.Instance.GenerateLevel(level);

        UIManager.Instance.levelsMenu.Visible = false;
        UIManager.Instance.editor.Visible = false;
    }

    //play level from the web using its level id
    public async void PlayLevel(int id)
    {
        //fetch level
        Level level = await GetLevelFromAPI(id);

        //if level fetch failed, bail out
        if (level == null) return;

        LevelManager.Instance.GenerateLevel(level);

        UIManager.Instance.levelsMenu.Visible = false;
        UIManager.Instance.editor.Visible = false;
    }

    public async void RemixLevel(Level level)
    {
        UIManager.Instance.editor.ImportLevel(level);

        UIManager.Instance.editor.Visible = true;
        UIManager.Instance.levelsMenu.Visible = false;
    }

    public void ReturnToLevelsMenu(bool topMenu = false)
    {
        if (topMenu) UIManager.Instance.levelsMenu.ShowLevelsList(true);
        UIManager.Instance.returnToEditorButton.Visible = false;
        UIManager.Instance.levelsMenu.Visible = true;
        UIManager.Instance.editor.Visible = false;
        LevelManager.Instance.DestroyLevel();
    }

    public void ReturnToEditor()
    {
        UIManager.Instance.returnToEditorButton.Visible = false;
        UIManager.Instance.editor.Visible = true;
        LevelManager.Instance.DestroyLevel();
    }

    async Task<Level> GetLevelFromAPI(int id)
    {
        //request level json
        try
        {
            //send request to server for level data
            var response = await client.GetAsync($"https://platformed.jmeow.net/api/level?levelId={id}");
            if (!response.IsSuccessStatusCode) return null;
            //parse json
            return await JsonSerializer.DeserializeAsync<Level>(await response.Content.ReadAsStreamAsync());
        }
        catch
        {
            return null;
        }
    }

    public async Task<LevelList> BrowseLevelsFromAPI()
    {
        //request levels json
        try
        {
            //send request to server for level data
            var response = await client.GetAsync($"https://platformed.jmeow.net/api/browse");
            if (!response.IsSuccessStatusCode) return null;
            //parse json
            return await JsonSerializer.DeserializeAsync<LevelList>(await response.Content.ReadAsStreamAsync());
        }
        catch
        {
            return null;
        }
    }

    public bool IsLevelsFolderSet()
    {
        if (GetLevelsFolder() is null) return false;
        else return true;
    }

    public void SetLevelsFolder(string folder)
    {
        ConfigFile config = new();

        config.SetValue("Preferences", "levels_folder", folder);

        config.Save("user://config.cfg");
    }

    public string GetLevelsFolder()
    {
        ConfigFile config = new();

        Error error = config.Load("user://config.cfg");

        //catch if the config file didn't get fetched
        if (error != Error.Ok) return null;

        string folder = (string)config.GetValue("Preferences", "levels_folder");

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

    public async void SaveLevelAsFile(Level level, string path)
    {
        var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, level);
        stream.DisposeAsync();


    }

    public void SetPreference(string key, Variant value)
    {
        ConfigFile config = new();

        config.SetValue("Preferences", key, value);

        config.Save("user://config.cfg");
    }

    public Variant? GetPreference(string key)
    {
        ConfigFile config = new();

        Error error = config.Load("user://config.cfg");

        //catch if the config file didn't get fetched
        if (error != Error.Ok) return null;

        return config.GetValue("Preferences", key);
    }
}
