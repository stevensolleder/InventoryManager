using System.ComponentModel.DataAnnotations;
using InventoryManager.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManager.Options;

public class JwtOptions
{
    [MinLength(1)] public string Issuer { get; }
    [MinLength(1)] public string Audience { get; }
    public SymmetricSecurityKey SigningKey { get; }
    [MinLength(1)] public string SigningAlgorithm { get; }
    [Range(30, 3000)] public uint TokenExpirationTime { get; }
    
    public JwtOptions(string issuer, string audience, SymmetricSecurityKey signingKey, string signingAlgorithm, uint tokenExpirationTime)
    {
        Issuer = issuer;
        Audience = audience;
        SigningKey = signingKey;
        SigningAlgorithm = signingAlgorithm;
        TokenExpirationTime = tokenExpirationTime;

        this.ValidateAllProperties();
    }
}