namespace TechFixShopAPI.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password_Hash { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }

    }
}