using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ProductsCRUDMVC.Data;
using ProductsCRUDMVC.Data.Enum;
using ProductsCRUDMVC.Interfaces;
using ProductsCRUDMVC.Models;
using ProductsCRUDMVC.ViewModel;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;



namespace ProductsCRUDMVC.Controllers
{
    public class ProductController : Controller

    {
        
        //Ajouter l'interface pour le controller
        private readonly IProductRepository _productRepository;
        //Pour avoir un l'acces au chemin des fichiers roots 
        private readonly IWebHostEnvironment _webHostEnvironment;

		private readonly ILoggerFactory _loggerFactory;

		private readonly IMemoryCache _MemoryCache;



		public ProductController(ApplicationDbContext context, IProductRepository productRepository , IWebHostEnvironment webHostEnvironment, ILoggerFactory loggerFactory, IMemoryCache MemoryCache)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
            _loggerFactory = loggerFactory;
            _MemoryCache = MemoryCache;


		}


        // GET: ProductController
        /*public async Task<ActionResult> Index()
        {
            IEnumerable<product> products = await _productRepository.GetAll();
            return View(products);
        }*/

        [BindProperty(SupportsGet = true)]
        public String des { get; set; }

        [BindProperty(SupportsGet = true)]
        public category cat { get; set; }

        [BindProperty(SupportsGet = true)]
        public float? minPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public float? maxPrice { get; set; }

        public async Task<ActionResult> Index ()
        {
			// Use _loggerFactory to create an ILogger instance
			ILogger logger = _loggerFactory.CreateLogger<ProductController>();

			// Now you can use the logger to log messages
			logger.LogInformation("[LOG MESSAGE : CACHE] This is a Logging Message : Products List is cached since it is fetched for the first time !");

			string cacheKey = $"GetProducts{cat}_{des}_{minPrice}_{maxPrice}";

			Console.WriteLine($"Description: {des}");
            Console.WriteLine($"Category: {cat}");
            Console.WriteLine($"MinPrice: {minPrice}");
            Console.WriteLine($"MaxPrice: {maxPrice}");
            // Retrieve all products initially
            IEnumerable<product> products = await _productRepository.GetAll();

			if (_MemoryCache.TryGetValue(cacheKey, out List<product> cachedProducts))
			{
				// Use the cached result
				return View(cachedProducts);
			}

			// Apply filters based on the provided criteria
			if (!string.IsNullOrEmpty(des))
            {
                products = products.Where(p => p.Description.Contains(des));
            }

            if (cat != 0)
            {
                products = products.Where(p => p.Category == cat);
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }


			// Use _loggerFactory to create an ILogger instance
            ILogger loggerF = _loggerFactory.CreateLogger<ProductController>();

			// Now you can use the logger to log messages
			loggerF.LogInformation("[LOG MESSAGE : CACHE] This is a Logging Message : Products List is cached since it is fetched for the first time !");

			// Store the result in the cache with a specific expiration time (e.g., 10 minutes)
			_MemoryCache.Set(cacheKey, products, TimeSpan.FromMinutes(10));


			return View(products);
        }


