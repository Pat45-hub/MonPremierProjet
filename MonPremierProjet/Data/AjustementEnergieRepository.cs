using Microsoft.EntityFrameworkCore;
using MonPremierProjet.Models;
using System.Linq.Expressions;

namespace MonPremierProjet.Data
{
    public class AjustementEnergieRepository : IAjustementEnergieRepository
    {
        private readonly ScadaContext _context;
        private readonly DbSet<AjustementEnergie> _table;

        public AjustementEnergieRepository(ScadaContext context)
        {
            _context = context;
            _table = _context.Set<AjustementEnergie>();
        }

        public async Task<IEnumerable<AjustementEnergie>> FindAll(Expression<Func<AjustementEnergie, bool>>? filter = null) // Le ? permet de mettre filter à nullable
        {
            if (filter != null)
            {
                return await _table.Where(filter).ToListAsync(); //Si on fait un  async task on doit mettre await
            }
            return await _table.ToListAsync();
        }
        
        public async Task<AjustementEnergie?> FindOne(Expression<Func<AjustementEnergie, bool>>? filter = null) // Le ? permet de mettre filter à nullable
        {
            if (filter != null)
            {
                return await _context.AjustementEnergies.Where(filter).FirstOrDefaultAsync(); //Si on fait un  async task on doit mettre await
            }
            return await _context.AjustementEnergies.FirstOrDefaultAsync();
        }
    }
}
