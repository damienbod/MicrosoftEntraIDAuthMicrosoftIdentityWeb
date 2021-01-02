using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TokenManagement.Pages.AadTokenPolicies
{
    public class IndexModel : PageModel
    {
        private readonly TokenLifetimePolicyGraphApiService _tokenLifetimePolicyGraphApiService;

        public IndexModel(TokenLifetimePolicyGraphApiService tokenLifetimePolicyGraphApiService)
        {
            _tokenLifetimePolicyGraphApiService = tokenLifetimePolicyGraphApiService;
        }

        public List<TokenLifetimePolicyDto> TokenLifetimePolicyDto { get; set; }

        public async Task OnGetAsync()
        {
            var policies = await _tokenLifetimePolicyGraphApiService.GetPolicies();
            TokenLifetimePolicyDto = policies.CurrentPage.Select(policy => new TokenLifetimePolicyDto
            {
                Definition = policy.Definition.FirstOrDefault(),
                DisplayName = policy.DisplayName,
                IsOrganizationDefault = policy.IsOrganizationDefault.GetValueOrDefault(),
                Id = policy.Id
            }).ToList();
        }
    }
}
