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
    [Export] public Preferences preferences;

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
    public async Task PlayLevel(Level level)
    {
        LevelManager.Instance.GenerateLevel(level);

        UIManager.Instance.levelsMenu.Visible = false;
        UIManager.Instance.editor.Visible = false;
    }

    //play level from the web using its level id
    public async Task PlayLevel(int id)
    {
        //fetch level
        Level level = await GetLevelFromAPI(id);

        //if level fetch failed, bail out
        if (level == null) return;

        LevelManager.Instance.GenerateLevel(level);

        UIManager.Instance.levelsMenu.Visible = false;
        UIManager.Instance.editor.Visible = false;
    }

    public async Task RemixLevel(Level level)
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
        message.Headers.Add("Cookie", $"session-id={(string)preferences.GetPreference("session_id")}; token={(string)preferences.GetPreference("token")}");

        var response = await client.SendAsync(message);

        if (response.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<List<Level>>(await response.Content.ReadAsStreamAsync());
        }
        else
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

    public async Task UploadLevel(Level level)
    {
        if (!IsLoggedIn()) return;

        HttpRequestMessage message = new(HttpMethod.Post, "https://platformed.jmeow.net/api/upload");
        message.Headers.Add("Cookie", $"session-id={(string)preferences.GetPreference("session_id")}; token={(string)preferences.GetPreference("token")}");
        message.Content = new StringContent(JsonSerializer.Serialize(level));

        var response = await client.SendAsync(message);

        if (response.IsSuccessStatusCode) UIManager.Instance.PopupNotification("Level uploaded successfully");
        else UIManager.Instance.PopupNotification("Level uploaded failed. If you are connected to the internet, the servers may be down.");
    }

    public async Task EditLevel(Level level, int id)
    {
        if (!IsLoggedIn()) return;

        HttpRequestMessage message = new(HttpMethod.Patch, "https://platformed.jmeow.net/api/edit");
        level.Id = id;
        message.Headers.Add("Cookie", $"session-id={(string)preferences.GetPreference("session_id")}; token={(string)preferences.GetPreference("token")}"); 
        message.Content = new StringContent(JsonSerializer.Serialize(new LevelUpdate()
        {
            LevelId = id,
            Data = level.Data,
            Width = level.Width,
            Height = level.Height
        }));

        var response = await client.SendAsync(message);

        if (response.IsSuccessStatusCode) UIManager.Instance.PopupNotification("Level updated successfully");
        else UIManager.Instance.PopupNotification("Level update failed. If you are connected to the internet, the servers may be down.");
    }

    public async Task EditLevelDetails(LevelDetails details)
    {
        if (!IsLoggedIn()) return;

        HttpRequestMessage message = new(HttpMethod.Patch, "https://platformed.jmeow.net/api/edit");
        message.Headers.Add("Cookie", $"session-id={(string)preferences.GetPreference("session_id")}; token={(string)preferences.GetPreference("token")}");
        message.Content = new StringContent(JsonSerializer.Serialize(details));

        var response = await client.SendAsync(message);

        if (response.IsSuccessStatusCode) UIManager.Instance.PopupNotification("Level details updated successfully");
        else UIManager.Instance.PopupNotification("Details update failed. If you are connected to the internet, the servers may be down.");
    }

    public async Task<bool> DeleteLevel(int id)
    {
        if (!IsLoggedIn()) return false;

        HttpRequestMessage message = new(HttpMethod.Delete, "https://platformed.jmeow.net/api/delete");
        message.Headers.Add("Cookie", $"session-id={(string)preferences.GetPreference("session_id")}; token={(string)preferences.GetPreference("token")}");
        message.Content = new StringContent(JsonSerializer.Serialize(new LevelDelete()
        {
            LevelId = id
        }));

        GD.Print(JsonSerializer.Serialize(new LevelDelete()
        {
            LevelId = id
        }));

        var response = await client.SendAsync(message);
        GD.Print(await response.Content.ReadAsStringAsync());

        if (response.IsSuccessStatusCode)
        {
            UIManager.Instance.PopupNotification("Level deleted successfully");
            return true;
        }
        else
        {
            UIManager.Instance.PopupNotification("Level deletion failed. If you are connected to the internet, the servers may be down.");
            return false;
        }

    }

    public bool IsLoggedIn()
    {
        try
        {
            return (bool)preferences.GetPreference("logged_in");
        }
        catch
        {
            return false;
        }
    }

    public class LoginCredentials
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class Me
    {
        [JsonPropertyName("user")]
        public int UserId { get; set; }

        [JsonPropertyName("theme")]
        public string Theme { get; set; }
    }

    public async Task<bool> Login(LoginCredentials credentials)
    {
        var headers = new StringContent(JsonSerializer.Serialize(credentials));
        var response = await client.PostAsync("https://platformed.jmeow.net/api/login", headers);

        if (response.IsSuccessStatusCode)
        {
            //save user credentials
            preferences.SetPreference("username", credentials.Username);

            var cookies = GetCookies(response);
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "token") preferences.SetPreference("token", cookie.Value);
                if (cookie.Name == "session-id") preferences.SetPreference("session_id", cookie.Value);
            }

            preferences.SetPreference("logged_in", true);

            Me me = await GetMe();
            if (me is not null) preferences.SetPreference("user_id", me.UserId);
            
            return true;
        }
        else return false;
    }

    public void Logout()
    {
        preferences.SetPreference("username", default);
        preferences.SetPreference("token", default);
        preferences.SetPreference("session_id", default);
        preferences.SetPreference("user_id", default);
        preferences.SetPreference("logged_in", false);
    }

    public async Task<Me> GetMe()
    {
        if (!IsLoggedIn()) return null;

        HttpRequestMessage message = new(HttpMethod.Get, "https://platformed.jmeow.net/api/me");
        message.Headers.Add("Cookie", $"session-id={(string)preferences.GetPreference("session_id")}; token={(string)preferences.GetPreference("token")}");

        var response = await client.SendAsync(message);

        if (response.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<Me>(await response.Content.ReadAsStreamAsync());
        }
        else return null;

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
}
