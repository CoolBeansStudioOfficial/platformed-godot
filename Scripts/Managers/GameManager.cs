using Godot;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    
    public async Task<bool> PlayLevel(int id)
    {
        Level level = await GetLevelFromAPI(id);

        //if level fetch failed, bail out
        if (level == null) return false;

        LevelManager.Instance.GenerateLevel(level);

        //spawn player
        player = (Node2D)playerScene.Instantiate();
        AddChild(player);
        player.Position = new(currentLevel.Data.Spawn.X * 16, currentLevel.Data.Spawn.Y * 16);

        return true;
    }

    public void ResetLevel(bool playerDied)
    {
        if (playerDied)
        {
            //play death sound effect
        }

        player.Position = new(currentLevel.Data.Spawn.X * 16, currentLevel.Data.Spawn.Y * 16);
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

    
}
