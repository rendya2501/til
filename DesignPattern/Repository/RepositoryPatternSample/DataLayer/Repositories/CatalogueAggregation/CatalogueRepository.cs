using DataLayer.Entities;

namespace DataLayer.Repositories.CatalogueAggregation
{
    public class CatalogueRepository : ICatalogueRepository
    {
        public Task<int> AddAsync(Catalogue entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Catalogue>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Catalogue> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Catalogue entity)
        {
            throw new NotImplementedException();
        }
    }
}