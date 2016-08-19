namespace Model.Entity
{
    public enum ListingStatus
    {
        Listed = 0,
        DelistedForAllCustomers = 1,
        DelistedForSomeCustomers = 2   // <- but not for all!
    }
}
