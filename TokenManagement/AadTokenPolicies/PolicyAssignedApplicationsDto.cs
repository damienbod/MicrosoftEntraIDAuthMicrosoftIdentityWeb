namespace TokenManagement
{
    public class PolicyAssignedApplicationsDto
    {
        public string Id { get; set; }
        public string AppId { get; set; }
        public string DisplayName { get; set; }

        /// <summary>
        /// only "AzureADMyOrg" can be assigned
        /// </summary>
        public string SignInAudience { get; set; } 
    }
}
