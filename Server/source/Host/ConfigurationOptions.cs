namespace Host {
    public class ConnectionStringOptions {
        public string NpgSQL { get; set; }
        public string EventStore { get; set; }
    }
    public class EmailServiceOptions {
        /// <summary>
        /// 
        /// </summary>
        public string From { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string SmtpServer { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool UseTls { get; set; } = true;
        
        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

    }
}