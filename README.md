# Microsoft Entra ID (Azure AD) authentication, authorization with ASP.NET Core

[![.NET](https://github.com/damienbod/AzureADAuthRazorUiServiceApiCertificate/workflows/.NET/badge.svg)](https://github.com/damienbod/AzureADAuthRazorUiServiceApiCertificate/actions?query=workflow%3A.NET) 

# Blogs

- [Implement Microsoft Entra ID Client credentials flow using Client Certificates for service APIs](https://damienbod.com/2020/10/01/implement-azure-ad-client-credentials-flow-using-client-certificates-for-service-apis/)
- [Using Key Vault certificates with Microsoft.Identity.Web and ASP.NET Core applications](https://damienbod.com/2020/10/09/using-key-vault-certificates-with-microsoft-identity-web-and-asp-net-core-applications/)
- [Using encrypted access tokens in Azure with Microsoft.Identity.Web and Azure App registrations](https://damienbod.com/2020/10/22/using-encrypted-access-tokens-in-azure-with-microsoft-identity-web-and-azure-app-registrations/)
- [Implement a Web APP and an ASP.NET Core Secure API using Microsoft Entra ID which delegates to a second API](https://damienbod.com/2020/11/09/implement-a-web-app-and-an-asp-net-core-secure-api-using-azure-ad-which-delegates-to-second-api/)
- [Using multiple APIs in Angular and ASP.NET Core with Microsoft Entra ID authentication](https://damienbod.com/2020/12/08/using-multiple-apis-in-angular-and-asp-net-core-with-azure-ad-authentication/)
- [Using multiple APIs in Blazor with Microsoft Entra ID authentication](https://damienbod.com/2020/12/14/using-multiple-apis-in-blazor-with-azure-ad-authentication/)
- [Microsoft Entra ID Access Token Lifetime Policy Management in ASP.NET Core](https://damienbod.com/2021/01/05/azure-ad-access-token-lifetime-policy-management-in-asp-net-core/)
- [Implement OAUTH Device Code Flow with Microsoft Entra ID and ASP.NET Core](https://damienbod.com/2021/01/28/implement-oauth-device-code-flow-with-azure-ad-and-asp-net-core/)
- [Implement app roles authorization with Microsoft Entra ID and ASP.NET Core](https://damienbod.com/2021/02/01/implement-app-roles-authorization-with-azure-ad-and-asp-net-core/)
- [Securing Blazor Web assembly using cookies](https://damienbod.com/2021/03/08/securing-blazor-web-assembly-using-cookies/)
- [Implementing authorization in Blazor ASP.NET Core applications using Microsoft Entra ID security groups](https://damienbod.com/2022/02/21/implementing-authorization-in-blazor-asp-net-core-applications-using-azure-ad-security-groups/)
- [Implementing OAuth2 APP to APP security using Microsoft Entra ID from a Web APP](https://damienbod.com/2022/03/28/implementing-oauth2-app-to-app-security-using-azure-ad-from-a-web-app/)

## History

- 2025-02-07 Small updates
- 2025-01-03 .NET 9, Bootstrap 5
- 2024-04-11 Updated packages
- 2023-12-01 Updated .NET 8
- 2023-08-14 Updated downstream APIs solution
- 2023-08-14 Updated packages
- 2023-05-05 Updated packages
- 2023-03-12 Updated .NET 7, updated nuget packages, implicit usings
- 2022-10-09 Updated nuget packages
- 2022-08-01 Updated nuget packages
- 2022-04-03 Updated nuget packages, added nullable to projects, added CC flow demo
- 2022-02-19 Updated nuget packages
- 2022-01-07 Updated nuget packages
- 2021-11-04 Updated to .NET 6
- 2021-08-13 Updated Blazor BFF app and fixed login button
- 2021-07-03 Microsoft.Identity.Web to 1.14, Angular OIDC V12
- 2021-06-20 Microsoft.Identity.Web to 1.13.1
- 2021-06-06 Microsoft.Identity.Web to 1.12
- 2021-05-13 Microsoft.Identity.Web to 1.9.2, updated packages
- 2021-04-15 Microsoft.Identity.Web to 1.9.1
- 2021-03-11 Microsoft.Identity.Web to 1.8.0
- 2021-03-05 Microsoft.Identity.Web to 1.7.0
- 2021-02-13 Added MSAL exception handling, Microsoft.Identity.Web to 1.6.0
- 2021-02-01 Added app roles authorization example
- 2021-01-28 Added device code flow, Microsoft.Identity.Web to 1.5.1
- 2021-01-19 Updated nuget, npm packages, moved to latest secrets access for certs in Key Vault
- 2021-01-05 Added token management, updated Microsoft.Identity.Web to 1.4.1
- 2020-12-14 Using multiple APIs in Blazor with Microsoft Entra ID authentication
- 2020-12-12 Updated Microsoft.Identity.Web to 1.4.0
- 2020-12-08 Added Using multiple APIs in Angular and ASP.NET Core with Microsoft Entra ID authentication
- 2020-12-04 Updated to .NET 5
- 2020-11-15 Updated Microsoft.Identity.Web to 1.3.0
- 2020-10-25 Updated Microsoft.Identity.Web to 1.2.0

## Links Private key JWT Client authentication

https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-client-creds-grant-flow#second-case-access-token-request-with-a-certificate

## Links Microsoft Entra ID Client credentials flow

https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-credential-flows

https://tools.ietf.org/html/rfc7523

https://openid.net/specs/openid-connect-core-1_0.html#ClientAuthentication

## Links

https://docs.microsoft.com/en-us/azure/active-directory/develop/access-tokens

https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-Assertions

https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-client-creds-grant-flow

https://github.com/AzureAD/microsoft-identity-web/wiki/Using-certificates#describing-client-certificates-to-use-by-configuration

https://www.youtube.com/watch?v=ACZQk61Iq9I

https://www.scottbrady91.com/OAuth/Removing-Shared-Secrets-for-OAuth-Client-Authentication

https://github.com/KevinDockx/ApiSecurityInDepth

https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki

## Links Certs

https://docs.microsoft.com/en-us/azure/azure-app-configuration/quickstart-aspnet-core-app?tabs=core3x

https://github.com/a-patel/azure-app-configuration-labs/blob/ce6c57c0d9837dcdff246bab005b321d4897ee71/src/AzureAppConfigurationLabs.Demo/Program.cs

https://devblogs.microsoft.com/azure-sdk/authentication-and-the-azure-sdk/

https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-protected-web-api-verification-scope-app-roles

https://dev.to/425show/just-what-is-the-default-scope-in-the-microsoft-identity-platform-azure-ad-2o4d

## Links Azure SDK

<a href="https://devblogs.microsoft.com/azure-sdk/authentication-and-the-azure-sdk/">Authentication and the Azure SDK</a>

