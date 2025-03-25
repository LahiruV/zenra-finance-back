using System;
using zenra_finance_back.Data;
using zenra_finance_back.Models;
using zenra_finance_back.Services.IServices;

namespace zenra_finance_back.Services
{
    public class YourEntityService : IYourEntityService
    {
        private readonly AppDbContext _context;

        public YourEntityService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<YourEntity> GetAll()
        {
            return _context.YourEntities.ToList();
        }

        public YourEntity GetById(int id)
        {
            return _context.YourEntities.SingleOrDefault(e => e.Id == id);
        }

        public void Create(YourEntity entity)
        {
            _context.YourEntities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(YourEntity entity)
        {
            _context.YourEntities.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.YourEntities.SingleOrDefault(e => e.Id == id);
            if (entity != null)
            {
                _context.YourEntities.Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}
