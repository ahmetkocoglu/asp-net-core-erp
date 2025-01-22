namespace asp_net_core_erp.Models;

public record User(
    int Id,
    string Username,
    string Name,
    string Email,
    string Password,
    string[] Roles)
{
}