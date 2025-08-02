using PayPal.Api;

public class PayPalService
{
    private readonly string _clientId;
    private readonly string _clientSecret;

    public PayPalService(IConfiguration config)
    {
        _clientId = config["PayPal:ClientId"];
        _clientSecret = config["PayPal:ClientSecret"];
    }

    private APIContext GetApiContext()
    {
        var oauthToken = new OAuthTokenCredential(_clientId, _clientSecret).GetAccessToken();
        return new APIContext(oauthToken);
    }

    public Payment CreatePayment(string baseUrl, string intent, decimal amount)
    {
        var apiContext = GetApiContext();

        var payer = new Payer() { payment_method = "paypal" };

        var redirectUrls = new RedirectUrls()
        {
            cancel_url = $"{baseUrl}/Payment/Cancelled",
            return_url = $"{baseUrl}/Payment/Success"
        };

        var details = new Details() { tax = "0", shipping = "0", subtotal = amount.ToString() };

        var amountObj = new Amount()
        {
            currency = "USD",
            total = amount.ToString(),
            details = details
        };

        var transactionList = new List<Transaction>()
        {
            new Transaction()
            {
                description = "Transaction description",
                amount = amountObj
            }
        };

        var payment = new Payment()
        {
            intent = intent,
            payer = payer,
            transactions = transactionList,
            redirect_urls = redirectUrls
        };

        return payment.Create(apiContext);
    }
}
