namespace RepositoryPatternSample.Entities;
public class BankBranch
{
    public int CatalogueId { get; set; }
    public string Name { get; set; }
    public List<Book> Books { get; set; }
}
