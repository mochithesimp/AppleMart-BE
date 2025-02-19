using AutoMapper;
using iPhoneBE.Data.Helper;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.CategoryModel;
using iPhoneBE.Data.ViewModels.CategoryDTO;
using iPhoneBE.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace iPhoneBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;
        private readonly IMapper mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(ICategoryServices categoryServices, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _categoryServices = categoryServices;
            this.mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Customer")]
        //[Authorize(Roles = "Admin, Staff, Customer")]
        public async Task<ActionResult<IEnumerable<CategoryViewModel>>> GetAll(string? categoryName = null)
        {
            var categories = await _categoryServices.GetAllAsync(categoryName);
            return Ok(mapper.Map<List<CategoryViewModel>>(categories));
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin, Staff, Customer")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _categoryServices.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(mapper.Map<CategoryViewModel>(category));
        }

        [HttpPost()]
        //[Authorize(Roles = "Admin, Staff")]
        public async Task<ActionResult<Category>> Add([FromBody] CreateCategoryModel Createcategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.BeginTransaction();
            try
            {
                var category = mapper.Map<Category>(Createcategory);
                if (category == null)
                {
                    return BadRequest("Invalid category data.");
                }
                category = await _categoryServices.AddAsync(category);

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(mapper.Map<CategoryViewModel>(category));
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }



        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryModel updateCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.BeginTransaction();
            try
            {

                var category = mapper.Map<Category>(updateCategory);
                bool result = await _categoryServices.UpdateAsync(id, category);
                if (!result)
                {
                    _unitOfWork.RollbackTransaction();
                    return NotFound();
                }
                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(mapper.Map<CategoryViewModel>(category));
            }
            catch (KeyNotFoundException ex)
            {
                _unitOfWork.RollbackTransaction();
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _unitOfWork.RollbackTransaction();
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var existingCategory = await _categoryServices.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                bool result = await _categoryServices.DeleteAsync(id);
                if (!result)
                {
                    _unitOfWork.RollbackTransaction();
                    return NotFound();
                }

                _unitOfWork.SaveChanges();
                _unitOfWork.CommitTransaction();

                return Ok(mapper.Map<CategoryViewModel>(existingCategory));
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
