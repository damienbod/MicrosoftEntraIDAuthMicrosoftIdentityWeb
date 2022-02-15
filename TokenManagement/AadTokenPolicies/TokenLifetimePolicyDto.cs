using System.ComponentModel.DataAnnotations;

namespace TokenManagement;

public class TokenLifetimePolicyDto
{
    public string Id { get; set; }
    [Required]
    public string Definition { get; set; }
    [Required]
    public string DisplayName { get; set; }
    public bool IsOrganizationDefault { get; set; }
}