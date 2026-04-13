<<<<<<< HEAD:Services/StaffEmailService.cs
﻿using Domain.Repositories;
using Services.Abstractions;
using System.Text;
=======
﻿using System.Text;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.AspNetCore.Identity;
>>>>>>> origin/main:HospitalManagementSystem2/Services/StaffEmailGenerator.cs

namespace HospitalManagementSystem2.Services;

public class StaffEmailGenerator : IStaffEmailGenerator
{
    private const int MAX_TRIES = 100;
    private readonly UserManager<IdentityUser> _userManager;

    public StaffEmailGenerator(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GenerateEmailAsync(string firstname, string? lastname, string domain)
    {
<<<<<<< HEAD:Services/StaffEmailService.cs
        var username = CreateUsername(firstName, lastName);
        var domain = Constants.DomainNames.Organization;
=======
        var username = CreateUsername(firstname, lastname);
>>>>>>> origin/main:HospitalManagementSystem2/Services/StaffEmailGenerator.cs
        var email = new StringBuilder($"{username}@{domain}");

        // Check if base email is available
        if (await _userManager.FindByEmailAsync(email.ToString()) == null) 
            return email.ToString();
        
        var count = 1;
        email.Length = username.Length; // Truncate to username
        email.Append(count).Append('@').Append(domain); // Append "1@domain.com"

        // Keep updating the number until a free email is found
        while (await _userManager.FindByEmailAsync(email.ToString()) != null)
        {
            count++;
            email.Length = username.Length; // Truncate to username
            email.Append(count).Append('@').Append(domain); // Append new number and domain

            if (count > MAX_TRIES)
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