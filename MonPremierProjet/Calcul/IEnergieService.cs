// fichier: EnergieService.cs
using MonPremierProjet.DTO;
using MonPremierProjet.Models;
using System.Runtime.CompilerServices;

namespace MonPremierProjet.Calcul
{
    public interface IEnergieService
    {


        Task<List<CoutEnergieDto>> GetCoutEnergies();





    }



}
