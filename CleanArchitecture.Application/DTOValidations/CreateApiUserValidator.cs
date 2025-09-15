using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Services;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTOValidations
{
    public class CreateApiUserValidator : AbstractValidator<ApiUserDto>
    {
        private readonly IAccountRepository accountRepository;

        public CreateApiUserValidator(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage("ClientId is required.")
                .MaximumLength(100).WithMessage("ClientId must not exceed 100 characters.")
                .MustAsync(async (user, clientId, cancellation) => !await accountRepository.UserExistsAsync(user.ClientId))
                .WithMessage("ClientId must be unique.");
        }
    }
}
