using Duende.IdentityServer.Models;

namespace IdentityServer.Host;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
            new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new("api")
    ];

    public static IEnumerable<Client> Clients =>
    [
        // interactive client using code flow + pkce
        new()
        {
            ClientId = "bff",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,

            RedirectUris = { "https://app.acme.local:5102/signin-oidc" },
            FrontChannelLogoutUri = "https://app.acme.local:5102/signout-oidc",
            PostLogoutRedirectUris = { "https://app.acme.local:5102/signout-callback-oidc" },

            AllowOfflineAccess = true,
            AllowedScopes = { "openid", "profile", "api" }
        }
    ];
}
