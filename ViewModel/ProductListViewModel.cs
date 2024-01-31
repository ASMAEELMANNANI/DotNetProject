using ProductsCRUDMVC.Data.Enum;

namespace ProductsCRUDMVC.ViewModel
{
	public class ProductListViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public float Price { get; set; }
		public string Color { get; set; }
		public string Image { get; set; }
		public int CategoryId { get; set; }
		public category CategoryName { get; set; }
		public int Quantity { get; set; }
	}
}
