using System.ComponentModel.DataAnnotations;

namespace LaciSynchroni.Shared.Models;

public class BannedRegistrations
{
    [Key]
    [MaxLength(100)]
    public string DiscordIdOrLodestoneAuth { get; set; }
}
