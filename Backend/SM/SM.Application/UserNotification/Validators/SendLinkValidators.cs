using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Abstractions.Validators.Abstractions;
using SM.Application.UserNotification.Command;

namespace SM.Application.UserNotification.Validators
{
    public class SendLinkValidatorsRule : CompositeRule<SendLinkRequest>
    {
        private ValidatorResponse _validator;

        public SendLinkValidatorsRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(SendLinkRequest request)
        {
            var isValid = true;
            if (string.IsNullOrEmpty(request.To))
            {
                _validator.AddNotification("O campo E-mail é obrigatório.");
                isValid = false;
            }
            if (string.IsNullOrEmpty(request.Url))
            {
                _validator.AddNotification("O campo URL é obrigatório.");
                isValid = false;
            }
            if (string.IsNullOrEmpty(request.Message))
            {
                _validator.AddNotification("O campo mensagem é obrigatório.");
                isValid = false;
            }
            return isValid;
        }
    }
}
