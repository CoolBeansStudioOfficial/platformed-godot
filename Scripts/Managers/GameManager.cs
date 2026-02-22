using Godot;
using System.Collections.Generic;
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

    public void ReturnToLevelView(bool explore = false)
    {
        if (explore) UIManager.Instance.levelsMenu.ViewExplore();
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
}
