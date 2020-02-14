namespace UzunTec.API.Authentication.RestAPI.Authentication
{
    // snake case properties for Identity Server Compatbility
    public class TokenData
    {
        public string Access_token { get; }
        public int Expires_in { get; }
        public string Token_type { get; }

        public TokenData(string token, int secondsToExpire, string type)
        {
            this.Access_token = token;
            this.Expires_in = secondsToExpire;
            this.Token_type = type;
        }
    }
}
