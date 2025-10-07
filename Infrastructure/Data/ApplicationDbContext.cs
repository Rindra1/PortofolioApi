using Microsoft.EntityFrameworkCore;
using PortofolioApi.Domain.Entities;


namespace PortofolioApi.Infrastructure.Data;
//Generer la base avec Entity
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserLogin> UserLogin { get; set; } = default!;
    public DbSet<Utilisateur> Utilisateur { get; set; } = default!;

    public DbSet<Projet> Projet { get; set; } = default!;

    public DbSet<Lien> Lien { get; set; } = default!;

    public DbSet<Contact> Contact { get; set; } = default!;
    public DbSet<Experience> Experience { get; set; } = default!;
    public DbSet<Competence> Competence{get;set;} = default!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Utilisateur>()
            .HasKey(u => u.IdUser);

        modelBuilder.Entity<Projet>()
            .HasKey(u => u.IdProjet);

        modelBuilder.Entity<Contact>()
            .HasKey(p => p.IdContact);

        modelBuilder.Entity<Experience>()
            .HasKey(p => p.IdExperience);
        
        modelBuilder.Entity<Competence>()
            .HasKey(c=>c.IdCompetence);

        modelBuilder.Entity<Lien>()
            .HasKey(l => l.IdLien);
        modelBuilder.Entity<UserLogin>()
            .HasKey(u => u.IdUserLogin);

         // Relation one-to-one Utilisateur <-> UserLogin
        modelBuilder.Entity<Utilisateur>()
            .HasOne(u => u.UserLogin)
            .WithOne(ul => ul.Utilisateur)
            .HasForeignKey<Utilisateur>(u => u.IdUserLogin)
            .IsRequired(false); // ou true si tu veux obliger la relation

    
        // Utilisateur <-> Projet (1-N)
        modelBuilder.Entity<Projet>()
            .HasOne(p => p.Utilisateur)
            .WithMany(u => u.Projets)
            .HasForeignKey(p => p.UtilisateurId)
        .OnDelete(DeleteBehavior.Cascade);

        //Projet <-> Lien (1-N)
        modelBuilder.Entity<Lien>()
            .HasOne(p => p.Projet)
            .WithMany(u => u.Liens)
            .HasForeignKey(p => p.IdProjet)
            .IsRequired(false)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contact>()
            .HasOne(p=>p.Utilisateur) 
            .WithMany(m=>m.Contacts)
            .HasForeignKey(fk=>fk.IdUser)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Experience>()
            .HasOne(p=>p.Utilisateur) 
            .WithMany(m=>m.Experiences)
            .HasForeignKey(fk=>fk.IdUser)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Competence>()
            .HasOne(p=>p.Utilisateur) 
            .WithMany(m=>m.Competences)
            .HasForeignKey(fk=>fk.IdUser)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

