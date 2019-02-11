using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using ChangelingArionumPool.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChangelingArionumPool.Services
{
    public class Node
    {
        private static readonly Lazy<Node> _instance =
            new Lazy<Node>(() => new Node());

        public static Node Instance { get { return _instance.Value; } }

        private Node() { }

        private Random _random = new Random();

        private Block _currentBlock;
        private DateTime _currentBlockTimestamp = DateTime.UnixEpoch;
        private object _currentBlockLock = new object();

        private string NodeUrl
        {
            get
            {
                List<string> nodeUrlList = Configuration.NodeUrl;
                return nodeUrlList[_random.Next(nodeUrlList.Count)];
            }
        }

        public Block CurrentBlock
        {
            get
            {
                lock(_currentBlockLock)
                {
                    if((DateTime.Now - _currentBlockTimestamp).TotalMilliseconds > Configuration.BlockRefreshInterval)
                    {
                        UpdateCurrentBlock(NodeUrl);
                        _currentBlockTimestamp = DateTime.Now;
                    }

                    return _currentBlock;
                }
            }
        }

        private void UpdateCurrentBlock(string nodeUrl)
        {
            string url = nodeUrl + "api.php?q=currentBlock";
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.Timeout = 10000;

            var content = String.Empty;
            try
            {
                using (var response = http.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        content = stream.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving current block, node not available.", ex);
            }

            try
            {
                var blockResponse = JsonConvert.DeserializeObject<BlockResponse>(content);

                if (blockResponse.status != "ok" || blockResponse.data == null)
                {
                    throw new Exception("Error retrieving current block, node sent error response.");
                }

                _currentBlock = new Block
                {
                    Id = blockResponse.data.id,
                    Generator = blockResponse.data.generator,
                    Height = blockResponse.data.height,
                    Reward = 0,
                    Type = (blockResponse.data.height >= 80000 && blockResponse.data.height % 2 == 1) ? BlockType.GpuBlock : BlockType.CpuBlock,
                    Difficulty = blockResponse.data.difficulty
                };
            }
            catch (FormatException)
            {
                throw new Exception("Error retrieving current block, received difficulty is invalid.");
            }
            catch (Exception)
            {
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<NodeErrorResponse>(content);
                    throw new Exception("Error retrieving current block, node sent error response: " + errorResponse.data);
                }
                catch (Exception)
                {
                    throw new Exception("Error retrieving current block, node sent invalid response.");
                }
            }
        }

        public bool SubmitBlock(string nonce, string argon)
        {
            var postData = ("argon=" + WebUtility.UrlEncode(argon));
            postData += ("&nonce=" + WebUtility.UrlEncode(nonce));
            postData += ("&private_key=" + WebUtility.UrlEncode(Configuration.PoolPrivateKey));
            postData += ("&public_key=" + WebUtility.UrlEncode(Configuration.PoolPublicKey));
            var data = Encoding.ASCII.GetBytes(postData);

            string nodeUrl = NodeUrl;

            string url = nodeUrl + "mine.php?q=submitNonce";
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.Timeout = 100000;
            http.Method = "POST";
            http.ContentType = "application/x-www-form-urlencoded";
            http.ContentLength = data.Length;

            using (var stream = http.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var content = String.Empty;
            try
            {
                using (var response = http.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        content = stream.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error submiting nonce, node not available.", ex);
            }

            try
            {
                var response = JsonConvert.DeserializeObject<SubmitResponse>(content);

                if (response.status != "ok" || response.data != "accepted")
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw new Exception("Error submiting nonce, node sent invalid response.");
            }

            lock (_currentBlockLock)
            {
                UpdateCurrentBlock(nodeUrl);
                _currentBlock.Reward = GetBlockReward(_currentBlock.Height, nodeUrl);
            }

            return true;
        }

        public decimal GetBlockReward(int height, string nodeUrl = null)
        {
            if(String.IsNullOrEmpty(nodeUrl))
            {
                nodeUrl = NodeUrl;
            }

            string url = nodeUrl + "api.php?q=getBlockTransactions&includeMiningRewards=true&height=" + height.ToString();
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.Timeout = 100000;

            var content = String.Empty;
            try
            {
                using (var response = http.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        content = stream.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving block transactions, node not available.", ex);
            }

            try
            {
                var transactionsResponse = JsonConvert.DeserializeObject<TransactionsResponse>(content);

                if (transactionsResponse.status != "ok" || transactionsResponse.data == null)
                {
                    throw new Exception("Error retrieving block transactions, node sent error response.");
                }

                foreach(TransactionsResponse.TransactionResponseData transaction in transactionsResponse.data)
                {
                    if(transaction.version == 0 && transaction.type == "mining" && transaction.message != "masternode")
                    {
                        return transaction.val;
                    }
                }
                return 0;
            }
            catch (Exception)
            {
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<NodeErrorResponse>(content);
                    throw new Exception("Error retrieving block transactions, node sent error response: " + errorResponse.data);
                }
                catch (Exception)
                {
                    throw new Exception("Error retrieving block transactions, node sent invalid response.");
                }
            }
        }


    }
}
