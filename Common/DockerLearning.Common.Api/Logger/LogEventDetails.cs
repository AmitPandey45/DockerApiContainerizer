namespace DockerLearning.Common.Api.Logger
{
    public class LogEventDetails
    {
        public string TimeStamp { get; set; }
        public string LogLevel { get; set; }
        public string LogInfo { get; set; }
        public string? UniqueRequestId { get; set; }
        public int ThreadId { get; set; }
        public string TenantId { get; set; }
        public string AccountId { get; set; }
        public string? UserId { get; set; }
        public string? IdentityDelegationUserId { get; set; }
        public string? Api { get; set; }
        public string? HttpMethod { get; set; }
        public string? HostName { get; set; }
        public int? HttpStatusCode { get; set; }
        public string Message { get; set; }
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
    }
}
