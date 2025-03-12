using System.ComponentModel;

namespace UserApi.Repository.Model
{
    public enum SystemUserRolesEnum
    {
        // Have access to everything, only user that can do hard delete
        [Description("Admin")]
        Admin = 1,
        [Description("Regular")]
        // User have access to fetch and soft delete
        Regular = 2,
    }
}
