using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DataLayer.Repositories.BookAggregation;
using DataLayer.Repositories.CatalogueAggregation;

namespace DataLayer.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private IDbConnection _connection;
    private IDbTransaction _transaction;
    private IBookRepository _bookRepository;
    private ICatalogueRepository _catalogueRepository;
    private bool _dispose;

    public IBookRepository BookRepository => _bookRepository ??= new BookRepository(_transaction);
    public ICatalogueRepository CatalogueRepository => _catalogueRepository ??= new CatalogueRepository(_transaction);

    public UnitOfWork(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
        _connection.Open();
        _transaction = _connection.BeginTransaction();
    }
    ~UnitOfWork()
    {
        dispose(false);
    }

    public void Commit()
    {
        try
        {
            _transaction.Commit();
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();
            ResetRepositories();
        }
    }

    private void ResetRepositories()
    {
        _bookRepository = null!;
        _catalogueRepository = null!;
    }

    public void Dispose()
    {
        dispose(true);
        GC.SuppressFinalize(this);
    }

    private void dispose(bool disposing)
    {
        if (_dispose)
        {
            return;
        }
        if (disposing)
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null!;
            }
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null!;
            }
            _dispose = true;
        }
    }

}