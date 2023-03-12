using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace TokenManagement.Pages.AadTokenPolicies;

[AuthorizeForScopes(Scopes = new string[] { "Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration" })]
public class DeleteModel : PageModel
{
    private readonly TokenLifetimePolicyGraphApiService _tokenLifetimePolicyGraphApiService;

    public DeleteModel(TokenLifetimePolicyGraphApiService tokenLifetimePolicyGraphApiService)
    {
        _tokenLifetimePolicyGraphApiService = tokenLifetimePolicyGraphApiService;
    }

    [BindProperty]
    public TokenLifetimePolicyDto TokenLifetimePolicyDto { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var policy = await _tokenLifetimePolicyGraphApiService.GetPolicy(id);
        TokenLifetimePolicyDto = new TokenLifetimePolicyDto
        {
            Definition = policy.Definition.FirstOrDefault(),
            DisplayName = policy.DisplayName,
            IsOrganizationDefault = policy.IsOrganizationDefault.GetValueOrDefault(),
            Id = policy.Id
        };

        if (TokenLifetimePolicyDto == null)
        {
            return NotFound();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var policy = await _tokenLifetimePolicyGraphApiService.GetPolicy(id);

        if (policy != null)
        {
            await _tokenLifetimePolicyGraphApiService.DeletePolicy(policy.Id);
        }

        return RedirectToPage("./Index");
    }
}