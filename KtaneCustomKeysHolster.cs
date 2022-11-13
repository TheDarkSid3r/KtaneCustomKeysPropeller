using System;
using System.Collections.Generic;
using System.Text;
using RT.Json;
using RT.Serialization;

namespace KtaneCustomKeys
{
    public static class KtaneCustomKeysHolster
    {
        private static readonly Dictionary<string, Hold> s_storage = new Dictionary<string, Hold>();

        private static readonly Random s_random = new Random();

        private static readonly char[] s_tokenChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();

        private static string CreateCode()
        {
            lock (s_storage)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < 5; i++)
                    builder.Append(s_random.Next(10));

                string code = builder.ToString();
                if (s_storage.ContainsKey(code))
                    return CreateCode();

                return code;
            }
        }

        private static string CreateToken()
        {
            lock (s_storage)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < 15; i++)
                    builder.Append(s_tokenChars[s_random.Next(s_tokenChars.Length)]);

                return builder.ToString();
            }
        }

        public static JsonValue Push(string data)
        {
            lock (s_storage)
            {
                Hold hold = new Hold(data);
                s_storage.Add(hold.Code, hold);
                return ClassifyJson.Serialize(new Dictionary<string, string>
                {
                    { "code", hold.Code },
                    { "token", hold.Token }
                });
            }
        }

        public static bool Has(string code)
        {
            lock (s_storage)
                return s_storage.ContainsKey(code);
        }

        public static string Pull(string code)
        {
            lock (s_storage)
                return s_storage[code].Data;
        }

        public static void Remove(string code)
        {
            lock (s_storage)
                if (s_storage.ContainsKey(code))
                    s_storage.Remove(code);
        }

        public static bool IsAuthorized(string code, string token)
        {
            lock (s_storage)
                return s_storage.ContainsKey(code) && s_storage[code].Token == token;
        }

        private class Hold
        {
            public string Code { get; }

            public string Data { get; }

            public string Token { get; }

            public Hold(string data)
            {
                Code = CreateCode();
                Data = data;
                Token = CreateToken();
            }
        }
    }
}