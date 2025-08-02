using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

public class VnPayLibrary
{
    private SortedList<String, String> _requestData = new SortedList<String, String>();

    // Thêm dữ liệu vào request
    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _requestData.Add(key, value);
        }
    }

    // Tạo URL thanh toán với VNPAY
    public string CreateRequestUrl(string baseUrl, string hashSecret)
    {
        string queryString = string.Join("&", _requestData.Select(kv => kv.Key + "=" + HttpUtility.UrlEncode(kv.Value)));
        string signData = string.Join("&", _requestData.Select(kv => kv.Key + "=" + kv.Value));

        // Tạo chữ ký HMACSHA512
        string vnp_SecureHash = HmacSHA512(hashSecret, signData);
        return $"{baseUrl}?{queryString}&vnp_SecureHash={vnp_SecureHash}";
    }

    // Hàm tính toán HMACSHA512
    private string HmacSHA512(string key, string data)
    {
        var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    // Phân tích phản hồi từ VNPAY
    public Dictionary<string, string> GetFullResponseData(string querystring, string hashSecret)
    {
        var responseData = HttpUtility.ParseQueryString(querystring);
        var responseDict = responseData.AllKeys.ToDictionary(k => k, k => responseData[k]);

        // Kiểm tra chữ ký VNPAY
        if (responseDict.ContainsKey("vnp_SecureHash"))
        {
            string receivedHash = responseDict["vnp_SecureHash"];
            responseDict.Remove("vnp_SecureHash");

            string rawData = string.Join("&", responseDict.OrderBy(k => k.Key).Select(kv => kv.Key + "=" + kv.Value));
            string calculatedHash = HmacSHA512(hashSecret, rawData);

            if (!receivedHash.Equals(calculatedHash, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Invalid VNPAY response signature");
            }
        }
        return responseDict;
    }
}
