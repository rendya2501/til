using Microsoft.AspNetCore.Mvc;
using DataLayer.Entities;
using DataLayer.Repositories.BookAggregation;

namespace Presentation.Controllers;

[Route("api/[controller]")]
public class BookController : Controller
{
    private readonly IBookRepository _bookRepository;
    public BookController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _bookRepository.GetAllAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _bookRepository.GetByIdAsync(id);
        if (data == null) return Ok();
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Add(Book product)
    {
        var data = await _bookRepository.AddAsync(product);
        return Ok(data);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _bookRepository.DeleteAsync(id);
        return Ok(data);
    }

    [HttpPut]
    public async Task<IActionResult> Update(Book product)
    {
        var data = await _bookRepository.UpdateAsync(product);
        return Ok(data);
    }
}
