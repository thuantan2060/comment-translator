using System;

namespace TranslateClient.Common
{
    public class GoogleToken
    {
        private DateTime _expireTime { get; set; }
        public string Value { get; set; }
        public GoogleToken(string Token)
        {
            Value = Token;
            _expireTime = DateTime.Now.AddHours(1);
        }

        public bool IsExpire()
        {
            return DateTime.Now < _expireTime;
        }
    }
}
