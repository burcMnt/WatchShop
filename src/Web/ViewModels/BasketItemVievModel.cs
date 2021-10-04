namespace Web.ViewModels
{
    public class BasketItemVievModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string PictureUri { get; set; }
    }
}