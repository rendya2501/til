using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DapperUnitOfWorkSample.UnitOfWork;

public interface IUnitOfWork
{
    IRepository<Invoice> InvoiceRepository { get; }
    IRepository<InvoiceLine> InvoiceLineRepository { get; }
    void Commit();
    void Rollback();
}
