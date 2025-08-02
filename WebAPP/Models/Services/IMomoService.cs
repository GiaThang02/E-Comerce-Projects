using WebAPP.Models.Momo;

namespace WebAPP.Models.Services
{
    public interface IMomoService
    {
        Task<string> CreatePayment(string orderId, long amount, string returnUrl, string notifyUrl);

    }
}
