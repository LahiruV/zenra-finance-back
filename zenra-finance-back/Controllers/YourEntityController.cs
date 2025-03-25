using Microsoft.AspNetCore.Mvc;
using zenra_finance_back.Models;
using zenra_finance_back.Services.IServices;

namespace zenra_finance_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YourEntityController : ControllerBase
    {
        private readonly IYourEntityService _service;

        public YourEntityController(IYourEntityService service)
        {
            _service = service;
        }

        // GET: api/YourEntity
        [HttpGet]
        public ActionResult<IEnumerable<YourEntity>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        // GET: api/YourEntity/5
        [HttpGet("{id}")]
        public ActionResult<YourEntity> GetById(int id)
        {
            var entity = _service.GetById(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        // POST: api/YourEntity
        [HttpPost]
        public IActionResult Create(YourEntity entity)
        {
            _service.Create(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        // PUT: api/YourEntity/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, YourEntity entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            _service.Update(entity);
            return NoContent();
        }

        // DELETE: api/YourEntity/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}
