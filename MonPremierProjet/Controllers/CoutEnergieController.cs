// fichier: CoutEnergiController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonPremierProjet.Calcul;
using MonPremierProjet.Data;
using MonPremierProjet.DTO;
using MonPremierProjet.Models;

namespace MonPremierProjet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoutEnergieController : ControllerBase
    {
        private readonly IEnergieService _service;

        public CoutEnergieController(IEnergieService service)
        {
           _service = service;
        }

        [HttpGet]


        public ActionResult<IEnumerable<CoutEnergieDto>> GetCoutEnergie()
        {
            
            return Ok(_service.GetCoutEnergies());

        }
    }
}
