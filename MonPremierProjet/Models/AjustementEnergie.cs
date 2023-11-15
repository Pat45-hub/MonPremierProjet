// fichier AjustementEnergie.cs

namespace MonPremierProjet.Models
{
    public class AjustementEnergie
    {
        public int Id { get; set; }
        public string NomAjustement { get; set; } = string.Empty;
        public decimal Valeur { get; set; }
    }
}
