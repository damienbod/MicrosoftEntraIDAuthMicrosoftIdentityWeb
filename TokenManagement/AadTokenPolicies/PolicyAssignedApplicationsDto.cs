namespace TokenManagement;

public class PolicyAssignedApplicationsDto
{
    public string Id { get; set; }
    public string AppId { get; set; }
    public string DisplayName { get; set; }

    /// <summary>
    /// only "AzureADMyOrg" and "AzureADMultipleOrgs" can be assigned a policy
    /// </summary>
    public string SignInAudience { get; set; }

    public string PolicyAssigned { get; set; }
}
