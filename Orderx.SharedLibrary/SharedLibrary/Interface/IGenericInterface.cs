﻿using SharedLibrary.Responses;
using System.Linq.Expressions;

namespace SharedLibrary.Interfaces
{
    public interface IGenericInterface<T> where T: class
    {
        Task<Response> CreateAsync(T entity);
        Task<Response> UpdateAsync(T entity);
        Task<Response> DeleteAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindByIdAsync(int id);
        Task<T> GetByAsync(Expression<Func<T, bool>> expression);
    }
}
