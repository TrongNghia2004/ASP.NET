namespace luongtrongnghia.Model
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        // public double TotailPrice { get; set; }
         public int? Quantity { get; set; }  


    }
}
