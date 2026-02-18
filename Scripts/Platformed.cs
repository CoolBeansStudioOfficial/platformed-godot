using Godot;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HttpClient = System.Net.Http.HttpClient;

public class Level
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("data")]
    public Data Data { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("owner")]
    public int Owner { get; set; }

    [JsonPropertyName("tags")]
    public List<object> Tags { get; set; }

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; }

    [JsonPropertyName("approvals")]
    public int Approvals { get; set; }

    [JsonPropertyName("disapprovals")]
    public int Disapprovals { get; set; }

    [JsonPropertyName("approval_percentage")]
    public int ApprovalPercentage { get; set; }

    [JsonPropertyName("total_plays")]
    public int TotalPlays { get; set; }

    [JsonPropertyName("finished_plays")]
    public int FinishedPlays { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("level_style")]
    public string LevelStyle { get; set; }
}

public class Data
{
    [JsonPropertyName("zoom")]
    public int Zoom { get; set; }

    [JsonPropertyName("spawn")]
    public Spawn Spawn { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("layers")]
    public List<Layer> Layers { get; set; }

    [JsonPropertyName("wallJump")]
    public string WallJump { get; set; }

    [JsonPropertyName("xInertia")]
    public double XInertia { get; set; }

    [JsonPropertyName("yInertia")]
    public int YInertia { get; set; }

    [JsonPropertyName("jumpWidth")]
    public int JumpWidth { get; set; }

    [JsonPropertyName("jumpHeight")]
    public double JumpHeight { get; set; }

    [JsonPropertyName("tilesetPath")]
    public string TilesetPath { get; set; }

    [JsonPropertyName("bouncePadHeight")]
    public int BouncePadHeight { get; set; }
}

public class Layer
{
    [JsonPropertyName("data")]
    public List<JsonElement> Data { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class Spawn
{
    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }
}

public partial class Platformed : Node2D
{
    [Export] PackedScene playerScene;
    [Export] PackedScene groundBlock;

    HttpClient client;
    Level currentLevel;

	public override void _Ready()
	{
		client = new();
		currentLevel = GetLevelFromAPI(22).Result;

        GenerateLevel(currentLevel);

    }

	async Task<Level> GetLevelFromAPI(int id)
	{
		//request level json
		var response = await client.GetAsync($"https://platformed.jmeow.net/api/level?levelId={id}");
		GD.Print(await response.Content.ReadAsStringAsync());

        //parse json
        return await JsonSerializer.DeserializeAsync<Level>(await response.Content.ReadAsStreamAsync());

        

        GD.Print("width is " + level.Width);

        int rowCount = 0;
        foreach (var row in map)
        {
            int i = 0;
            foreach (var tile in row)
            {
                if (tile == 0)
                {
                    //empty tile
                    GD.Print("empty tile");
                }
                else
                {
                    GD.Print("should be spawning tile");

                    Node2D block = (Node2D)groundBlock.Instantiate();
                    AddChild(block);
                    block.Position = new(i * 16, rowCount * 16);
                }

                i++;
            }

            rowCount++;
        }

        //spawn player
        Node2D player = (Node2D)playerScene.Instantiate();
        AddChild(player);
        player.Position = new(level.Data.Spawn.X * 16, level.Data.Spawn.Y * 16);
    }

    void GenerateLevel(Level level)
    {
        var tiles = DecodeRLE(level.Data.Layers[0].Data, level.Width);
    }

    //takes list of rows of tiles and returns same list but with info about each tile
    List<List<TileInfo>> CreateTilemap(List<List<int>> tiles)
    {
        List<List<TileInfo>> tilemap = [];
        foreach (var row in tiles)
        {

        }
    }

    //takes encoded map data and converts it to a list of rows of tiles
    List<List<int>> DecodeRLE(List<JsonElement> rle, int width)
    {
        List<List<int>> map = [];

        List<int> currentRow = [];
        foreach (var element in rle)
        {
            //element represents multiple of the same tile
            if (element.ValueKind == JsonValueKind.Array)
            {
                var tile = element[0].GetInt32();
                var count = element[1].GetInt32();

                for (int i = 0; i < count; i++)
                {
                    currentRow.Add(tile);

                    if (currentRow.Count == width)
                    {
                        GD.Print($"adding row of length {currentRow.Count}");
                        List<int> toAdd = [];
                        toAdd.AddRange(currentRow);
                        map.Add(toAdd);
                        currentRow.Clear();
                    }
                }
            }
            //element represents single tile
            else
            {
                currentRow.Add(element.GetInt32());

                if (currentRow.Count == width)
                {
                    List<int> toAdd = [];
                    toAdd.AddRange(currentRow);
                    map.Add(toAdd);
                    currentRow.Clear();
                }
            }
        }

        return map;
    }


}
