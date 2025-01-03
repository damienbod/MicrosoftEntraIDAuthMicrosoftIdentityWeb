using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using TokenManagement.AadTokenPolicies;

namespace TokenManagement.Pages.AadTokenPolicies;

[AuthorizeForScopes(Scopes = new string[] { "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration" })]
public class IndexModel : PageModel
{
    private readonly TokenLifetimePolicyGraphApiService _tokenLifetimePolicyGraphApiService;

    public IndexModel(TokenLifetimePolicyGraphApiService tokenLifetimePolicyGraphApiService)
    {
        _tokenLifetimePolicyGraphApiService = tokenLifetimePolicyGraphApiService;
    }

    public List<TokenLifetimePolicyDto> TokenLifetimePolicyDto { get; set; } = [];

    public async Task OnGetAsync()
    {
        var policies = await _tokenLifetimePolicyGraphApiService.GetPolicies();
        TokenLifetimePolicyDto = policies.CurrentPage.Select(policy => new TokenLifetimePolicyDto
        {
            Definition = policy.Definition.FirstOrDefault()!,
            DisplayName = policy.DisplayName,
            IsOrganizationDefault = policy.IsOrganizationDefault.GetValueOrDefault(),
            Id = policy.Id
        }).ToList();
    }
}