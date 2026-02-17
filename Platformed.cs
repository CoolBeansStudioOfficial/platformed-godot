using Godot;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
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

	public override void _Ready()
	{
		client = new();
		GetLevel();
	
	}

	async void GetLevel()
	{
		//request level json
		var response = await client.GetAsync("https://platformed.jmeow.net/api/level?levelId=1");
		GD.Print(await response.Content.ReadAsStringAsync());

        //parse json
        var level = await JsonSerializer.DeserializeAsync<Level>(await response.Content.ReadAsStreamAsync());

        var tiles = DecodeRLE(level.Data.Layers[0].Data);

        GD.Print("");

        int tileCount = 0;
        
        for (int i = 0; i < level.Height; i++)
        {
            string row = "";
            for (int j = 0; j < level.Width; j++)
            {
                if (tiles[tileCount] == 0)
                {


                    row += "■";
                }
                else
                {
                    Node2D block = (Node2D)groundBlock.Instantiate();
                    AddChild(block);
                    block.Position = new(j * 16, i * 16);

                    row += "□";
                }

                tileCount++;
            }

            GD.Print(row);
        }

        Node2D player = (Node2D)playerScene.Instantiate();
        AddChild(player);
        player.Position = new(level.Data.Spawn.X * 16, level.Data.Spawn.Y * 16);


    }

    List<int> DecodeRLE(List<JsonElement> rle)
    {
        List<int> tiles = [];

        foreach (var element in rle)
        {
            if (element.ValueKind == JsonValueKind.Array)
            {
                var tile = element[0].GetInt32();
                var count = element[1].GetInt32();

                for (int i = 0; i < count; i++) tiles.Add(tile);
            }
            else tiles.Add(element.GetInt32());
        }

        return tiles;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
