using Core.Requests.Create;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators
{
    public class PedidoRequestValidator : AbstractValidator<PedidoRequest>
    {

        public PedidoRequestValidator()
        {
            RuleFor(x => x.UsuarioId)
                .NotEmpty().WithMessage("O ID do usuário é obrigatório.")
                .GreaterThan(0).WithMessage("O ID do usuário deve ser maior que zero.");

            RuleFor(x => x.TipoEntrega)
                .NotEmpty().WithMessage("O tipo de entrega é obrigatório.")
                .MaximumLength(50).WithMessage("O tipo de entrega deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Itens)
                .NotEmpty().WithMessage("A lista de itens do pedido não pode estar vazia.");

            RuleForEach(x => x.Itens)
                .SetValidator(new PedidoItemRequestValidator());
        }

    }
}
