using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using StockFlow.Application.Configuration;
using StockFlow.Application.DTOs;
using StockFlow.Application.Repositories;
using StockFlow.Application.Services.Interfaces;
using StockFlow.Domain.Entities;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace StockFlow.Application.Services;

public class AuthService: IAuthService
{
     private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return null;
        

        var user = await _userRepository.GetByUsernameAsync(request.Username.Trim(), cancellationToken);

        if (user is null)
            return null;

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
            return null;
        

        if (string.IsNullOrWhiteSpace(_jwtOptions.SecretKey) || Encoding.UTF8.GetByteCount(_jwtOptions.SecretKey) < 32)
            throw new InvalidOperationException("La clave JWT debe tener al menos 32 bytes.");
        

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_jwtOptions.ExpirationMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAt,
            TokenType = "Bearer"
        };
    }
    
}