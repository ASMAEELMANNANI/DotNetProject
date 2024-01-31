using ProductsCRUDMVC.Models;


namespace ProductsCRUDMVC.Interfaces
{
	public interface IProductRepository
	{
		Task<IEnumerable<product>> GetAll();
		Task<product> GetById(int id);

		bool Add(product p);
		bool Update(product p);
		bool Delete(product p);
		bool save();
	}
}
 