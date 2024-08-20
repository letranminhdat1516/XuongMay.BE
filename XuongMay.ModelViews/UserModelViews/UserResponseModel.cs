namespace XuongMay.ModelViews.UserModelViews
{
    public class UserResponseModel
    {
        public DateTimeOffset? DeletedTime;

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public bool IsDelete { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string? DeletedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? CreatedBy { get; set; }

    }
}
