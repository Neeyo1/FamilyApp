using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>,
    IdentityUserToken<int>>(options)
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<UserAssignment> UserAssignments { get; set; }
    public DbSet<Reaction> Reactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //AppUser - AppRole
        builder.Entity<AppUser>()
            .HasMany(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
            .HasMany(x => x.UserRoles)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .IsRequired();

        //AppUser - Group (Members)
        builder.Entity<UserGroup>().HasKey(x => new {x.UserId, x.GroupId});

        builder.Entity<AppUser>()
            .HasMany(x => x.UserGroups)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.Entity<Group>()
            .HasMany(x => x.UserGroups)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId)
            .IsRequired();

        //AppUser - Group (Owner)
        builder.Entity<AppUser>()
            .HasMany(x => x.OwnerOf)
            .WithOne(x => x.Owner)
            .HasForeignKey(x => x.OwnerId)
            .IsRequired();

        //Group - Assignment
        builder.Entity<Group>()
            .HasMany(x => x.Assignments)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId)
            .IsRequired();

        //AppUser - Assignment(Members)
        builder.Entity<UserAssignment>().HasKey(x => new {x.UserId, x.AssignmentId});

        builder.Entity<AppUser>()
            .HasMany(x => x.UserAssignments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.Entity<Assignment>()
            .HasMany(x => x.UserAssignments)
            .WithOne(x => x.Assignment)
            .HasForeignKey(x => x.AssignmentId)
            .IsRequired();

        //AppUser - Assignment(Creator)
        builder.Entity<AppUser>()
            .HasMany(x => x.AssignmentsCreated)
            .WithOne(x => x.CreatedBy)
            .HasForeignKey(x => x.CreatedById)
            .IsRequired();

        //AppUser - Assignment (Reaction)
        builder.Entity<Reaction>().HasKey(x => new {x.UserId, x.AssignmentId, x.Name});

        builder.Entity<AppUser>()
            .HasMany(x => x.Reactions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.Entity<Assignment>()
            .HasMany(x => x.Reactions)
            .WithOne(x => x.Assignment)
            .HasForeignKey(x => x.AssignmentId)
            .IsRequired();
    }
}
