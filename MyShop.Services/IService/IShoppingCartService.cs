using MyShop.Models;
using MyShop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services.IService
{
	public interface IShoppingCartService : IBaseService<ShoppingCart>
	{
		public void Update(ShoppingCart cart);
		public void Plus(int cartId);
		public void Minus(int cartId);
		public void Remove(int cartId);
		public double GetPriceBasedOnQuantity(ShoppingCart shoppingCart);
		public ShoppingCartVM GetShoppingCartVM();
		public ShoppingCartVM UpdateShoppingCartVM();
		public (ShoppingCartVM, string url) ProcessOrder(ShoppingCartVM shoppingCartVM);
		public void HandlePayment(int id);
	}
}
 