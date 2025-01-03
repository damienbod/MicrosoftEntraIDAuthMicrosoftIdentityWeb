using System.ComponentModel.DataAnnotations;

namespace TokenManagement.AadTokenPolicies;

public class TokenLifetimePolicyDto
{
    public string Id { get; set; } = string.Empty;
    [Required]
    public string Definition { get; set; } = string.Empty;
    [Required]
    public string DisplayName { get; set; } = string.Empty;
    public bool IsOrganizationDefault { get; set; }
}