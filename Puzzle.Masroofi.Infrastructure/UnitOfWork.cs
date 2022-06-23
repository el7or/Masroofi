using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core;
using System;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Infrastructure
{
    public class UnitOfWork : Disposable, IUnitOfWork
    {
        private MasroofiDbContext _dbContext;
        public UnitOfWork(MasroofiDbContext dbCOntext)
        {
            _dbContext = dbCOntext;
        }
        public int Commit()
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    int entries = _dbContext.SaveChanges();
                    transaction.Commit();
                    return entries;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex is DbUpdateConcurrencyException)
                    {
                        throw new DbUpdateConcurrencyException(ex.Message);
                    }
                    return 0;
                }
            }
        }

        public async Task<int> CommitAsync()
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    int entries = await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                    return entries;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex is DbUpdateConcurrencyException)
                    {
                        throw new DbUpdateConcurrencyException(ex.Message);
                    }
                    return 0;
                }
            }
        }

        protected override void DisposeCore()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
        }
    }

    public interface IUnitOfWork : IDisposable
    {
        int Commit();
        Task<int> CommitAsync();
    }

    public class Disposable : IDisposable
    {
        private bool isDisposed;
        ~Disposable()
        {
            Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                DisposeCore();
            }
            isDisposed = true;
        }

        protected virtual void DisposeCore()
        {
        }
    }
}
