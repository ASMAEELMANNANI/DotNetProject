using Microsoft.EntityFrameworkCore;
using ProductsCRUDMVC.Data;
using ProductsCRUDMVC.Interfaces;
using ProductsCRUDMVC.Models;

namespace ProductsCRUDMVC.Repository
{
	public class ProductRepository : IProductRepository
	{
		public readonly ApplicationDbContext _context;
		public ProductRepository(ApplicationDbContext context)
		{ 
			_context = context;
		}	
		public bool Add(product p)
		{
			//Generate the SQL to add a product : that's doen't mean that 
			_context.Add(p);
			return save();
		}

		public bool Delete(product p)
		{
			_context.Remove(p);
			return save();
		}

		//async : La programmation asynchrone est utilisée pour éviter le blocage du thread lors de l'exécution d'opérations longues ou bloquantes, telles que les opérations d'entrée-sortie (I/O) dans les applications web, comme l'accès à une base de données.
		//await : Le mot clé await est utilisé pour indiquer au programme de suspendre l'exécution de la méthode jusqu'à ce que l'opération asynchrone soit terminée. 
		//Task : indique que la méthode peut être exécutée de manière asynchrone, ce qui peut être utile dans des applications qui nécessitent une exécution non bloquante.
		public async Task<IEnumerable<product>> GetAll()
		{
			return await _context.Products.ToListAsync();
		}

		public async Task<product> GetById(int id)
		{
			return await _context.Products.FirstOrDefaultAsync(i => i.IdProd == id);
		}

		public bool save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}	

		public bool Update(product p)
		{
			var updated = _context.Update(p);
            return save();
		}

		
	}
}
