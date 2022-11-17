namespace DataLayer.Entities;
public class Catalogue
{
    public int CatalogueId { get; set; }
    public string Name { get; set; }
    public List<Book> Books { get; set; }
}
