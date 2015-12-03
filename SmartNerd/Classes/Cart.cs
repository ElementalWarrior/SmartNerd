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
        private List<OrderProduct> _products;

        public Cart()
        {
            _context = new SmartNerdDataContext();
            _order = new DataModels.Order();
            _context.Orders.InsertOnSubmit(_order);
        }
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
        public Guid CartID
        {
            get
            {
                return _order.CartID;
            }
        }
        public Guid? AccountID
        {
            get
            {
                return _order.AccountID;
            }
            set
            {
                _order.AccountID = value;
            }
        }
        public decimal Total
        {
            get
            {
                return _order.OrderTotal;
            }
        }
        public int? AddressID
        {
            get
            {
                return _order.AddressID;
            }
            set
            {
                _order.AddressID = value;
            }
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
                                 select new OrderProduct(op)).ToList();
                }
                return _products.ToList();
            }
            //we'll use methods to modify the products 
        }
        public void AddProduct(Models.Menu.Product prod)
        {
            OrderProduct classOp = Products.FirstOrDefault(p => p.ProductID == prod.ProductID);
            if (classOp == null)
            {
                DataModels.OrderProduct op = new DataModels.OrderProduct
                {
                    Quantity = prod.Quantity,
                    ProductID = prod.ProductID,
                    OrderID = _order.OrderID
                };
                _context.OrderProducts.InsertOnSubmit(op);
                OrderProduct orderProd = new OrderProduct(op);
                orderProd.Price = (from p in _context.Products
                                   where p.ProductID == prod.ProductID
                                   select p.Price).First();
                Products.Add(orderProd);
            }
            else
            {
                classOp.Quantity++;
            }
        }
        public void RemoveProduct(int productID)
        {
            List<DataModels.OrderProduct> ops = (from op in _context.OrderProducts
                                                 where op.ProductID == productID
                                                 select op).ToList();
            _context.OrderProducts.DeleteAllOnSubmit(ops);
        }
        public void UseNewAddress(Address newAddress)
        {
            SmartNerdDataContext _context2 = new SmartNerdDataContext();
            DataModels.Address _newAddress = new DataModels.Address
            {
                FullName = newAddress.FullName,
                City = newAddress.City,
                Line1 = newAddress.Line1,
                Line2 = newAddress.Line2,
                County = newAddress.County,
                StateOrProvince = newAddress.StateOrProvince,
                ZipCode = newAddress.ZipCode,
            };
            _context2.Addresses.InsertOnSubmit(_newAddress);
            _context2.SubmitChanges();
            _order.AddressID = _newAddress.AddressID;
        }
        public void Save()
        {
            if(_order.AccountID != null) {
                decimal total = 0;
                foreach(OrderProduct p in Products)
                {
                    total += p.Price * p.Quantity;
                }
                _order.OrderTotal = total;


                _context.SubmitChanges();
            }
        }
        #endregion
    }
}