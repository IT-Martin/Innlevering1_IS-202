namespace Reachout.Models.Entities
{
    public class Account
    {
        public string security_stamp { get; set; } = Guid.NewGuid().ToString();
        public int failed_attempts { get; set; }
        public DateTime? locked_until { get; set; }
        public bool access_revoked { get; set; }
        public bool email_confirmed { get; set; }
        public int account_id { get; set; }
        public string account_type { get; set; } = "Private";
        public string email { get; set; } = string.Empty;
        public string contact_name { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string password_hash { get; set; } = string.Empty;
    }
}
