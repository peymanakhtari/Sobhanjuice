using SobhanJuice.Data;
using SobhanJuice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadicalTherapy.Data.Repository
{
    public class UnitOfWork : IDisposable
    {
        private MyContext context = new MyContext();
        private GenericRepository<Comment> _commentRepository;
        private GenericRepository<KeyValue> _keyValueRepository;
        private GenericRepository<Order> _orderRepository;
        private GenericRepository<Product> _ProductRepository;
        private GenericRepository<OrderDetail> _orderDetailRepository;
        private GenericRepository<User> _userRepository;
        private GenericRepository<Category> _catogoryRepository;
        private GenericRepository<Address> _addressRepository;

        private bool disposed = false;
        public GenericRepository<Address> AddressRepository
        {
            get
            {
                if (this._addressRepository == null)
                {
                    this._addressRepository = new GenericRepository<Address>(context);
                }
                return _addressRepository;
            }
        }
        public GenericRepository<Comment> CommentRepository
        {
            get
            {
                if (this._commentRepository == null)
                {
                    this._commentRepository = new GenericRepository<Comment>(context);
                }
                return _commentRepository;
            }
        }
        public GenericRepository<KeyValue> KeyValueRepository
        {
            get
            {
                if (this._keyValueRepository == null)
                {
                    this._keyValueRepository = new GenericRepository<KeyValue>(context);
                }
                return _keyValueRepository;
            }
        }
        public GenericRepository<Order> OrderRepository
        {
            get
            {
                if (this._orderRepository == null)
                {
                    this._orderRepository = new GenericRepository<Order>(context);
                }
                return _orderRepository;
            }
        }
        public GenericRepository<User> UserRepository
        {
            get
            {
                if (this._userRepository == null)
                {
                    this._userRepository = new GenericRepository<User>(context);
                }
                return _userRepository;
            }
        }
        public GenericRepository<OrderDetail> OrderDetailRepository
        {
            get
            {
                if (this._orderDetailRepository == null)
                {
                    this._orderDetailRepository = new GenericRepository<OrderDetail>(context);
                }
                return _orderDetailRepository;
            }
        }
        public GenericRepository<Product> ProductRepository
        {
            get
            {
                if (this._ProductRepository == null)
                {
                    this._ProductRepository = new GenericRepository<Product>(context);
                }
                return _ProductRepository;
            }
        }
        public GenericRepository<Category> CategoryRepository
        {
            get
            {
                if (this._catogoryRepository == null)
                {
                    this._catogoryRepository = new GenericRepository<Category>(context);
                }
                return _catogoryRepository;
            }
        }
        public void Save()
        {
            context.SaveChanges();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
