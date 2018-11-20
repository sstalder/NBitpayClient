using System;
using System.IO;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Newtonsoft.Json;

namespace NBitpayClient.Tests
{
    public class Program
    {
        private readonly Uri BitPayUri = new Uri("https://test.bitpay.com/");

        private static Network Network = Network.TestNet;

        private Bitpay Bitpay = null;

        public static void Main(string[] args)
        {
            Logs.Configure(new FuncLoggerFactory(i => new CustomerConsoleLogger(i, (a, b) => true, false)));

            try
            {
                new Program().Run();

                Logs.Tests.LogInformation("Tests ran successfully");
            }
            catch (AssertException ex)
            {
                Logs.Tests.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                Logs.Tests.LogError(ex.ToString());
            }

            Console.ReadLine();
        }

        private void CanSerializeDeserialize()
        {
            var str = "{\"id\":\"NzzNUB5DEMLP5q95szL1VS\",\"url\":\"https://test.bitpay.com/invoice?id=NzzNUB5DEMLP5q95szL1VS\",\"posData\":\"posData\",\"status\":\"paid\",\"btcPrice\":\"0.001246\",\"price\":5,\"currency\":\"USD\",\"invoiceTime\":1503140597709,\"expirationTime\":1503141497709,\"currentTime\":1503140607752,\"btcPaid\":\"0.001246\",\"btcDue\":\"0.000000\",\"rate\":4012.12,\"exceptionStatus\":false,\"buyerFields\":{}}";
            var notif = JsonConvert.DeserializeObject<InvoicePaymentNotification>(str);
            var serialized = JsonConvert.SerializeObject(notif);

            //from https://bitpay.com/docs/invoice-callbacks
            var example1 = "{\r\n  \"id\":\"SkdsDghkdP3D3qkj7bLq3\",\r\n  \"url\":\"https://bitpay.com/invoice?id=SkdsDghkdP3D3qkj7bLq3\",\r\n  \"status\":\"paid\",\r\n  \"price\":10,\r\n  \"currency\":\"EUR\",\r\n  \"invoiceTime\":1520373130312,\r\n  \"expirationTime\":1520374030312,\r\n  \"currentTime\":1520373179327,\r\n  \"exceptionStatus\":false,\r\n  \"buyerFields\":{\r\n    \"buyerEmail\":\"test@bitpay.com\",\r\n    \"buyerNotify\":false\r\n  },\r\n  \"paymentSubtotals\": {\r\n    \"BCH\":1025900,\r\n    \"BTC\":114700\r\n  },\r\n  \"paymentTotals\": {\r\n    \"BCH\":1025900,\r\n    \"BTC\":118400\r\n  },\r\n  \"transactionCurrency\": \"BCH\",\r\n  \"amountPaid\": \"1025900\",\r\n  \"exchangeRates\": {\r\n    \"BTC\": {\r\n      \"EUR\": 8721.690715789999,\r\n      \"USD\": 10817.99,\r\n      \"BCH\": 8.911763736716368\r\n  },\r\n    \"BCH\": {\r\n      \"EUR\": 974.721189,\r\n      \"USD\": 1209,\r\n      \"BTC\": 0.11173752310536043\r\n    }\r\n  }\r\n}";
            var example2 = "{\r\n  \"id\":\"9E8qPC3zsvXRcA3tsbRLnC\",\r\n  \"url\":\"https://bitpay.com/invoice?id=9E8qPC3zsvXRcA3tsbRLnC\",\r\n  \"status\":\"confirmed\",\r\n  \"btcPrice\":\"1.854061\",\r\n  \"price\":\"19537.1\",\r\n  \"currency\":\"USD\",\r\n  \"invoiceTime\":1520417833546,\r\n  \"expirationTime\":1520418733546,\r\n  \"currentTime\":1520417905248,\r\n  \"btcPaid\":\"1.854061\",\r\n  \"btcDue\":\"0.000000\",\r\n  \"rate\":10537.46,\r\n  \"exceptionStatus\":false,\r\n  \"buyerFields\": {\r\n    \"buyerEmail\":\"test@bitpay.com\",\r\n    \"buyerNotify\":false\r\n  },\r\n  \"paymentSubtotals\": {\r\n    \"BCH\":1664857265,\r\n    \"BTC\":185406100\r\n  },\r\n  \"paymentTotals\": {\r\n    \"BCH\":1664857265,\r\n    \"BTC\":185409800\r\n  },\r\n  \"transactionCurrency\": \"BTC\",\r\n  \"amountPaid\": \"185409800\",\r\n  \"exchangeRates\": {\r\n    \"BTC\": {\r\n      \"USD\": 10537.46373483771,\r\n      \"BCH\": 8.979517456188931\r\n    },\r\n    \"BCH\": {\r\n      \"USD\": 1173.50,\r\n      \"BTC\": 0.111364558828356\r\n    }\r\n  }\r\n}";

            var example1notif = JsonConvert.DeserializeObject<InvoicePaymentNotification>(example1);
            var example2notif = JsonConvert.DeserializeObject<InvoicePaymentNotification>(example2);
            var serialized1 = JsonConvert.SerializeObject(example1notif);
            var serialized2 = JsonConvert.SerializeObject(example2notif);

            //from https://bitpay.com/docs/display-invoice
            var invoicestr = "{\"id\":\"7MxRGVuBC1XvV138b3AqAR\",\"guid\":\"177005a3-2867-4c65-add8-7ab088e3c414\",\"itemDesc\":\"Lawncare, March\",\"invoiceTime\":1520368215297,\"expirationTime\":1520369115297,\"currentTime\":1520368235844,\"url\":\"https://test.bitpay.com/invoice?id=7MxRGVuBC1XvV138b3AqAR\",\"posData\":\"{ \\\"ref\\\" : 711454, \\\"affiliate\\\" : \\\"spring112\\\" }\",\"status\":\"new\",\"exceptionStatus\":false,\"price\":10,\"currency\":\"USD\",\"btcPrice\":\"0.000942\",\"btcDue\":\"0.000971\",\"paymentSubtotals\":{\"BTC\":94200,\"BCH\":8496000},\"paymentTotals\":{\"BTC\":97100,\"BCH\":8496000},\"btcPaid\":\"0.000000\",\"amountPaid\":0,\"rate\":10621.01,\"exRates\":{\"BTC\":1,\"BCH\":8.984103997289974,\"USD\":10608.43},\"exchangeRates\":{\"BTC\":{\"BCH\":8.997805828532702,\"USD\":10621.01},\"BCH\":{\"USD\":1177,\"BTC\":0.11077647058823528}},\"paymentCurrencies\":[\"BTC\"],\"addresses\":{\"BTC\":\"mtXiukcxY2QjLSWGNaHdbvrvtakX4m5R1t\",\"BCH\":\"qz8tacx6fn0h6wwzd2k4y4ya5e2zddg0e5cm4nukfr\"},\"paymentUrls\":{\"BIP21\":\"bitcoin:mjBQNNE16a6gWKkkMxc2QiLzrZVViyruUe?amount=0.069032\",\"BIP72\":\"bitcoin:mjBQNNE16a6gWKkkMxc2QiLzrZVViyruUe?amount=0.069032&r=https://test.bitpay.com/i/7MxRGVuBC1XvV138b3AqAR\",\"BIP72b\":\"bitcoin:?r=https://test.bitpay.com/i/7MxRGVuBC1XvV138b3AqAR\",\"BIP73\":\"https://test.bitpay.com/i/7MxRGVuBC1XvV138b3AqAR\"},\"paymentCodes\":{\"BTC\":{\"BIP72b\":\"bitcoin:?r=https://test.bitpay.com/i/WoCy658tqHJrfa35F99gnp\",\"BIP73\":\"https://test.bitpay.com/i/WoCy658tqHJrfa35F99gnp\"},\"BCH\":{\"BIP72b\":\"bitcoincash:?r=https://test.bitpay.com/i/WoCy658tqHJrfa35F99gnp\",\"BIP73\":\"https://test.bitpay.com/i/WoCy658tqHJrfa35F99gnp\"}},\"token\":\"Hncf45uBVPNoiXbycHDh2cC37auMxhrxm5ijNCsTKGKfX4Y1vbjWCZvoSdciMNw5G\"}";
            var invoice = JsonConvert.DeserializeObject<Invoice>(invoicestr);
            var serializedInvoice = JsonConvert.SerializeObject(invoice);
        }

