using MonPremierProjet.Models;
using System.Linq.Expressions;

namespace MonPremierProjet.Data
{
    public interface IAjustementEnergieRepository
    {
        Task<IEnumerable<AjustementEnergie>> FindAll(Expression<Func<AjustementEnergie, bool>>? filter = null);
        Task<AjustementEnergie?> FindOne(Expression<Func<AjustementEnergie, bool>>? filter = null);
        // Permet de faire un where sur la table AjustementEnergie
    }
}
