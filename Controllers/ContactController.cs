using Microsoft.AspNetCore.Mvc;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Application.Services;

namespace PortofolioApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ContactService _service;
    public ContactController(ContactService service)
    {
        _service = service;
    }
    [HttpGet]
    public IEnumerable<ContactDTO> Get()
    {
        return _service.GetContact();  
    }

    
    [HttpGet("{idContact}")]
    public ActionResult<ContactDTO> GetById(int idContact)
    {
        ContactDTO contact = _service.GetContactById(idContact);
        if (contact == null) return NotFound();
        return Ok(contact);
    }

    [HttpPost]
    public IActionResult Add([FromBody] ContactDTO contact)
    {
        _service.AddContact(contact);
        return Ok();
    }

    [HttpPut("{IdContact}")]
    public IActionResult Update(int IdContact, [FromBody] ContactDTO contact)
    {
        ContactDTO contactById = _service.GetContactById(IdContact);
        if (contactById == null) return NotFound();
        _service.UpdateContact(contact);
        return Ok();
    }

    [HttpDelete("{idContact}")]
    public IActionResult Delete(int idContact)
    {
        ContactDTO contact = _service.GetContactById(idContact);
        if (contact == null) return NotFound();
        _service.RemoveContact(idContact);
        return Ok();

    }
}
