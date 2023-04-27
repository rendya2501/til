using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DapperUnitOfWorkSample.UnitOfWork;

public class UnitOfWork: IUnitOfWork
{
    private IDbConnection _connection;
    private IDbTransaction _transaction;

    public IRepository<Invoice> InvoiceRepository { get; }
    public IRepository<InvoiceLine> InvoiceLineRepository { get; }

    public DapperUnitOfWork(IDbConnection connection)
    {
        _connection = connection;
        _transaction = _connection.BeginTransaction();
        InvoiceRepository = new InvoiceRepository(_connection, _transaction);
        InvoiceLineRepository = new InvoiceLineRepository(_connection, _transaction);
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
        }
    }

    public void Rollback()
    {
        _transaction.Rollback();
        _transaction.Dispose();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _transaction = null;
    }
}
