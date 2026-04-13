namespace Services.Abstractions;

public interface IStaffEmailService
{
    public Task<string> CreateStaffEmailAsync(string firstName, string? lastName);
}