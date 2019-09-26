using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Application.Errors;
using System.Net;
using Application.Validators;

namespace Application.User
{
    public class Register
    {
        public class Command : IRequest<User>
        {
            public string Displayname { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Displayname).NotEmpty();
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).Password();
            }
        }

        public class Handler : IRequestHandler<Command, User>
        {
            private readonly DataContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly IJwtGenerator _jwtGenerator;
            public Handler(DataContext context, UserManager<AppUser> userManager, IJwtGenerator jwtGenerator)
            {
                _jwtGenerator = jwtGenerator;
                _userManager = userManager;
                _context = context;
            }

            public async Task<User> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await _context.Users.Where(x => x.Email.Equals(request.Email)).AnyAsync())
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });
                if (await _context.Users.Where(x => x.UserName.Equals(request.Username)).AnyAsync())
                    throw new RestException(HttpStatusCode.BadRequest, new { UserName = "UserName already exists" });

                var newUser = new AppUser
                {
                    DisplayName = request.Displayname,
                    UserName = request.Username,
                    Email = request.Email,
                };


                var result = await _userManager.CreateAsync(newUser, request.Password);
                if (result.Succeeded)
                {
                    return new User
                    {
                        DisplayName = newUser.DisplayName,
                        UserName = newUser.UserName,
                        Image = null,
                        Token = _jwtGenerator.CreateToken(newUser),
                    };
                }
                throw new Exception("Problem creating user");
            }
        }
    }
}