using Fiap.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Core.Context
{
    public class FiapDataContext : DbContext
    {
        public FiapDataContext(DbContextOptions<FiapDataContext> options) : base(options)
        {
            
        }

        public DbSet<Contato> Contatos { get; set; }
    }
}
