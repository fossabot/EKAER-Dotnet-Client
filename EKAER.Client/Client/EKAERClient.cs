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

using System;
using System.Collections.Generic;
using EKAER.Validation;
using EKAER.Errors;
using EKAER.Schema.Management;
using System.Linq;

namespace EKAER.Client
{
    public partial class ApiClient : BaseClient
    {
        private const string QUERY_TRADE_CARDS = "queryTradeCards";
        private const string VALIDATE_TRADE_CARDS = "validateTradeCards";
        private const string MANAGE_TRADE_CARDS = "manageTradeCards";
        public ApiClient(string username, string password, string VATNumber, string secretKey, string baseUrl = Constants.TEST) : base(username, password, VATNumber, secretKey, baseUrl)
        {
        }

        /// <summary>
        /// Queries one TradeCard by TradeCard number (TCN)
        /// </summary>
        /// <param name="tcn">TradeCard number (tcn)</param>
        /// <returns>TradeCardInfoType object</returns>
        public TradeCardInfo QueryTradeCard(string tcn)
        {
            if (!Validators.IsValidTradeCardNumber(tcn)) throw new ArgumentException(tcn);
            QueryTradeCardsRequest request = BuildRequest<QueryTradeCardsRequest>();
            request.Tcn = tcn;
            var response = Request<QueryTradeCardsResponse, QueryTradeCardsRequest>(QUERY_TRADE_CARDS, request);
            if (response.Result.FuncCode != FunctionCode.ERROR)
            {
                return response.TradeCards.Count > 0 ? response.TradeCards[0] : null;
            }
            throw new EKAERException(response.Result);
        }
        
        /// <summary>
        /// Queries one or more tradecards by arbitari parameters
        /// </summary>
        /// <param name="queryParams">The filter parameters</param>
        /// <returns>ICollection&lt;TradeCardInfoType&gt;</returns>
        public ICollection<TradeCardInfo> QueryTradeCard(QueryParams queryParams)
        {            
            queryParams.Validate();
            var request = BuildRequest<QueryTradeCardsRequest>();           
            request.QueryParams = queryParams;
            var response = Request<QueryTradeCardsResponse, QueryTradeCardsRequest>(QUERY_TRADE_CARDS, request);
            if (response.Result.FuncCode != FunctionCode.ERROR)
            {
                return response.TradeCards;
            }
            throw new EKAERException(response.Result);
        }

        public bool DeleteTradeCard(string tcn, string reason)
        {
            return DeleteTradeCard(tcn, reason, out _);
        }

        public bool DeleteTradeCard(string tcn, string reason, out string error)
        {
            var result = DeleteTradeCards(new Dictionary<string, string> { { tcn, reason } });
            error = result.FirstOrDefault()?.Result.Msg;
            if(result != null && result.Count > 0)
                return result.First().Result.FuncCode != FunctionCode.ERROR;
            return false;
        }

        public ICollection<TradeCardOperationResult> DeleteTradeCards(IDictionary<string, string> tcnsAndReasons)
        {            
            var request = BuildRequest<ManageTradeCardsRequest>();
            var index = 0;
            foreach (var elem in tcnsAndReasons)
            {
                request.TradeCardOperations.Add(new TradeCardOperation { Index = index++, Operation = OperationType.Delete, Tcn = elem.Key, StatusChangeModReasonText = elem.Value });
            }
            var response = Request<ManageTradeCardsResponse, ManageTradeCardsRequest>(MANAGE_TRADE_CARDS, request);
            if (response.Result.FuncCode != FunctionCode.ERROR)
            {                
                return response.TradeCardOperationsResults;
            }
            throw new EKAERException(response.Result);
        }

        public TradeCardOperationResult ValidateTradeCard(TradeCard tradeCard)
        {
            var result = ValidateTradeCards(new List<TradeCard> { tradeCard });
            if(result != null && result.Count > 0)
            {
                return result.First();
            }
            return null;
        }

