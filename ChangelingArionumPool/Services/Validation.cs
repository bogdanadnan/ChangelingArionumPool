using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using ChangelingArionumPool.Entities;
using ChangelingBizUtils;

namespace ChangelingArionumPool.Services
{
    public class Validation
    {
        private static readonly Lazy<Validation> _instance =
                new Lazy<Validation>(() => new Validation());

        public static Validation Instance { get { return _instance.Value; } }

        private Validation() { }

        private Random _random = new Random();

        public ValidationResult ValidateWallet(string wallet)
        {
            ValidationResult result = new ValidationResult { Status = ValidationStatus.Error, Message = "Error parsing wallet: " };

            if(String.IsNullOrEmpty(wallet))
            {
                result.Message += "no wallet provided.";
                return result;
            }

            if(wallet == "3uj7kyCcy5q6A1s1DQkgb58zXz6mLsjHpaoEkYxL6TjzkRP7muGZaXeGNcqk1bTgpQTVDuwPoKh49dGQn8bMwdBZ"
               || wallet == "4EtWPLwbUAs2JNnqb8yprvAKYCfA4dU3bJTRm2KjnM6f811MAh9qr7wrHABCHrnWPTdgmEF8iXqRBu2XSPHMuHnR")
            {
                result.Message += "blacklisted/broken wallet provided.";
                return result;
            }

            if(wallet.Length < 70 || wallet.Length > 128 || !Base58.IsValid(wallet))
            {
                result.Message += "invalid wallet provided.";
                return result;
            }

            result.Status = ValidationStatus.Success;
            result.Message = String.Empty;

            return result;
        }

        public ValidationResult ValidateWorker(string worker)
        {
            ValidationResult result = new ValidationResult { Status = ValidationStatus.Error, Message = "Error parsing worker name: " };

            if (String.IsNullOrEmpty(worker))
            {
                result.Message += "no worker name provided.";
                return result;
            }

            result.Status = ValidationStatus.Success;
            result.Message = String.Empty;

            return result;
        }

        public ValidationResult ValidateNonceBlockHeight(string heightStr)
        {
            ValidationResult result = new ValidationResult { Status = ValidationStatus.Error, Message = "Error validating nonce: " };

            if (!String.IsNullOrEmpty(heightStr))
            {
                int height = 0;
                if (Int32.TryParse(heightStr, out height))
                {
                    if (Node.Instance.CurrentBlock.Height != height)
                    {
                        result.Message += "block changed.";
                        return result;
                    }
                }
            }

            result.Status = ValidationStatus.Success;
            result.Message = String.Empty;

            return result;
        }

        public ValidationResult ValidateNonce(int ip, string wallet, string argon, string nonce, Block block)
        {
            ValidationResult result = new ValidationResult { Status = ValidationStatus.Error, Message = "Error validating nonce: " };

            if (String.IsNullOrEmpty(argon))
            {
                result.Status = ValidationStatus.SyntaxError;
                result.Message += "empty argon parameter.";
                return result;
            }

            if (String.IsNullOrEmpty(nonce))
            {
                result.Status = ValidationStatus.SyntaxError;
                result.Message += "empty nonce parameter.";
                return result;
            }

            try
            {
                int nonceStatus = DataStorage.Instance.CheckAbuse(ip, wallet, argon);
                switch (nonceStatus)
                {
                    case 1:
                        result.Status = ValidationStatus.Abuser;
                        result.Message += "too many invalid nonces from this ip, cooling down.";
                        return result;
                    case 2:
                        result.Status = ValidationStatus.Abuser;
                        result.Message += "duplicate nonce.";
                        return result;
                }
            }
            catch(Exception ex)
            {
                result.Status = ValidationStatus.UnexpectedError;
                result.Message += ex.Message;
                return result;
            }

            string argonHash = String.Empty;
            string baseHash = String.Empty;
            Argon2Params argon2Params = new Argon2Params(block.Type);

            argonHash = argon2Params.Prefix + argon;
            baseHash = Configuration.PoolPublicKey + "-" + nonce + "-" + block.Id + "-" + block.Difficulty;

            List<string> argonValidationServiceUrlList = Configuration.ArgonValidationServiceUrl;
            string argonValidationServiceUrl = argonValidationServiceUrlList[_random.Next(argonValidationServiceUrlList.Count)];

            argonValidationServiceUrl += ("validate?argon=" + HttpUtility.UrlEncode(argonHash) + "&base=" + HttpUtility.UrlEncode(baseHash));
            var http = (HttpWebRequest)WebRequest.Create(argonValidationServiceUrl);
            http.Timeout = 5000;

            var content = String.Empty;
            try
            {
                using (var response = http.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        content = stream.ReadToEnd().Trim();
                        if (content != "VALID")
                        {
                            result.Message += "argon2 hash is invalid.";
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = ValidationStatus.UnexpectedError;
                result.Message += ex.Message;
                return result;
            }
        
            result.Status = ValidationStatus.Success;
            result.Message = String.Empty;

            return result;
        }
    }
}
