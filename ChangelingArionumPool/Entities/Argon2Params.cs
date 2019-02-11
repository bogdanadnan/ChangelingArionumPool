using System;
namespace ChangelingArionumPool.Entities
{
    public class Argon2Params
    {
        public Argon2Params(BlockType type)
        {
            switch(type)
            {
                case BlockType.GpuBlock:
                    Memory = 16384;
                    Threads = 4;
                    Time = 4;
                    Prefix = "$argon2i$v=19$m=16384,t=4,p=4";
                    break;
                default:
                    Memory = 524288;
                    Threads = 1;
                    Time = 1;
                    Prefix = "$argon2i$v=19$m=524288,t=1,p=1";
                    break;
            }
        }

        public int Memory { get; private set; }
        public int Threads { get; private set; }
        public int Time { get; private set; }
        public string Prefix { get; private set; }
    }
}
