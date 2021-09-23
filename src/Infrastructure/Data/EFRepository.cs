using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class EFRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;

        public EFRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public Task CountAsync(Ardalis.Specification.ISpecification<T> spec)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<T> FirstAsync(Ardalis.Specification.ISpecification<T> spec)
        {
            throw new NotImplementedException();
        }

        public Task<T> FirstOrDefaultAsync(Ardalis.Specification.ISpecification<T> spec)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public Task<List<T>> ListAllAsync(Ardalis.Specification.ISpecification<T> spec)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
