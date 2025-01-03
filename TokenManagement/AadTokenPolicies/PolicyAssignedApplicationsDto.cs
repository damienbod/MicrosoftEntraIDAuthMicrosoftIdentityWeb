namespace TokenManagement.AadTokenPolicies;

public class PolicyAssignedApplicationsDto
{
    public string Id { get; set; } = string.Empty;
    public string AppId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// only "AzureADMyOrg" and "AzureADMultipleOrgs" can be assigned a policy
    /// </summary>
    public string SignInAudience { get; set; } = string.Empty;

    public string PolicyAssigned { get; set; } = string.Empty;
}