        public ICollection<TradeCardOperationResult> ValidateTradeCards(IEnumerable<TradeCard> tradeCards)
        {
            var request = BuildRequest<ManageTradeCardsRequest>();
            var index = 0;
            foreach (var tradeCard in tradeCards)
            {
                var operation = new TradeCardOperation
                {
                    Index = index++,
                    Operation = OperationType.Create,
                    TradeCard = tradeCard
                };
                request.TradeCardOperations.Add(operation);
            }
            var response = Request<ManageTradeCardsResponse, ManageTradeCardsRequest>(VALIDATE_TRADE_CARDS, request);
            if (response.Result.FuncCode != FunctionCode.ERROR)
            {
                return response.TradeCardOperationsResults;
            }
            throw new EKAERException(response.Result);
        }

        public void LocalValidate(TradeCard tradeCard)
        {
            if (!Validators.IsValidVatNumber(tradeCard.SellerVatNumber))
                throw new ArgumentException("Seller VAT number is not valid");
            if (!Validators.IsValidVatNumber(tradeCard.DestinationVatNumber))
                throw new ArgumentException("Destination VAT number is not valid");
            if (tradeCard.SellerCountry == "HU" && tradeCard.SellerVatNumber.Length != 8)
                throw new ArgumentException(string.Format("{0} is not a valid hungarian VAT number", tradeCard.SellerVatNumber));
            if (tradeCard.DestinationCountry == "HU" && tradeCard.DestinationVatNumber.Length != 8)
                throw new ArgumentException(string.Format("{0} is not a valid hungarian VAT number", tradeCard.SellerVatNumber));
            if (tradeCard.TradeType == Schema.Common.TradeType.Domestic || tradeCard.TradeType == Schema.Common.TradeType.Export)
            {
                if (string.IsNullOrEmpty(tradeCard.SellerCountry))
                    throw new ArgumentException("Seller country required when TradeType is Export or Domestic");
                if (string.IsNullOrEmpty(tradeCard.SellerAddress))
                    throw new ArgumentException("Seller address required when TradeType is Export or Domestic");
            }
            if (tradeCard.TradeType == Schema.Common.TradeType.Domestic || tradeCard.TradeType == Schema.Common.TradeType.Import)
            {
                if (string.IsNullOrEmpty(tradeCard.DestinationCountry))
                    throw new ArgumentException("Destination country required when TradeType is Import or Domestic");
                if (string.IsNullOrEmpty(tradeCard.DestinationAddress))
                    throw new ArgumentException("Destination address required when TradeType is Import or Domestic");
            }
            if(tradeCard.TradeType == Schema.Common.TradeType.Import && tradeCard.SellerCountry.Equals("HU"))            
                throw new ArgumentException("Seller country must not be HU when TradeType is import");
            
            if(tradeCard.TradeType == Schema.Common.TradeType.Export && !tradeCard.SellerCountry.Equals("HU"))
                throw new ArgumentException("Seller country must be HU when tradeType is Export");

            if (string.IsNullOrEmpty(tradeCard.SellerName))
                throw new ArgumentNullException("SellerName");
            if (string.IsNullOrEmpty(tradeCard.DestinationName))
                throw new ArgumentNullException("DestinationName");
            if (tradeCard.ModByCarrierEnabled && string.IsNullOrEmpty(tradeCard.Carrier))
                throw new ArgumentNullException("If ModByCarrierEnabled then Carrier must not be null");
            if (tradeCard.Items != null)
            {
                foreach(var item in tradeCard.Items)
                {
                    if (item.Value < 0) throw new ArgumentOutOfRangeException("TradeCardItem.Value must be greater or equal to zero");
                    if (item.Value > 0) item.ValueSpecified = true;
                    if (item.Weight < 0) throw new ArgumentOutOfRangeException("Weight must be greater or equal to zero");
                    if (string.IsNullOrEmpty(item.ProductName)) throw new ArgumentNullException("ProductName");
                    if (string.IsNullOrEmpty(item.ProductVtsz)) throw new ArgumentNullException("ProductVTSZ");
                    if (!Validators.IsValidVTSZ(item.ProductVtsz)) throw new ArgumentException("ProductVTSZ");
                }
            }
        }

        public TradeCardInfo CreateTradeCard(TradeCard tradeCard)
        {
            CreateTradeCards(new List<TradeCard>() { tradeCard }, out IEnumerable<TradeCardOperationResult> errors, out IEnumerable<TradeCardInfo> success);
            if(errors != null && errors.Count() > 0)
            {
                throw new EKAERException(errors.First().Result);
            }
            var newCard = success.FirstOrDefault();
            return newCard;
        }

