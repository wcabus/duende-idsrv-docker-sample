# Duende IdentityServer - Docker sample

This is a sample setup to run an IdentityServer host and BFF Blazor app in a Docker Compose environment.

## Before you run this!

1. Install [mkcert](https://github.com/FiloSottile/mkcert) on your machine.
2. Create a local CA using mkcert and a certificate for your Docker Compose environment. See [Create-Certificate.ps1](https://github.com/wcabus/duende-idsrv-docker-sample/blob/main/Create-Certificate.ps1) for instructions. Make sure that the certificates are located in the ./certificates folder!
3. Update your local HOSTS file to include the following line:
   127.0.0.1 acme.local login.acme.local app.acme.local

You should now be able to run the app using Docker Compose (in the root folder of this solution):

```bash
docker compose up
```

The BFF can be accessed at https://app.acme.local:5102 and IdentityServer lives at https://login.acme.local:5101

## How does this work?

### Use the same address everywhere

By adding a line to the HOSTS file, binding `login.acme.local` and `app.acme.local` to the local IP address `127.0.0.1`, your browser can translate the addresses to the correct IP address.

But inside the Docker network, the two services still don't know that these addresses exist. For the BFF to be able to contact IdentityServer to retrieve the discovery document, 
it too needs to know that `login.acme.local` points to a specific address. 
That's where [aliases](https://docs.docker.com/reference/compose-file/services/#aliases) are being used for in the Docker Compose file:

```yaml {11}
# excerpt from docker-compose.yml
services:
  identityserver.host:
    image: identityserver.host
    build:
      context: .
      dockerfile: IdentityServer.Host/Dockerfile
    networks:
      acme:
        aliases:
          - login.acme.local
```

Furthermore, the BFF app is configured to use `https://login.acme.local:5101` for its authority, and IdentityServer's client configuration uses the correct domain for the various redirect URIs:

```csharp
// Config.cs
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
```

### SSL certificates

Using mkcert, we've generated a local CA and derived a wildcard SSL certificate for `*.acme.local`. This certificate is added to the services using the [docker-compose.override.yml](https://github.com/wcabus/duende-idsrv-docker-sample/blob/main/docker-compose.override.yml) file (and some variables stored in the `.env` file).

To enable backchannel configuration, we also need to add the CA certificate to the Docker images. That's why the Dockerfile's have the following two commands:

```Dockerfile
COPY ["certificates/cacerts.crt", "/usr/local/share/ca-certificates/acme/"]
RUN update-ca-certificates
```
