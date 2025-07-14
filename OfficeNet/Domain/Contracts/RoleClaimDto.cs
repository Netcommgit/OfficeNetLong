namespace OfficeNet.Domain.Contracts
{
    public class RoleClaimDto
    {
        public long RoleId { get; set; }
        public string ClaimType { get; set; } = "Permission";
        public string ClaimValue { get; set; }
    }

    public class AddRolePermissionsDto
    {
        public long RoleId { get; set; }
        public List<string> Permissions { get; set; }
    }

    public class RoleWithClaimsDto
    {
        public string RoleName { get; set; }
        public List<string> Permissions { get; set; }
    }

}