        public void CreateTradeCards(IEnumerable<TradeCard> tradeCards, out IEnumerable<TradeCardOperationResult> errors, out IEnumerable<TradeCardInfo> success)
        {
            var request = BuildRequest<ManageTradeCardsRequest>();
            var index = 0;
            foreach (var tradeCard in tradeCards)
            {
                LocalValidate(tradeCard);
                request.TradeCardOperations.Add(new TradeCardOperation
                {
                    Index = index++,
                    Operation = OperationType.Create,
                    TradeCard = tradeCard
                });                
            }
            var response = Request<ManageTradeCardsResponse, ManageTradeCardsRequest>(MANAGE_TRADE_CARDS, request);
            if (response.Result.FuncCode != FunctionCode.ERROR)
            {
                errors  = response.TradeCardOperationsResults.ToList().Where(p => p.Result.FuncCode == FunctionCode.ERROR);
                success = response.TradeCardOperationsResults.ToList().Where(p => p.Result.FuncCode != FunctionCode.ERROR)?.Select(p => p.TradeCardInfo);
                return;
            }            
            throw new EKAERException(response.Result);
        }

        public TradeCardInfo ModifyTradeCard(TradeCard tradeCard)
        {
            ModifyTradeCards(new List<TradeCard> { tradeCard }, out IEnumerable<TradeCardOperationResult> errors, out IEnumerable<TradeCardInfo> success);
            if (errors != null && errors.Count() > 0) throw new EKAERException(errors.First().Result);
            return success.FirstOrDefault();
        }

        public void ModifyTradeCards(IEnumerable<TradeCard> tradeCards, out IEnumerable<TradeCardOperationResult> errors, out IEnumerable<TradeCardInfo> success)
        {
            var request = BuildRequest<ManageTradeCardsRequest>();
            var index = 0;
            foreach (var tradeCard in tradeCards)
            {
                LocalValidate(tradeCard);
                request.TradeCardOperations.Add(new TradeCardOperation
                {
                    Index = index++,
                    Operation = OperationType.Modify,
                    TradeCard = tradeCard
                });
            }
            var response = Request<ManageTradeCardsResponse, ManageTradeCardsRequest>(MANAGE_TRADE_CARDS, request);
            if (response.Result.FuncCode != FunctionCode.ERROR)
            {
                errors = response.TradeCardOperationsResults.ToList().Where(p => p.Result.FuncCode == FunctionCode.ERROR);
                success = response.TradeCardOperationsResults.ToList().Where(p => p.Result.FuncCode != FunctionCode.ERROR)?.Select(p => p.TradeCardInfo);
                return;
            }
            throw new EKAERException(response.Result);
        }

        public void FinalizeTradeCards(IEnumerable<string> tradeCardNumbers, out IEnumerable<TradeCardOperationResult> errors, out IEnumerable<TradeCardInfo> success)
        {
            var request = BuildRequest<ManageTradeCardsRequest>();
            var index = 0;
            foreach(var tradeCardNumber in tradeCardNumbers)
            {
                request.TradeCardOperations.Add(new TradeCardOperation
                {
                    Index = index++,
                    Operation = OperationType.Finalize,
                    Tcn = tradeCardNumber
                });
            }
            var response = Request<ManageTradeCardsResponse, ManageTradeCardsRequest>(MANAGE_TRADE_CARDS, request);
            if (response.Result.FuncCode != FunctionCode.ERROR)
            {
                errors = response.TradeCardOperationsResults.ToList().Where(p => p.Result.FuncCode == FunctionCode.ERROR);
                success = response.TradeCardOperationsResults.ToList().Where(p => p.Result.FuncCode != FunctionCode.ERROR)?.Select(p => p.TradeCardInfo);
                return;
            }
            throw new EKAERException(response.Result);
        }

        public TradeCardInfo FinalizeTradeCard(string tradeCardNumber)
        {
            FinalizeTradeCards(new List<string>() { tradeCardNumber }, out IEnumerable<TradeCardOperationResult> errors, out IEnumerable<TradeCardInfo> success);
            if(errors!=null && errors.Count() > 0)
            {
                throw new EKAERException(errors.First().Result);
            }
            return success.FirstOrDefault();
        }
    }
}