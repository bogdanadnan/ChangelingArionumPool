using System;
using ChangelingArionumPool.Entities;

namespace ChangelingArionumPool.Services
{
    public class LimitCalculator
    {
        private static readonly Lazy<LimitCalculator> _instance =
            new Lazy<LimitCalculator>(() => new LimitCalculator());

        public static LimitCalculator Instance { get { return _instance.Value; } }

        private LimitCalculator() { }

        public int GetLimit(string wallet, BlockType blockType, bool adjust = true)
        {
            switch (blockType)
            {
                case BlockType.CpuBlock:
                    return Configuration.CBlocksStartLimit;
                case BlockType.GpuBlock:
                    return Configuration.GBlocksStartLimit;
            }
            return 0;
        }
    }
}
