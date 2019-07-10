// Copyright (c) 2019 Péter Németh
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using EKAER.Errors;
using EKAER.Schema.Management;
using System;

namespace EKAER.Client.Sample
{
    class Program
    {
        private static string apiUser = "";
        private static string apiPassword = "";
        private static string VATNumber = "";
        private static string apiSecret = "";
        private static ApiClient client;

        static void Main(string[] args)
        {
            client = new ApiClient(apiUser, apiPassword, VATNumber, apiSecret);
            try
            {
                // Create
                var tradeCard = client.CreateTradeCard(BuildNewTradeCard());
                Console.WriteLine(tradeCard.Tcn + " created");

                // Query
                tradeCard = client.QueryTradeCard(tradeCard.Tcn);
                Console.WriteLine($"TradeCard {tradeCard.Tcn} object received");

                // Modify
                tradeCard.ArrivalDate = DateTime.Now;
                foreach (var plan in tradeCard.DeliveryPlans)
                {
                    foreach (var elem in plan.Items)
                    {
                        elem.ItemOperation = ItemOperation.Modify;
                        elem.ItemOperationSpecified = true;
                    }
                }
                tradeCard = client.ModifyTradeCard(tradeCard);
                Console.WriteLine($"TradeCard modified. New arrival date: {tradeCard.ArrivalDate}");                
                // Finalize
                try
                {
                    var finalized = client.FinalizeTradeCard(tradeCard.Tcn);
                    Console.WriteLine("TradeCard finalized");
                }
                catch (EKAERException e)
                {
                    Console.WriteLine(e.Result.ReasonCode);
                    Console.WriteLine(e.Result.FuncCode);
                    Console.WriteLine(e.Message);
                }

                // Delete
                if (client.DeleteTradeCard(tradeCard.Tcn, "", out string error))
                {
                    Console.WriteLine(tradeCard.Tcn + " deleted");
                }
                else
                {
                    Console.WriteLine(error);
                }
            }
            catch (EKAERException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static TradeCard BuildNewTradeCard()
        {
            var tradeCard = new TradeCard
            {
                OrderNumber = "1224345",
                TradeCardType = Schema.Common.TradeCardType.Normal,
                TradeType = Schema.Common.TradeType.Domestic,
                SellerName = "Minta eladó Kft.",
                SellerVatNumber = VATNumber,
                SellerCountry = "HU",
                SellerAddress = "4024 Debrecen, Egyik utca 1",
                DestinationCountry = "HU",
                DestinationAddress = "1220 Budapest, Másik utca 1",
                DestinationName = "Minta vevő Kft.",
                DestinationVatNumber = "12345676",
                ArrivalDate = DateTime.Now.AddDays(1),
                ArrivalDateSpecified = true,
                LoadDate = DateTime.Now,
                Vehicle = new Vehicle { PlateNumber = "ABC123" }
            };
            var deliveryPlan = new DeliveryPlan
            {
                IsDestinationCompanyIdentical = false,
                LoadLocation = new Location { Country = "HU", Name = "Minta eladó Kft.", VATNumber = "12345676", StreetType = "utca", City = "Debrecen", ZipCode = "4024", Street = "Minta", StreetNumber = "1" },
                UnloadLocation = new Location { Country = "HU", Name = "Minta vevő Kft", VATNumber = "12345676", StreetType = "utca", City = "Budapest", ZipCode = "1223", Street = "Minta", StreetNumber = "1" }
            };
            var item = new TradeCardItemType
            {
                ProductName = "Sajtos pogácsa",
                TradeReason = Schema.Common.TradeReason.Sale,
                Weight = 7500,
                Value = 1000000,
                ValueSpecified = true,
                ProductVtsz = "38248400",
                ItemOperation = ItemOperation.Create
            };
            deliveryPlan.Items.Add(item);
            tradeCard.DeliveryPlans.Add(deliveryPlan);
            return tradeCard;
        }
    }
}
