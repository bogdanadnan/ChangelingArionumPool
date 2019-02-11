using System;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace ChangelingArionumPool.Services
{
    public class DataStorage
    {
        private static readonly Lazy<DataStorage> _instance =
            new Lazy<DataStorage>(() => new DataStorage());

        public static DataStorage Instance { get { return _instance.Value; } }

        private DataStorage() { }

        public void SaveHashRateEntry(string wallet, string worker, int hashRateCblocks, int hashRateGblocks, int ip)
        {
            using (MySqlConnection _db = new MySqlConnection(Configuration.DataStorageConnString))
            {
                _db.Open();
                var param = new DynamicParameters();
                param.Add("_wallet", wallet);
                param.Add("_worker", worker);
                param.Add("_hash_rate_cblocks", hashRateCblocks);
                param.Add("_hash_rate_gblocks", hashRateGblocks);
                param.Add("_ip", ip);

                _db.Execute("saveHashRateEntry", param, commandType: CommandType.StoredProcedure);
            }
        }

        public int CheckAbuse(int ip, string wallet, string argon)
        {
            using (MySqlConnection _db = new MySqlConnection(Configuration.DataStorageConnString))
            {
                _db.Open();
                var param = new DynamicParameters();
                param.Add("_ip", ip);
                param.Add("_wallet", wallet);
                param.Add("_argon", argon);

                return _db.QuerySingle<int>("checkAbuse", param, commandType: CommandType.StoredProcedure);
            }
        }

        public void UpdateAbuse(int ip, string wallet)
        {
            using (MySqlConnection _db = new MySqlConnection(Configuration.DataStorageConnString))
            {
                _db.Open();
                var param = new DynamicParameters();
                param.Add("_ip", ip);
                param.Add("_wallet", wallet);

                _db.Execute("updateAbuse", param, commandType: CommandType.StoredProcedure);
            }
        }

        public void SaveBlock(string id, int height, decimal reward, string winner)
        {
            using (MySqlConnection _db = new MySqlConnection(Configuration.DataStorageConnString))
            {
                _db.Open();
                var param = new DynamicParameters();
                param.Add("_uid", id);
                param.Add("_height", height);
                param.Add("_reward", reward);
                param.Add("_winner", winner);

                _db.Execute("saveBlock", param, commandType: CommandType.StoredProcedure);
            }
        }

        public void SaveShare(string wallet, string nonce, string argon, int height, int deadline)
        {
            using (MySqlConnection _db = new MySqlConnection(Configuration.DataStorageConnString))
            {
                _db.Open();
                var param = new DynamicParameters();
                param.Add("_wallet", wallet);
                param.Add("_nonce", nonce);
                param.Add("_argon", argon);
                param.Add("_block_height", height);
                param.Add("_deadline", deadline);

                _db.Execute("saveShare", param, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
