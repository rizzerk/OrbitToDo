using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OrbitToDo;

public static class ApiService
{
    private static readonly HttpClient _client = new HttpClient
    {
        BaseAddress = new Uri("https://todo-list.dcism.org")
    };

    // ──────────────────────────────────────────────
    // AUTH
    // ──────────────────────────────────────────────

    /// <summary>
    /// POST /signup_action.php
    /// </summary>
    public static async Task<ApiResult> SignUpAsync(
        string firstName, string lastName, string email,
        string password, string confirmPassword)
    {
        var body = new
        {
            first_name = firstName,
            last_name = lastName,
            email = email,
            password = password,
            confirm_password = confirmPassword
        };

        return await PostAsync("/signup_action.php", body);
    }

    /// <summary>
    /// GET /signin_action.php?email=...&password=...
    /// </summary>
    public static async Task<ApiResult<UserData>> SignInAsync(string email, string password)
    {
        string url = $"/signin_action.php?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";

        try
        {
            var response = await _client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            int status = node["status"].GetValue<int>();

            if (status == 200)
            {
                var data = node["data"];
                var user = new UserData
                {
                    id         = data["id"].GetValue<int>(),
                    fname      = data["fname"].GetValue<string>(),
                    lname      = data["lname"].GetValue<string>(),
                    email      = data["email"].GetValue<string>(),
                };
                return new ApiResult<UserData> { Success = true, Data = user, Message = "Success" };
            }
            else
            {
                return new ApiResult<UserData> { Success = false, Message = node["message"].GetValue<string>() };
            }
        }
        catch (Exception ex)
        {
            return new ApiResult<UserData> { Success = false, Message = $"Network error: {ex.Message}" };
        }
    }

    // ──────────────────────────────────────────────
    // TODO ITEMS
    // ──────────────────────────────────────────────

    /// <summary>
    /// GET /getItems_action.php?status=active|inactive&user_id=...
    /// </summary>
    public static async Task<ApiResult<List<ToDoClass>>> GetItemsAsync(string status, int userId)
    {
        string url = $"/getItems_action.php?status={status}&user_id={userId}";

        try
        {
            var response = await _client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            int statusCode = node["status"].GetValue<int>();

            if (statusCode == 200)
            {
                var items = new List<ToDoClass>();
                var data = node["data"].AsObject();

                foreach (var kvp in data)
                {
                    var obj = kvp.Value;
                    items.Add(new ToDoClass
                    {
                        item_id          = obj["item_id"].GetValue<int>(),
                        item_name        = obj["item_name"].GetValue<string>(),
                        item_description = obj["item_description"]?.GetValue<string>() ?? "",
                        status           = obj["status"].GetValue<string>(),
                        user_id          = obj["user_id"].GetValue<int>()
                    });
                }
                return new ApiResult<List<ToDoClass>> { Success = true, Data = items };
            }
            else
            {
                // 400 usually means no items found — return empty list, not an error
                return new ApiResult<List<ToDoClass>> { Success = true, Data = new List<ToDoClass>() };
            }
        }
        catch (Exception ex)
        {
            return new ApiResult<List<ToDoClass>> { Success = false, Message = $"Network error: {ex.Message}" };
        }
    }

    /// <summary>
    /// POST /addItem_action.php
    /// </summary>
    public static async Task<ApiResult<ToDoClass>> AddItemAsync(string name, string description, int userId)
    {
        var body = new
        {
            item_name        = name,
            item_description = description,
            user_id          = userId
        };

        try
        {
            var content  = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/addItem_action.php", content);
            string json  = await response.Content.ReadAsStringAsync();
            var node     = JsonNode.Parse(json);
            int status   = node["status"].GetValue<int>();

            if (status == 200)
            {
                var data = node["data"];
                var item = new ToDoClass
                {
                    item_id          = data["item_id"].GetValue<int>(),
                    item_name        = data["item_name"].GetValue<string>(),
                    item_description = data["item_description"]?.GetValue<string>() ?? "",
                    status           = data["status"].GetValue<string>(),
                    user_id          = data["user_id"].GetValue<int>()
                };
                return new ApiResult<ToDoClass> { Success = true, Data = item, Message = node["message"].GetValue<string>() };
            }
            else
            {
                return new ApiResult<ToDoClass> { Success = false, Message = node["message"]?.GetValue<string>() ?? "Failed to add item." };
            }
        }
        catch (Exception ex)
        {
            return new ApiResult<ToDoClass> { Success = false, Message = $"Network error: {ex.Message}" };
        }
    }

