using Microsoft.AspNetCore.Identity;

namespace Jobino.Services;

public class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("رمز عبور نمی‌تواند خالی باشد");
        }

        return _passwordHasher.HashPassword(null!, password);
    }

    public bool Verify(string passwordHash, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            null!,
            passwordHash,
            password);

        return result == PasswordVerificationResult.Success
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}