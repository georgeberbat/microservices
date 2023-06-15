using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dex.Ef.Contracts.Entities;
using Microsoft.EntityFrameworkCore;

namespace Profile.Dal.Model
{
    [Table("user")]
    [Index(nameof(CreatedUtc))]
    [Index(nameof(UpdatedUtc))]
    [Index(nameof(DeletedUtc))]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Phone), IsUnique = true)]
    public sealed class UserDb : ICreatedUtc, IUpdatedUtc, IDeletable
    {
        [Key] [Column("id")] public Guid Id { get; set; }

        [Column("created_utc")] public DateTime CreatedUtc { get; set; }

        [Column("deleted_utc")] public DateTime? DeletedUtc { get; set; }

        [Column("updated_utc")] public DateTime UpdatedUtc { get; set; }

        [Required] [Column("phone")] public string Phone { get; set; } = null!;

        [Column("email")] public string? Email { get; set; }

        [Column("email_confirmed")] public DateTime? EmailConfirmed { get; set; }

        [Required] [Column("password")] public string Password { get; set; } = null!;
    
        [Column("first_name")] public string? FirstName { get; set; }
        
        [Column("middleName")] public string? MiddleName { get; set; }

        [Column("last_name")] public string? LastName { get; set; }
    }
}