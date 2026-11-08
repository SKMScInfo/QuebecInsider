using FluentValidation;
using QuebecInsider.Api.Dtos;

namespace QuebecInsider.Api.Validators;

public sealed class NewsletterSignupValidator : AbstractValidator<NewsletterSignupDto>
{
    public NewsletterSignupValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'adresse courriel est obligatoire.")
            .EmailAddress().WithMessage("Le format de l'adresse courriel est invalide.")
            .MaximumLength(254);

        RuleFor(x => x.Culture)
            .Must(c => c is "fr-CA" or "en-CA")
            .WithMessage("La culture doit être 'fr-CA' ou 'en-CA'.");
    }
}