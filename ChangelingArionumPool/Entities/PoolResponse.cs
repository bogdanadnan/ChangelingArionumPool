using ChangelingArionumPool.Services;

namespace ChangelingArionumPool.Entities
{
    public class PoolResponse
    {
        public string status { get; set; } // ok
        public object data { get; set; } // data
        public string coin { get; set; } // arionum
        public string pool 
        {
            get
            {
                return "changeling - 0.1";
            }
        } // pool version (extension to protocol)
    }

    public class PoolErrorResponse : PoolResponse
    {
        public PoolErrorResponse(string error)
        {
            status = "error";
            coin = "arionum";
            data = error;
        }
    }

    public class InfoResponse : PoolResponse
    {
        public InfoResponse(Block block, Argon2Params argon2Params, int limit)
        {
            status = "ok";
            coin = "arionum";
            data = new Info
            {
                recommendation = "mine",
                argon_mem = argon2Params.Memory,
                argon_threads = argon2Params.Threads,
                argon_time = argon2Params.Time,
                difficulty = block.Difficulty,
                block = block.Id,
                height = block.Height,
                public_key = Configuration.PoolPublicKey,
                limit = limit
            };
        }

        public class Info
        {
            public string recommendation { get; set; } // mine
            public int argon_mem { get; set; } // 16384
            public int argon_threads { get; set; } // 4
            public int argon_time { get; set; } // 4
            public string difficulty { get; set; } // 3759407
            public string block { get; set; } // ULhEHteTgy9ksmohmKtvVUwnYggwXuRdrVc8kCXKraDJx92Qu47jejqMFyqzWuQMcJe7XZscd2vU1t87Aev4GhJ
            public int height { get; set; } // 138699
            public string public_key { get; set; } // PZ8Tyr4Nx8MHsRAGMpZmZ6TWY63dXWSCy7AEg3h9oYjeR74yj73q3gPxbxq9R3nxSSUV4KKgu1sQZu9Qj9v2q2HhT5H3LTHwW7HzAA28SjWFdzkNoovBMncD
            public int limit { get; set; } // 30000
        }
    }

    public class ShareAcceptedResponse : PoolResponse
    {
        public ShareAcceptedResponse()
        {
            status = "ok";
            coin = "arionum";
            data = "accepted";
        }
    }
}