using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;


namespace BulkyBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _db;

        public CategoryController(IUnitOfWork db)
        {
            _db = db;
        }
        [HttpGet]
        public IEnumerable<Category> GetAll()
        {
            IEnumerable<Category> data = _db.Category.GetAll();
            return data;

        }
        [HttpGet("{Id}")]
        public ActionResult<Category> GetById(int Id)
        {

            var data = _db.Category.GetFirstOrDefault(op => op.Id == Id);
            if (data is null)
                return BadRequest();
            else
                return data;
        }
        [HttpPost]
        public ActionResult PostCategory([FromBody] Category Model)
        {
            _db.Category.Add(Model);
            _db.Save();
            return NoContent();
        }
        [HttpPut("{Id}")]
        public ActionResult PutCategory(int Id, [FromBody] Category Model)
        {
            if (Id != Model.Id)
            {
                return BadRequest();
            }
            _db.Category.Update(Model);
            _db.Save();
            return NoContent();
        }
        [HttpDelete("{Id}")]
        public ActionResult Delete(int Id)
        {
            var category = _db.Category.GetFirstOrDefault(op => op.Id == Id);
            if(category is not null)
            {
                _db.Category.Delete(category);
                _db.Save();
                return NoContent();
            }
            return NotFound();
           
        }
        
    }
}

