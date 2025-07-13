using Core.Requests.Delete;
using FluentValidation;

namespace Core.Validators
{
    public class PedidoDeleteRequestValidator : AbstractValidator<PedidoDeleteRequest>
    {

        public PedidoDeleteRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O ID do pedido é obrigatório.")
                .GreaterThan(0).WithMessage("O ID do pedido deve ser maior que zero.");
        }

    }
}
