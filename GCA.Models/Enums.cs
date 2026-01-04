namespace GCA.Models
{
    public enum PartState
    {
        Available,
        Sold,
        Damaged
    }

    public enum UserRole
    {
        Admin,
        Employee
        // Client/Supplier roles might be external users, but for now app users are Admin/Employee
    }
}
