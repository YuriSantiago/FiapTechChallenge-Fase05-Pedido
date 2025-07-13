using Core.Requests.Create;
using FluentValidation;

namespace Core.Validators
{
    public class PedidoItemRequestValidator : AbstractValidator<PedidoItemRequest>
    {

        public PedidoItemRequestValidator()
        {
            RuleFor(x => x.ProdutoId)
                .NotEmpty().WithMessage("O ID do pedido é obrigatório.")
                .GreaterThan(0).WithMessage("O ID do produto deve ser maior que zero.");

            RuleFor(x => x.Quantidade)
                .NotEmpty().WithMessage("A quantidade é obrigatória.")
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
        }

    }
}
