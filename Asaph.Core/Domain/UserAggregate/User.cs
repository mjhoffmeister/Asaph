using FluentResults;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Asaph.Core.Domain.UserAggregate;

/// <summary>
/// User entity.
/// </summary>
public class User : Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="fullName">Full name.</param>
    /// <param name="emailAddress">Email address.</param>
    /// <param name="phoneNumber">Phone number.</param>
    protected User(
        string fullName, string emailAddress, string? phoneNumber)
    {
        EmailAddress = emailAddress;
        FullName = fullName;
        PhoneNumber = phoneNumber;
    }

    /// <summary>
    /// Email address.
    /// </summary>
    public string EmailAddress { get; }

    /// <summary>
    /// Full name.
    /// </summary>
    public string FullName { get; }

    /// <summary>
    /// Phone number.
    /// </summary>
    public string? PhoneNumber { get; }

    /// <summary>
    /// Tries to create a user.
    /// </summary>
    /// <param name="fullName">Full name.</param>
    /// <param name="emailAddress">Email address.</param>
    /// <param name="phoneNumber">Phone number.</param>
    /// <returns>The result of the create attempt.</returns>
    public static Result<User> TryCreate(
        string? fullName,
        string? emailAddress,
        string? phoneNumber)
    {
        // Try to normalize the phone number
        Result<string?> phoneNumberNormalizationResult = TryNormalizePhoneNumber(phoneNumber);

        // Validate
        Result validationResult = Result
            .Merge(
                ValidateFullName(fullName),
                ValidateEmailAddress(emailAddress),
                phoneNumberNormalizationResult)
            .ToResult();

        // Return failed result if passed parameters are invalid
        if (validationResult.IsFailed)
            return validationResult;

        return Result.Ok(
            new User(fullName!, emailAddress!, phoneNumberNormalizationResult.Value));
    }

    /// <summary>
    /// Determines whether an email address is valid.
    /// </summary>
    /// <param name="emailAddress">Email address.</param>
    /// <returns>True if the email address is valid; false, otherwise.</returns>
    public static Result ValidateEmailAddress(string? emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            return Result.Fail("Email address is required.");

        try
        {
            // Normalize the domain
            emailAddress = Regex.Replace(
                emailAddress,
                @"(@)(.+)$",
                DomainMapper,
                RegexOptions.None,
                TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            static string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                IdnMapping idn = new();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return Result.Fail(
                "A timeout occurred while trying to validate email address domain.");
        }
        catch (ArgumentException)
        {
            return Result.Fail("Invalid domain name in email address.");
        }

        try
        {
            return Result.OkIf(
                Regex.IsMatch(
                    emailAddress,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase,
                    TimeSpan.FromMilliseconds(250)),
                "Invalid email address.");
        }
        catch (RegexMatchTimeoutException)
        {
            return Result.Fail("A timeout occurred while trying to validate email address.");
        }
    }

    /// <summary>
    /// Determines whether a full name is valid.
    /// </summary>
    /// <param name="fullName">Full name.</param>
    /// <returns>True if the full name is valid; false, otherwise.</returns>
    public static Result ValidateFullName(string? fullName) =>
        Result.OkIf(fullName?.Trim().Length > 0, "Full name is required.");

    /// <summary>
    /// Tries to normalize a phone number.
    /// </summary>
    /// <param name="phoneNumber">Phone number.</param>
    /// <returns>True if the phone number was normalized; false, otherwise.</returns>
    public static Result<string?> TryNormalizePhoneNumber(string? phoneNumber)
    {
        if (phoneNumber == null)
            return Result.Ok<string?>(null);

        try
        {
            // Normalize the phone number by removing parentheses, dashes, dots, and spaces
            phoneNumber = Regex.Replace(phoneNumber, @"[()-.\s]", "");
        }
        catch (RegexMatchTimeoutException)
        {
            return Result.Fail("A timeout occurred while trying to validate phone number.");
        }

        return phoneNumber.Length == 10 && double.TryParse(phoneNumber, out _) ?
            Result.Ok<string?>(phoneNumber) :
            Result.Fail<string?>("Invalid phone number.");
    }
}