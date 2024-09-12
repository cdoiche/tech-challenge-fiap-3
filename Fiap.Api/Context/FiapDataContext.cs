using Fiap.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Infra.Context
{
    public class FiapDataContext : DbContext
    {
        public FiapDataContext(DbContextOptions<FiapDataContext> options) : base(options)
        {

        }

        public DbSet<Contato> Contatos { get; set; }
    }
}
