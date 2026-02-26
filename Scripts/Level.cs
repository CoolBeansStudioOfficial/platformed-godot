using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Level
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("data")]
    public Data Data { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

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

    [JsonPropertyName("owned")]
    public bool Owned { get; set; }
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

    [JsonPropertyName("triggers")]
    public List<TriggerParams> Triggers { get; set; }

    [JsonPropertyName("wallJump")]
    public string WallJump { get; set; }

    [JsonPropertyName("xInertia")]
    public double XInertia { get; set; }

    [JsonPropertyName("yInertia")]
    public double YInertia { get; set; }

    [JsonPropertyName("jumpWidth")]
    public double JumpWidth { get; set; }

    [JsonPropertyName("jumpHeight")]
    public double JumpHeight { get; set; }

    [JsonPropertyName("tilesetPath")]
    public string TilesetPath { get; set; }

    [JsonPropertyName("bouncePadHeight")]
    public double BouncePadHeight { get; set; }
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

public class TriggerParams
{
    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("execute")]
    public List<Execute> Execute { get; set; }
}

public class Execute
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("x")]
    public int? X { get; set; }

    [JsonPropertyName("y")]
    public int? Y { get; set; }

    [JsonPropertyName("block")]
    public int? Block { get; set; }
}
