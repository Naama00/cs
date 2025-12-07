namespace DO
{
    public record Sale(int Id, int ProductId, int RequiredQuantity, double DiscountedPrice, bool IsForClubMembers, DateTime SaleStartDate, DateTime SaleEndDate)
    {
        
        public Sale() : this(0, 0, 0, 0.0, false, DateTime.MinValue, DateTime.MinValue)
        {
        }
    }
}