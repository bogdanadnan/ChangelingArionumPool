using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using ChangelingArionumPool.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ChangelingArionumPool.Services
{
    public class Pool
    {
        private static readonly Lazy<Pool> _instance =
            new Lazy<Pool>(() => new Pool());

        public static Pool Instance { get { return _instance.Value; } }

        public PoolResponse Info(string wallet, string worker, string hashRateCblocksStr, string hashRateGblocksStr, int ip)
        {
            var validWallet = Validation.Instance.ValidateWallet(wallet);
            if (validWallet.Status != ValidationStatus.Success)
            {
                return new PoolErrorResponse(validWallet.Message);
            }

            var validWorker = Validation.Instance.ValidateWorker(worker);
            if (validWorker.Status != ValidationStatus.Success)
            {
                return new PoolErrorResponse(validWorker.Message);
            }

            int hashRateCblocks = 0;
            int hashRateGblocks = 0;

            if (!String.IsNullOrEmpty(hashRateCblocksStr) || !String.IsNullOrEmpty(hashRateGblocksStr))
            {
                double hashRateCblocksHighPrecision = 0;
                double hashRateGblocksHighPrecision = 0;

                if (!String.IsNullOrEmpty(hashRateCblocksStr) && !Double.TryParse(hashRateCblocksStr, out hashRateCblocksHighPrecision))
                {
                    return new PoolErrorResponse("Error parsing cblocks hashrate: not a number.");
                }

                if (!String.IsNullOrEmpty(hashRateGblocksStr) && !Double.TryParse(hashRateGblocksStr, out hashRateGblocksHighPrecision))
                {
                    return new PoolErrorResponse("Error parsing gblocks hashrate: not a number.");
                }

                hashRateCblocks = (int)Math.Ceiling(hashRateCblocksHighPrecision);
                hashRateGblocks = (int)Math.Ceiling(hashRateGblocksHighPrecision);

                if (hashRateCblocks > 0 || hashRateGblocks > 0)
                {
                    try
                    {
                        DataStorage.Instance.SaveHashRateEntry(wallet, worker, hashRateCblocks, hashRateGblocks, ip);
                    }
                    catch (Exception ex)
                    {
                        return new PoolErrorResponse(ex.Message);
                    }
                }
            }

            try
            {
                Block block = Node.Instance.CurrentBlock;
                Argon2Params argon2Params = new Argon2Params(block.Type);
                int limit = LimitCalculator.Instance.GetLimit(wallet, block.Type);

                return new InfoResponse(block, argon2Params, limit);
            }
            catch (Exception ex)
            {
                return new PoolErrorResponse(ex.Message);
            }
        }

        public PoolResponse SubmitNonce(string wallet, string heightStr, string argon, string nonce, string private_key, string public_key, int ip)
        {
            var validWallet = Validation.Instance.ValidateWallet(wallet);
            if (validWallet.Status != ValidationStatus.Success)
            {
                return new PoolErrorResponse(validWallet.Message);
            }

            var validHeight = Validation.Instance.ValidateNonceBlockHeight(heightStr);
            if (validHeight.Status != ValidationStatus.Success)
            {
                return new PoolErrorResponse(validHeight.Message);
            }

            Block block;
            try
            {
                block = Node.Instance.CurrentBlock;
            }
            catch (Exception ex)
            {
                return new PoolErrorResponse("Error retrieving current block: " + ex.Message);
            }

            var validNonce = Validation.Instance.ValidateNonce(ip, wallet, argon, nonce, block);
            if (validNonce.Status != ValidationStatus.Success)
            {
                if (validNonce.Status == ValidationStatus.Error)
                {
                    try {
                        DataStorage.Instance.UpdateAbuse(ip, wallet);
                    }
                    catch (Exception) { }
                }
                return new PoolErrorResponse(validNonce.Message);
            }

            string argonHash = String.Empty;
            string baseHash = String.Empty;
            Argon2Params argon2Params = new Argon2Params(block.Type);

            argonHash = argon2Params.Prefix + argon;
            baseHash = Configuration.PoolPublicKey + "-" + nonce + "-" + block.Id + "-" + block.Difficulty;

            var hash = Encoding.ASCII.GetBytes(baseHash + argonHash);
            SHA512 shaM = new SHA512Managed();
            for (var i = 0; i < 6; i++)
            {
                hash = shaM.ComputeHash(hash);
            }

            string durationString = ((int)hash[10]).ToString() + ((int)hash[15]).ToString() + ((int)hash[20]).ToString() + ((int)hash[23]).ToString() +
                              ((int)hash[31]).ToString() + ((int)hash[40]).ToString() + ((int)hash[45]).ToString() + ((int)hash[55]).ToString();
            durationString = durationString.TrimStart('0');

            BigInteger duration;
            if (!BigInteger.TryParse(durationString, out duration))
            {
                return new PoolErrorResponse("Error validating nonce: error parsing BigInteger from " + durationString + ".");
            }

            var result = BigInteger.Divide(duration, block.IntegerDifficulty);
            int limit = LimitCalculator.Instance.GetLimit(wallet, block.Type, false);

            if (result <= limit)
            {
                if (result <= 240)
                {
                    bool succeded = false;
                    try
                    {
                        succeded = Node.Instance.SubmitBlock(nonce, argon);
                    }
                    catch (Exception ex)
                    {
                        return new PoolErrorResponse("Error submiting nonce: " + ex.Message);
                    }

                    if (!succeded)
                    {
                        return new PoolErrorResponse("Error submiting nonce: block changed.");
                    }

                    Block newBlock;
                    try
                    {
                        newBlock = Node.Instance.CurrentBlock;
                    }
                    catch (Exception ex) //this shouldn't happen, add logging
                    {
                        return new PoolErrorResponse("Error retrieving current block: " + ex.Message);
                    }

                    if (newBlock.Height <= block.Height || newBlock.Generator != Configuration.PoolAddress)
                    {
                        return new PoolErrorResponse("Error submiting nonce: block changed.");
                    }

                    if (newBlock.Reward == 0)
                    {
                        return new PoolErrorResponse("Error retrieving block reward.");
                    }

                    try
                    {
                        DataStorage.Instance.SaveBlock(newBlock.Id, newBlock.Height, newBlock.Reward, wallet);
                    }
                    catch (Exception ex)
                    {
                        return new PoolErrorResponse("Error saving block reward: " + ex.Message);
                    }
                }

                try
                {
                    DataStorage.Instance.SaveShare(wallet, nonce, argon, block.Height, (int)result);
                }
                catch (Exception ex)
                {
                    return new PoolErrorResponse("Error saving submitted share: " + ex.Message);
                }

                return new ShareAcceptedResponse();
            }
            else
            {
                try
                {
                    DataStorage.Instance.UpdateAbuse(ip, wallet);
                }
                catch (Exception) { }

                return new PoolErrorResponse("Error validating nonce: deadline exceeds accepted value.");
            }
        }

        public PoolResponse SubmitData(string payload)
        {
            /*
                [
                    {
                        type : "CPU",
                        subtype : "CPU",
                        name : "Intel i7 7700k",
                        id : 1,
                        cblocks_hashrate : 25,
                        gblocks_hashrate : 500,
                        cblocks_intensity : 100,
                        gblocks_intensity : 35
                    },
                    {
                        type : "GPU",
                        subtype : "CUDA",
                        name : "GeForce 1050 GTX 2GB",
                        id : 1,
                        cblocks_hashrate : 25,
                        gblocks_hashrate : 500,
                        cblocks_intensity : 100,
                        gblocks_intensity : 35
                    },
                    {
                        type : "GPU",
                        subtype : "CUDA",
                        name : "GeForce 1050 GTX 2GB",
                        id : 2,
                        cblocks_hashrate : 25,
                        gblocks_hashrate : 500,
                        cblocks_intensity : 100,
                        gblocks_intensity : 35
                    },

                ]
                */
            
            if (String.IsNullOrEmpty(payload))
            {
                return new PoolErrorResponse("Received payload is empty.");
            }

            throw new NotImplementedException();
        }
    }
}
