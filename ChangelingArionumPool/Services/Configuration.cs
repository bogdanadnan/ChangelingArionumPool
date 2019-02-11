using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ChangelingArionumPool.Services
{
    public static class Configuration
    {
        static Configuration()
        {
            DataStorageConnString = ConfigurationManager.ConnectionStrings["DataStorage"].ConnectionString;
            PoolAddress = ConfigurationManager.AppSettings["PoolAddress"];
            PoolPublicKey = ConfigurationManager.AppSettings["PoolPublicKey"];
            PoolPrivateKey = ConfigurationManager.AppSettings["PoolPrivateKey"];
            BlockRefreshInterval = Int32.Parse(ConfigurationManager.AppSettings["BlockRefreshInterval"]);
            NodeUrl = ConfigurationManager.AppSettings["NodeUrl"].Split(';').Select(s => ("http://" + s.Trim() + "/")).ToList();
            CBlocksStartLimit = Int32.Parse(ConfigurationManager.AppSettings["CBlocksStartLimit"]);
            GBlocksStartLimit = Int32.Parse(ConfigurationManager.AppSettings["GBlocksStartLimit"]);
            ArgonValidationServiceUrl = ConfigurationManager.AppSettings["ArgonValidationServiceUrl"].Split(';').Select(s => ("http://" + s.Trim() + "/")).ToList();
            ShareValueProcessingInterval = Int32.Parse(ConfigurationManager.AppSettings["ShareValueProcessingInterval"]);
            PaymentProcessingInterval = Int32.Parse(ConfigurationManager.AppSettings["PaymentProcessingInterval"]);
        }

        public static string DataStorageConnString { get; private set; }
        public static string PoolAddress { get; private set; }
        public static string PoolPublicKey { get; private set; }
        public static string PoolPrivateKey { get; private set; }
        public static int BlockRefreshInterval { get; private set; }
        public static List<string> NodeUrl { get; private set; }
        public static int CBlocksStartLimit { get; private set; }
        public static int GBlocksStartLimit { get; private set; }
        public static List<string> ArgonValidationServiceUrl { get; private set; }
        public static int ShareValueProcessingInterval { get; private set; }
        public static int PaymentProcessingInterval { get; private set; }
    }
}
