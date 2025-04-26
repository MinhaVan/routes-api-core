using Routes.Data.Context;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Routes.Data.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : Entity
{
    private readonly APIContext _context;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(APIContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T> ObterPorIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task DeletarAsync(int id)
    {
        await _dbSet
            .Where(e => EF.Property<int>(e, "Id") == id)
            .ExecuteUpdateAsync(e => e.SetProperty(e => EF.Property<Domain.Enums.StatusEntityEnum>(e, "Status"), Domain.Enums.StatusEntityEnum.Deletado));
    }

    public async Task<IEnumerable<T>> ObterTodosAsync()
    {
        return await _dbSet.AsSplitQuery().ToListAsync();
    }

    public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(predicate);

        // Aplica os includes se forem fornecidos
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.AsSplitQuery().ToListAsync();
    }

    public async Task<Paginado<T>> BuscarPaginadoAsync(int pagina, int tamanho, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(predicate);

        var quantidade = await query.CountAsync();

        // Aplica os includes se forem fornecidos
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        var data = await query
            .AsSplitQuery()
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync();

        var response = new Paginado<T>(pagina, tamanho, quantidade, data);
        return response;
    }

    public async Task<T> BuscarUmAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(predicate);

        // Aplica os includes se forem fornecidos
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.AsSplitQuery().FirstOrDefaultAsync();
    }

    public async Task AdicionarAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(T entity)
    {
        var result = await _dbSet.FirstOrDefaultAsync(t => t.Id.Equals(entity.Id));
        _dbSet.Entry(result).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AdicionarAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(IEnumerable<T> entities)
    {
        foreach (var item in entities)
        {
            var result = await _dbSet.FirstOrDefaultAsync(t => t.Id.Equals(item.Id));
            _dbSet.Entry(result).CurrentValues.SetValues(item);
        }
        await _context.SaveChangesAsync();
    }
}