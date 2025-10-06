using Entities;
using Entities.Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Infraestructure.Converters;
using System;

namespace Infraestructure.Configuration
{
    public class ContextBase : IdentityDbContext<Usuario>
    {
        public ContextBase(DbContextOptions options) : base(options) { }
        
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Contato> Contatos { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveConversion<MultiDbDateTimeConverter>();
            configurationBuilder.Properties<DateTime?>().HaveConversion<MultiDbNullableDateTimeConverter>();
            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.NavigationBaseIncludeIgnored));
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.InvalidIncludePathError));
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var roles = Enum.GetValues(typeof(RoleEnum))
                .Cast<RoleEnum>()
                .Select(e => new IdentityRole
                {
                    Id = ((int)e).ToString(),
                    Name = e.ToString(),
                    NormalizedName = e.ToString().ToUpperInvariant(),
                    ConcurrencyStamp = e.ToString()
                });

            builder.Entity<IdentityRole>().HasData(roles);
            builder.Entity<Usuario>().ToTable("AspNetUsers").HasKey(t => t.Id);

            builder.Entity<Contato>(entity =>
            {
                entity.ToTable("contato");
                entity.HasKey(e => e.Codg);
                entity.Property(e => e.Nome).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.Telefone).HasMaxLength(20);
            });

            builder.Entity<Agendamento>(entity =>
            {
                entity.ToTable("agendamento");
                entity.HasKey(e => e.Codg);
                entity.Property(e => e.Dscr).HasMaxLength(200);
                entity.Property(e => e.DataHora).IsRequired();

                // Relacionamento 1:N (um Contato possui vários Agendamentos)
                entity
                    .HasOne(a => a.Contato)
                    .WithMany(c => c.Agendamentos)
                    .HasForeignKey(a => a.ContatoId)
                    .OnDelete(DeleteBehavior.Cascade);
                // Removido índice único anterior (EF criará índice não único automaticamente se necessário)
            });
        }
    }
}
