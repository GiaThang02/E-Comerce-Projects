using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace WebAPP.Models.Services
{
    public class MomoService
    {

        private readonly string _partnerCode = "partnerCode";
        private readonly string _accessKey = "accessKey";
        private readonly string _secretKey = "secretKey";
        private readonly string _endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";

        public async Task<string> CreatePayment(string orderId, long amount, string returnUrl, string notifyUrl)
        {
            var requestId = Guid.NewGuid().ToString();
            var rawSignature = $"partnerCode={_partnerCode}&accessKey={_accessKey}&requestId={requestId}&amount={amount}&orderId={orderId}&orderInfo=Thanh toán đơn hàng&returnUrl={returnUrl}&notifyUrl={notifyUrl}&requestType=captureMoMoWallet";

            var signature = GenerateSignature(rawSignature, _secretKey);

            var requestData = new
            {
                partnerCode = _partnerCode,
                accessKey = _accessKey,
                requestId = requestId,
                orderId = orderId,
                amount = amount.ToString(),
                orderInfo = "Thanh toán đơn hàng",
                returnUrl = returnUrl,
                notifyUrl = notifyUrl,
                requestType = "captureMoMoWallet",
                signature = signature
            };

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
        }

        private string GenerateSignature(string message, string secret)
        {
            var encoding = new UTF8Encoding();
            byte[] keyBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
            }
        }
    }
}
