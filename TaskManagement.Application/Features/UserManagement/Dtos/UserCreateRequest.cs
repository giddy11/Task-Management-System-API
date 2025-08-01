using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.UserManagement;

namespace TaskManagement.Application.Features.UserManagement.Dtos;

public class UserCreateRequest
{
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Password { get; set; } = default!;
    //public AccountTypes AccountType { get; set; } = AccountTypes.User;
}
