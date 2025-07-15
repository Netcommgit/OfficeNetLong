namespace OfficeNet.Service.Permission
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(string userId, string permission);
    }
}
