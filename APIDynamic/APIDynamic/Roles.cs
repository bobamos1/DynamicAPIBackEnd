using DynamicStructureObjects;

namespace APIDynamic
{
    public enum Roles
    {
        [Value("Client")]
        Client = 1,
        [Value("Admin")]
        Admin = 2
    }

    public static class RoleHelper
    {

        public static long ID(this Roles role) => (long)role;
        public static KeyValuePair<long, bool> CanModify(this Roles role) => new KeyValuePair<long, bool>((long)role, true);
        public static KeyValuePair<long, bool> CanNotModify(this Roles role) => new KeyValuePair<long, bool>((long)role, false);

    }
}
