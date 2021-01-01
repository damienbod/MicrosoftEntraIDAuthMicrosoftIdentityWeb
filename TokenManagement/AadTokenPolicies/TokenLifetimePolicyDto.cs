using System.Collections.Generic;

namespace TokenManagement
{
    public class TokenLifetimePolicyDto
    {
        public string Id { get; set; }
        public string Definition { get; set; }
        public string DisplayName { get; set; }
        public bool IsOrganizationDefault { get; set; }
        public string Description{ get; set; }
}
}