        // GET: ProductController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            product Product = await _productRepository.GetById(id);
            return View(Product);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            return View();
        }

        // Property to represent the uploaded image file
        [BindProperty]
        public IFormFile Image { get; set; }
        // POST: ProductController/Create
        //Create a new prroduct using http post
        [HttpPost]
        public async Task<ActionResult> Create(product p)
        {
            Console.WriteLine("/*****************Hi from Create************/");
         
            if (!ModelState.IsValid)
            {
                Console.WriteLine("/*****************Le model n'est pas valide************/");
                Console.WriteLine($"Image: {p.Image}");
                Console.WriteLine($"Name: {p.Name}");
                Console.WriteLine($"Description: {p.Description}");
                Console.WriteLine($"Cat: {p.Category}");
                Console.WriteLine($"Stock: {p.Stock}");
                Console.WriteLine($"price: {p.Price}");
                return View(p);
            }

            


            if (Image != null && Image.Length > 0)
            {
                // Generate a unique filename using a timestamp
                var fileName = DateTime.Now.Ticks + Path.GetExtension(Image.FileName);

                //search aboit le file root
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Ensure the uploads folder exists
                Directory.CreateDirectory(uploadsFolder);

                // Save the file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }

                // Save the file path in your database
                p.Image = "/images/" + fileName; // Update the path as per your project structure
            }
            _productRepository.Add(p);
            return RedirectToAction("Index");
        }


        // GET: ProductController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            product Product = await _productRepository.GetById(id);
            return View(Product);
        }

      
        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(product p)

        {
          

           
            if (!ModelState.IsValid)
                {
                     return View(p);
                }

            if (Image != null && Image.Length > 0)
            {
                // Generate a unique filename using a timestamp
                var fileName = DateTime.Now.Ticks + Path.GetExtension(Image.FileName);

                //search aboit le file root
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Ensure the uploads folder exists
                Directory.CreateDirectory(uploadsFolder);

                // Save the file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }

                // Save the file path in your database
                p.Image = "/images/" + fileName; // Update the path as per your project structure
            }

            // Ajoutez la logique pour mettre à jour les détails du produit dans la base de données.
            _productRepository.Update(p);

            return RedirectToAction("Index");
        }


       
        public async Task<ActionResult> Delete(int id)
        {

                // Récupérez le produit depuis la base de données
                product productToDelete = await _productRepository.GetById(id);

                if (productToDelete == null)
                {
                    return NotFound();
                }

                // Supprimez le produit de la base de données directement
                _productRepository.Delete(productToDelete);

                return RedirectToAction("Index");
           
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult AddToCart(int productId, int quantity)
		{
			try
			{
				// Retrieve the shopping cart from cookies or create a new one
				var shoppingCart = HttpContext.Request.Cookies.ContainsKey("ShoppingCart")
					? JsonConvert.DeserializeObject<ShoppingCart>(HttpContext.Request.Cookies["ShoppingCart"])
					: new ShoppingCart();

				// Add the selected product and quantity to the shopping cart
				shoppingCart.AddProduct(productId, quantity);

               
                // Serialize the shopping cart to JSON
                var shoppingCartJson = JsonConvert.SerializeObject(shoppingCart);

				// Set cookie options for the shopping cart
				var cookieOptions = new CookieOptions
				{
					Expires = DateTime.Now.AddDays(7),
					HttpOnly = true,
					Path = "/"
				};

				// Update the shopping cart in cookies and session
				Response.Cookies.Append("ShoppingCart", shoppingCartJson, cookieOptions);

                
                HttpContext.Session.SetString("ShoppingCart", shoppingCartJson);

                
                // Log the addition of products to the shopping cart
                //_logger.LogInformation($"\x1b[32m**********Added {quantity} {(quantity > 1 ? "items" : "item")} to the shopping cart.**********\x1b[0m");

                // Provide a success message to be displayed
                TempData["SuccessMsg"] = $"Added {quantity} {(quantity > 1 ? "items" : "item")} to the shopping cart.";

				// Redirect to the shopping cart view
				return RedirectToAction("ViewCart");
			}
			catch (Exception ex)
			{
				// Log errors and redirect to the error page
				//_logger.LogError(ex, "\x1b[31mError in AddToCart action: {ErrorMessage}\x1b[0m", ex.Message);
				TempData["ErrorMsg"] = "An error occurred while adding the product to the shopping cart.";
				return RedirectToAction("Index");
			}
		}


        // Action to display the shopping cart
        // Action to display the shopping cart

        public IActionResult ViewCart()
        {
            try
            {
                // Retrieve the shopping cart from cookies or create a new one
                var shoppingCart = HttpContext.Request.Cookies.ContainsKey("ShoppingCart")
                    ? JsonConvert.DeserializeObject<ShoppingCart>(HttpContext.Request.Cookies["ShoppingCart"])
                    : new ShoppingCart();

                // Set ViewBag.CartProducts to be used in the View
                ViewBag.CartProducts = shoppingCart;

                // ... (autres parties de votre code)

                // Return the ViewCart view with the updated shopping cart
                return View("ViewCart", shoppingCart);
            }
            catch (Exception ex)
            {
                // Log an error message if an exception occurs during the ViewCart action
                TempData["ErrorMsg"] = "An error occurred while retrieving the shopping cart.";

                // Return the Index view with the product list view models
                return View("Index");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int productId)
        {
            Console.WriteLine("/*****************Hi From Remove************/");
            try
            {
                // Retrieve the shopping cart from cookies or create a new one
                var shoppingCart = HttpContext.Request.Cookies.ContainsKey("ShoppingCart")
                    ? JsonConvert.DeserializeObject<ShoppingCart>(HttpContext.Request.Cookies["ShoppingCart"])
                    : new ShoppingCart();

                // Remove the selected product from the shopping cart
                shoppingCart.RemoveProduct(productId);

                // Serialize the shopping cart to JSON
                var shoppingCartJson = JsonConvert.SerializeObject(shoppingCart);

                // Set cookie options for the shopping cart
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Path = "/"
                };

                // Update the shopping cart in cookies and session
                Response.Cookies.Append("ShoppingCart", shoppingCartJson, cookieOptions);
                HttpContext.Session.SetString("ShoppingCart", shoppingCartJson);

                // Provide a success message to be displayed
                TempData["SuccessMsg"] = "Product removed from the shopping cart.";

                // Redirect to the shopping cart view
                return RedirectToAction("ViewCart");
            }
            catch (Exception ex)
            {
                // Log errors and redirect to the error page
                //_logger.LogError(ex, "\x1b[31mError in RemoveFromCart action: {ErrorMessage}\x1b[0m", ex.Message);
                TempData["ErrorMsg"] = "An error occurred while removing the product from the shopping cart.";
                return RedirectToAction("ViewCart");
            }
        }




    }

    // POST action to handle adding a product to the shopping cart






}
