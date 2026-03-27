using Godot;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

    public async Task<List<Level>> GetMyLevelsFromAPI()
    {
        if (!IsLoggedIn()) return null;

        HttpRequestMessage message = new(HttpMethod.Get, "https://platformed.jmeow.net/api/myLevels");
        message.Headers.Add("Cookie", $"session-id={(string)GetPreference("session_id")}; token={(string)GetPreference("token")}");

        var response = await client.SendAsync(message);

        if (response.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<List<Level>>(await response.Content.ReadAsStreamAsync());
        }
        else
        {
            GD.Print(await response.Content.ReadAsStringAsync());
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

    public async void UploadLevel(Level level)
    {
        if (!IsLoggedIn()) return;

        HttpRequestMessage message = new(HttpMethod.Post, "https://platformed.jmeow.net/api/upload");
        message.Headers.Add("Cookie", $"session-id={(string)GetPreference("session_id")}; token={(string)GetPreference("token")}");
        message.Content = new StringContent(JsonSerializer.Serialize(level));

        var response = await client.SendAsync(message);

        if (response.IsSuccessStatusCode) UIManager.Instance.PopupNotification("Level uploaded successfully");
        else UIManager.Instance.PopupNotification("Level uploaded failed. If you are connected to the internet, the servers may be down.");
    }

    public bool IsLoggedIn()
    {
        return (bool)GetPreference("logged_in");
    }

    public class LoginCredentials
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public async Task<bool> Login(LoginCredentials credentials)
    {
        var headers = new StringContent(JsonSerializer.Serialize(credentials));
        var response = await client.PostAsync("https://platformed.jmeow.net/api/login", headers);

        if (response.IsSuccessStatusCode)
        {
            //save user credentials
            SetPreference("username", credentials.Username);

            var cookies = GetCookies(response);
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "token") SetPreference("token", cookie.Value);
                if (cookie.Name == "session-id") SetPreference("session_id", cookie.Value);
            }

            SetPreference("logged_in", true);
            return true;
        }
        else return false;
    }

    public void Logout()
    {
        SetPreference("username", default);
        SetPreference("token", default);
        SetPreference("session_id", default);
        SetPreference("logged_in", false);
    }

    //i found this on stack overflow lol
    public static CookieCollection GetCookies(HttpResponseMessage message)
    {
        var cookies = new CookieCollection();
        if (message == null)
            return cookies;
        var setCookie = Enumerable.Empty<string>();
        if (message.Headers.TryGetValues("Set-Cookie", out setCookie))
        {
            foreach (var cookieStr in setCookie)
            {
                foreach (var cookieToken in cookieStr.Split(';'))
                {
                    var name = cookieToken.Trim();
                    var value = "";
                    if (cookieToken.Contains('='))
                    {
                        var keyValueTokens = cookieToken.Split('=');
                        name = (keyValueTokens[0]).Trim();
                        if (keyValueTokens.Length > 1 && !string.IsNullOrEmpty(keyValueTokens[1]))
                        {
                            value = (keyValueTokens[1]).Trim();
                        }
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        var cookie = new Cookie(name, value);
                        cookies.Add(cookie);
                    }
                }
            }
        }
        return cookies;
    }

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

        try
        {
            return config.GetValue("Preferences", key);
        }
        catch
        {
            return null;
        }
    }
}
