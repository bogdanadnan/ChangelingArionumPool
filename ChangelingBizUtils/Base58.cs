using System;

namespace ChangelingBizUtils
{
    public class Base58
    {
        private const string alphabet = "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";

        public static bool IsValid(string base58)
        {
            for(int i=0; i<base58.Length; i++)
            {
                if(!alphabet.Contains(base58[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
