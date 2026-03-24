# UzunTec API Authentication

Libraries and a sample REST API that help .NET developers add **JWT Bearer** authentication to ASP.NET Core applications. Token responses use **snake_case** property names (`access_token`, `expires_in`, `token_type`) for compatibility with common OAuth2-style clients and Identity Server–like payloads.

## Repository layout

| Project | Purpose |
|--------|---------|
| **UzunTec.API.Authentication.Engine** | `netstandard2.0` library: issue JWTs, configure JWT Bearer validation, DI extension for `IServiceCollection`. Packaged as **UzunTec.API.Authentication.Engine** (NuGet metadata in the `.csproj`). |
| **UzunTec.API.Authentication.RestAPI** | ASP.NET Core **3.1** sample API: token endpoint pattern, `[Authorize]` examples, Swagger UI with Bearer security. |

Solution file: `UzunTec.API.Atuhentication.sln` (note the filename spelling).

## Requirements

- [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet/3.1) (for the sample web project)
- For consuming only the Engine library: any host that supports `netstandard2.0` and the referenced ASP.NET Core / JWT packages

## Quick start (sample API)

From the repository root:

```bash
dotnet restore UzunTec.API.Atuhentication.sln
dotnet run --project UzunTec.API.Authentication.RestAPI/UzunTec.API.Authentication.RestAPI.csproj
```

By default the app listens on **http://localhost:5000** (see `Properties/launchSettings.json`). Open **http://localhost:5000/swagger** to try the API.

### Example flows in the sample

1. **Obtain a token** — `GET /api/user/{user}`  
   - For `admin`, the sample returns a token with extra claims and the `WritePermission` role.  
   - For other users, a simpler token is returned.

2. **Call a protected endpoint** — Send header:  
   `Authorization: Bearer <access_token>`  
   - `GET /api/anyobject` — requires any authenticated user.  
   - `GET /api/anyobject/{id}` — requires role `WritePermission`.

3. **Inspect token payload (sample)** — `GET /api/user/tokenData` with Bearer token returns the JWT payload (implementation relies on the Engine’s token reader during validation).

## Using the Engine in your own API

### 1. Reference the project or NuGet package

Add a project reference to `UzunTec.API.Authentication.Engine`, or install the published package **UzunTec.API.Authentication.Engine** (version and feed as defined by your team).

### 2. Configure `AuthSettings` in appsettings

Bind a section named to match your code (the sample uses `AuthSettings`):

```json
{
  "AuthSettings": {
    "JtiCode": "your-jti-identifier",
    "Audience": "YourAudience",
    "Issuer": "YourIssuer",
    "TokenExpireTimeInSeconds": 7200,
    "SignatureKey": "your-secret-key-for-hmac-minimum-length-per-algorithm",
    "IgnoreSignature": false,
    "SignatureAlgorithm": "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256"
  }
}
```

`AuthenticationConfig` defaults `SignatureAlgorithm` to **HMAC SHA-256**. For **RSA**, set it to `http://www.w3.org/2001/04/xmldsig-more#rsa-sha256`; the Engine will generate an RSA key pair when issuing tokens (see `Authenticator`).

### 3. Register services in `Startup`

```csharp
services.AddAuthenticationEngine(Configuration.GetSection("AuthSettings"));
```

Then in the pipeline (order matters):

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Use `[Authorize]` and `[Authorize(Roles = "...")]` on controllers or actions as usual.

### 4. Issue tokens from your login or token endpoint

Inject `Authenticator` and call:

- `GenerateToken(string userCode)`
- `GenerateToken(string userCode, IReadOnlyDictionary<string, string> claims, IEnumerable<string> roles)`

Returned `TokenData` exposes:

- `Access_token` — JWT string  
- `Expires_in` — lifetime in seconds (from config)  
- `Token_type` — e.g. `"Bearer"`

## Security notes

- Replace sample **SignatureKey**, **Issuer**, **Audience**, and **JtiCode** with production values; never commit real secrets. Prefer user secrets, environment variables, or a secret manager in development.
- `IgnoreSignature: true` disables signing key validation — useful only for special test scenarios, not production.
- `TokenExpireTimeInSeconds: 0` disables lifetime validation in the bearer options; use with care.
- In production, keep **HTTPS** enabled and set `RequireHttpsMetadata` appropriately if you harden JWT bearer options beyond the sample.

## Building the NuGet package

The Engine project has `GeneratePackageOnBuild` enabled. Building the Engine produces a `.nupkg` under the output folder (typical path: `bin/Debug` or `bin/Release`).

```bash
dotnet pack UzunTec.API.Authentication.Engine/UzunTec.API.Authentication.Engine.csproj -c Release
```

## License and attribution

Package metadata in the Engine project lists **© 2020 Uzun Technology** and repository **https://github.com/uztec/api-auth**. Add or adjust a `LICENSE` file in this repo if you need an explicit open-source license text.
