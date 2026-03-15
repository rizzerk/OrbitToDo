using System.Collections.Generic;

namespace OrbitToDo
{
    // Simple in-memory user store (no database required)
    public class UserClass
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }

    // Singleton session store
    public static class AppSession
    {
        public static UserClass CurrentUser { get; set; }
        public static List<ToDoClass> TodoList { get; set; } = new List<ToDoClass>();
        public static List<UserClass> RegisteredUsers { get; set; } = new List<UserClass>();
        private static int _nextId = 1;
        private static int _nextUserId = 1;

        public static int GetNextId() => _nextId++;
        public static int GetNextUserId() => _nextUserId++;
    }
}
