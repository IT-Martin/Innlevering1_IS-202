namespace Innlevering1_Gruppe2.Models
{

    //Inneholder navn og posisjon for en ressurs
    //Brukes av Controller og controller sender det videre til brukergrensesnittet.
    public class RessursViewModel
    {
        //Gir nye ressursobjekter en unik ID som brukes til å finne lagde objekter.
        public int Id { get; set; }
        public string RessursNavn { get; set; } = string.Empty;
        public string TypeIcon { get; set; } = "fa-solid fa-location-dot";

        public double Latitude { get; set; }

        public double Longitude { get; set; }



    }
}
