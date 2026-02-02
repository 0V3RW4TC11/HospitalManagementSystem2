namespace Commands.Admin
{
    public record CreateAdminCommand(string Password) : AdminBaseCommand;
}