        private void Run()
        {
            //CanSerializeDeserialize();
            EnsureRegisteredKey();
            //CanMakeInvoice();
            //CanGetRate();
            ThrowsError();
        }

        private void ThrowsError()
        {
            var invoice = Bitpay.CreateInvoice(new Invoice
            {
                Price = 0.01m,
                Currency = "USD",
                ItemDesc = "This should fail",
                FullNotifications = true,
                PaymentCurrencies = new[] { "BTC" }
            });

            Assert.NotNull(invoice);
        }

        private void CanMakeInvoice()
        {
            var invoice = Bitpay.CreateInvoice(new Invoice
            {
                Price = 1.0m,
                Currency = "USD",
                PosData = "posData",
                OrderId = "orderId",
                NotificationURL = "/notification",
                ItemDesc = "Some description",
                FullNotifications = true
            });

            var response = Bitpay.CreateInvoice(invoice);

            Assert.NotNull(response);

            var invoice2 = Bitpay.GetInvoice(invoice.Id);

            Assert.NotNull(invoice2);
        }

        private void CanGetRate()
        {
            var rates = Bitpay.GetRates();
            Assert.NotNull(rates);
            Assert.True(rates.AllRates.Count > 0);

            var btcrates = Bitpay.GetRates("BTC");
            Assert.NotNull(rates);
            Assert.True(btcrates.AllRates.Count > 0);

            var btcusd = Bitpay.GetRate("BTC", "USD");
            Assert.NotNull(btcusd);
            Assert.Equal(btcusd.Code, "USD");
        }

        private void EnsureRegisteredKey()
        {
            if (!Directory.Exists(Network.Name))
                Directory.CreateDirectory(Network.Name);

            BitcoinSecret k = null;
            var keyFile = Path.Combine(Network.Name, "key.env");
            try
            {
                k = new BitcoinSecret(File.ReadAllText(keyFile), Network);
            }
            catch { }

            if (k != null)
            {
                try
                {
                    Bitpay = new Bitpay(k.PrivateKey, BitPayUri);
                    if (Bitpay.TestAccess(Facade.Merchant))
                        return;
                }
                catch { }
            }

            k = k ?? new BitcoinSecret(new Key(), Network);
            File.WriteAllText(keyFile, k.ToString());

            Bitpay = new Bitpay(k.PrivateKey, BitPayUri);
            var pairing = Bitpay.RequestClientAuthorization("test", Facade.Merchant);

            throw new AssertException("You need to approve the test key to access bitpay by going to this link " + pairing.CreateLink(Bitpay.BaseUrl).AbsoluteUri);
        }
    }
}