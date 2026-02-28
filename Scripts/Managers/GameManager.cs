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
    }

    public void ReturnToLevelsMenu(bool topMenu = false)
    {
        if (topMenu) UIManager.Instance.levelsMenu.ShowLevelsList(true);
        UIManager.Instance.levelsMenu.Visible = true;
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

    public async Task<List<Level>> BrowseLevelsFromAPI()
    {
        //request levels json
        try
        {
            //send request to server for level data
            var response = await client.GetAsync($"https://platformed.jmeow.net/api/browse");
            if (!response.IsSuccessStatusCode) return null;
            //parse json
            return await JsonSerializer.DeserializeAsync<List<Level>>(await response.Content.ReadAsStreamAsync());
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Level>> GetLevelsFromFolder(string folder)
    {
        List<Level> levels = [];

        foreach (string path in Directory.GetFiles(folder))
        {
            if (Path.GetExtension(path) != ".json") continue;

            GD.Print($"found level {path}");

            //read and deserialize the level json
            Level level = await JsonSerializer.DeserializeAsync<Level>(File.OpenRead(path));


            GD.Print($"level name is {level.Name}");

            levels.Add(level);
        }

        return levels;
    }
}
