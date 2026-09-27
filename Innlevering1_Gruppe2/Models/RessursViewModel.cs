namespace Innlevering1_Gruppe2.Models
{
    public class RessursViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RessursNavn { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }



    }
}
