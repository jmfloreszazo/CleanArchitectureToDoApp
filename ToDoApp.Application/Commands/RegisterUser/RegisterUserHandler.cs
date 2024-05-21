using MediatR;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Domain.Email;
using ToDoApp.Domain.Interfaces;

namespace ToDoApp.Application.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IEmailSender _emailSender;

    public RegisterUserHandler(IAuthenticationService authenticationService, IEmailSender emailSender)
    {
        _authenticationService = authenticationService;
        _emailSender = emailSender;
    }

    public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.RegisterUser(request.UserForRegistration!);
        if (!result.Succeeded)
        {
            return result;
        }

        //await _emailSender.SendEmailAsync(new Message([request.UserForRegistration.Email],
        //   "Registration message", "User successfully registered", null));

        return result;
    }
}
