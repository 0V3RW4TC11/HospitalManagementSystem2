using System.Text;
using Domain;
using Domain.Constants;
using Domain.Providers;
using Services.Abstractions;

namespace Services;

internal sealed class StaffEmailService : IStaffEmailService
{
    private const int MaxTries = 100;

    private readonly IIdentityProvider _identityProvider;

    public StaffEmailService(IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    public async Task<string> CreateStaffEmailAsync(string firstName, string? lastName)
    {
        var username = CreateUsername(firstName, lastName);
        var domain = DomainNames.Organization;
        var email = new StringBuilder($"{username}@{domain}");

        // Check if base email is available
        if (!await _identityProvider.EmailExistsAsync(email.ToString())) 
            return email.ToString();
        
        var count = 1;
        email.Length = username.Length; // Truncate to username
        email.Append(count).Append('@').Append(domain); // Append "1@domain.com"

        // Keep updating the number until a free email is found
        while (await _identityProvider.EmailExistsAsync(email.ToString()))
        {
            count++;
            email.Length = username.Length; // Truncate to username
            email.Append(count).Append('@').Append(domain); // Append new number and domain

            if (count > MaxTries)
                throw new Exception("Staff email generation max tries reached");
        }

        return email.ToString();
    }
    
    private static string CreateUsername(string firstname, string? lastname)
    {
        if (string.IsNullOrWhiteSpace(firstname))
            throw new Exception("Firstname cannot be empty");

        return string.IsNullOrWhiteSpace(lastname) ? 
            firstname.ToLower() : $"{firstname.ToLower()}.{lastname.ToLower()}";
    }
}