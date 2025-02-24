using iPhoneBE.Data.Models.EmailModel;

namespace iPhoneBE.Data.Helper.EmailHelper
{
    public interface IEmailHelper
    {
        Task SendMailAsync(CancellationToken cancellationToken, EmailRequestModel emailRequest);
    }
}