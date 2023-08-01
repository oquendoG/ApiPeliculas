﻿using ApiPeliculas.Data.Repository.Interfaces;
using ApiPeliculas.Feats.Categories.DTOs;
using ApiPeliculas.Shared;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Feats.Categories.Controllers;

[ApiController]
[Route("api/Categorias")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategories()
    {
        ICollection<Category> categoriesDb =
            await categoryRepository.GetCategories();
        if (categoriesDb.Count == 0)
        {
            return NotFound();
        }

        List<CategoryDto> categories =
            categoriesDb.Adapt<List<CategoryDto>>();

        return Ok(categories);
    }

    [HttpGet("{id:Guid}", Name = "categoria")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        Category? categoryDb =
            await categoryRepository.GetCategoryById(id);
        if (categoryDb is null)
        {
            return NotFound(id);
        }

        CategoryDto category =
            categoryDb.Adapt<CategoryDto>();

        return Ok(category);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto category)
    {
        if (!ModelState.IsValid || category is null)
        {
            ModelState.AddModelError(string.Empty, "Revise que el nombre de la categoría");
            return BadRequest(category);
        }

        if (categoryRepository.CategoryExists(category.Name))
        {
            ModelState.AddModelError(string.Empty, "La categoría ya existe");
            return StatusCode(404, ModelState);
        }

        Category categoryToBd = category.Adapt<Category>();
        bool result = await categoryRepository.CreateCategory(categoryToBd);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Ha habido un error y no se pudo guardar el registro por favor consulte con el adminsitrador del sistema");
            return StatusCode(500, ModelState);
        }

        return CreatedAtRoute("categoria", new { id = categoryToBd.Id }, categoryToBd);
    }

    [HttpPatch("{id:Guid}", Name = "actualizar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchCategory(Guid id, [FromBody] CategoryDto category)
    {
        if (!ModelState.IsValid || category is null || id != category.Id)
        {
            return BadRequest(ModelState);
        }

        Category categoryToBd = category.Adapt<Category>();
        bool result = await categoryRepository.UpdateCategory(categoryToBd);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Ha habido un error y no se pudo actualizar el registro por favor consulte con el adminsitrador del sistema");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    [HttpDelete("{id:Guid}", Name = "borrar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        if (!categoryRepository.CategoryExists(id))
        {
            return NotFound(id);
        }

        Category? categoryToDelete = await categoryRepository.GetCategoryById(id);
        bool result = false;
        if (categoryToDelete is not null)
        {
            result = await categoryRepository.DeleteCategory(categoryToDelete);
        }

        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Ha habido un error y no se pudo eliminar el registro por favor consulte con el adminsitrador del sistema");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
}
