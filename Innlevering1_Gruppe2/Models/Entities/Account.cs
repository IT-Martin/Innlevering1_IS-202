namespace Innlevering1_Gruppe2.Models.Entities
{
    public class Account
    {

        public int account_id { get; set; }

        public string account_type { get; set; } = "Private";
        public string email { get; set; } = string.Empty;
        public string contact_name { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;

        public string password_hash { get; set; } = string.Empty;

        public ICollection<Resource> Resources { get; set; } = new List<Resource>();

    }
}
