using System;
using System.Numerics;
using ChangelingArionumPool.Services;

namespace ChangelingArionumPool.Entities
{
    public class Block
    {
        public string Id { get; set; }
        public string Generator { get; set; }
        public int Height { get; set; }
        public BlockType Type { get; set; }
        public BigInteger IntegerDifficulty { get; set; }

        private string difficulty;
        public string Difficulty
        {
            get
            {
                return difficulty;
            }
            set
            {
                difficulty = value;
                IntegerDifficulty = BigInteger.Parse(value);
            }
        }

        private decimal? reward;
        public decimal Reward
        {
            get
            {
                if(!reward.HasValue)
                {
                    try
                    {
                        reward = Node.Instance.GetBlockReward(Height);
                    }
                    catch(Exception)
                    { 
                        reward = 0; 
                    }
                }
                return reward.Value;
            }
            set
            {
                reward = value;
            }
        }
    }

    public enum BlockType
    {
        CpuBlock = 0,
        GpuBlock = 1
    }
}
