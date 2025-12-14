using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models;
using TestProject.Dtos;


namespace TestProject.Services;

public class ProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    // READ: all products
    public async Task<List<Product>> GetAllAsync()
    {
        return await _db.Products
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize)
        {
            var query = _db.Products.AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }



    // READ: single product
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _db.Products.FindAsync(id);
    }

    // CREATE
    public async Task<Product> CreateAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    // UPDATE
    public async Task<bool> UpdateAsync(int id, Product updated)
    {
        var existing = await _db.Products.FindAsync(id);
        if (existing is null)
            return false;

        existing.Name = updated.Name;
        existing.Price = updated.Price;

        await _db.SaveChangesAsync();
        return true;
    }

    // DELETE
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null)
            return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }
}
