namespace OrbitToDo;

// Singleton session — holds the signed-in user returned by the API
public static class AppSession
{
    public static UserData CurrentUser { get; set; }
}
