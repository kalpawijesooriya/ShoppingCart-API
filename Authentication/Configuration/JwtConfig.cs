namespace Authentication;

    public class JwtConfig
    {
        public string Secrect { get; set; }
        public TimeSpan ExpiryTimeFrame { get; set; }
    }

