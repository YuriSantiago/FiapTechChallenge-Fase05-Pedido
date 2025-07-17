using Core.Requests.Update;
using FluentValidation;

namespace Core.Validators
{
    public class PedidoCancelationRequestValidator : AbstractValidator<PedidoCancelationRequest>
    {

        public PedidoCancelationRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("O ID do pedido deve ser maior que zero.");

            RuleFor(regiao => regiao.DescricaoCancelamento)
               .NotEmpty().WithMessage("A descrição de cancelamento é obrigatória.")
               .MaximumLength(100).WithMessage("A descrição de cancelamento deve ter no máximo 150 caracteres.");
        }


    }
}
