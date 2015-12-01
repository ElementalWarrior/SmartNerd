using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartNerd
{
    public class Cart
    {
        private SmartNerdDataContext _context;
        private DataModels.Order _order;
        private IEnumerable<OrderProduct> _products;

        public Cart(Guid cartID)
        {
            _context = new SmartNerdDataContext();
            _order = (from o in _context.Orders
                      where o.CartID == cartID
                      select o).First();
        }

        #region Properties
        public int OrderID {
            get {
                return _order.OrderID;
            }
            //we don't ever want to change this, so no set
        }
        #endregion

        #region "Public Methods"
        public List<OrderProduct> Products
        {
            get
            {
                if (_products == null)
                {
                    _products = (from op in _context.OrderProducts
                                 where op.OrderID == _order.OrderID
                                 select new OrderProduct(op));
                }
                return _products.ToList();
            }
            //we'll use methods to modify the products 
        }
        public void Save()
        {
            decimal total = 0;
            foreach(OrderProduct p in Products)
            {
                total += p.Price * p.Quantity;
            }
            _order.OrderTotal = total;
            _context.SubmitChanges();
        }
        #endregion
    }
}