    /// <summary>
    /// PUT /editItem_action.php
    /// </summary>
    public static async Task<ApiResult> UpdateItemAsync(int itemId, string name, string description)
    {
        var body = new
        {
            item_id          = itemId,
            item_name        = name,
            item_description = description
        };

        return await PutAsync("/editItem_action.php", body);
    }

    /// <summary>
    /// PUT /statusItem_action.php  — "active" or "inactive"
    /// </summary>
    public static async Task<ApiResult> ChangeStatusAsync(int itemId, string status)
    {
        var body = new
        {
            item_id = itemId,
            status  = status
        };

        return await PutAsync("/statusItem_action.php", body);
    }

    /// <summary>
    /// DELETE /deleteItem_action.php?item_id=...
    /// </summary>
    public static async Task<ApiResult> DeleteItemAsync(int itemId)
    {
        try
        {
            var request  = new HttpRequestMessage(HttpMethod.Delete, $"/deleteItem_action.php?item_id={itemId}");
            var response = await _client.SendAsync(request);
            string json  = await response.Content.ReadAsStringAsync();
            var node     = JsonNode.Parse(json);
            int status   = node["status"].GetValue<int>();

            return status == 200
                ? new ApiResult { Success = true,  Message = node["message"].GetValue<string>() }
                : new ApiResult { Success = false, Message = node["message"].GetValue<string>() };
        }
        catch (Exception ex)
        {
            return new ApiResult { Success = false, Message = $"Network error: {ex.Message}" };
        }
    }

    // ──────────────────────────────────────────────
    // HELPERS
    // ──────────────────────────────────────────────

    private static async Task<ApiResult> PostAsync(string route, object body)
    {
        try
        {
            var content  = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(route, content);
            string json  = await response.Content.ReadAsStringAsync();
            var node     = JsonNode.Parse(json);
            int status   = node["status"].GetValue<int>();

            return status == 200
                ? new ApiResult { Success = true,  Message = node["message"].GetValue<string>() }
                : new ApiResult { Success = false, Message = node["message"].GetValue<string>() };
        }
        catch (Exception ex)
        {
            return new ApiResult { Success = false, Message = $"Network error: {ex.Message}" };
        }
    }

    private static async Task<ApiResult> PutAsync(string route, object body)
    {
        try
        {
            var content  = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var request  = new HttpRequestMessage(HttpMethod.Put, route) { Content = content };
            var response = await _client.SendAsync(request);
            string json  = await response.Content.ReadAsStringAsync();
            var node     = JsonNode.Parse(json);
            int status   = node["status"].GetValue<int>();

            return status == 200
                ? new ApiResult { Success = true,  Message = node["message"].GetValue<string>() }
                : new ApiResult { Success = false, Message = node["message"].GetValue<string>() };
        }
        catch (Exception ex)
        {
            return new ApiResult { Success = false, Message = $"Network error: {ex.Message}" };
        }
    }
}

// ──────────────────────────────────────────────
// Result wrapper classes
// ──────────────────────────────────────────────

public class ApiResult
{
    public bool   Success { get; set; }
    public string Message { get; set; }
}

public class ApiResult<T> : ApiResult
{
    public T Data { get; set; }
}

// ──────────────────────────────────────────────
// User data returned by Sign In
// ──────────────────────────────────────────────

public class UserData
{
    public int    id    { get; set; }
    public string fname { get; set; }
    public string lname { get; set; }
    public string email { get; set; }
}
