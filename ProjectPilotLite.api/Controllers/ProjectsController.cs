using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPilotLite.Api.Data;
using ProjectPilotLite.Api.Mapping;
using ProjectPilotLite.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectPilotLite.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Récupère la liste de tous les projets.
    /// </summary>
    /// <response code="200">Liste des projets récupérée avec succès.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectSummaryDto>>> GetProjects()
    {
        var projects = await _db.Projects
            .OrderBy(p => p.Name)
            .Select(p => new ProjectSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                Owner = p.Owner,
                Status = p.Status,
                StartDate = p.StartDate,
                Deadline = p.Deadline,
                TaskCount = p.Tasks.Count,
                DeliverableCount = p.Deliverables.Count
            })
            .ToListAsync();

        return Ok(projects);
    }


    /// <summary>
    /// Récupère les détails d'un projet par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant du projet.</param>
    /// <response code="200">Projet trouvé.</response>
    /// <response code="404">Projet introuvable.</response>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDetailDto>> GetProject(Guid id)
    {
        var project = await _db.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Deliverables)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(project.ToDetailDto());
    }

    /// <summary>
    /// Crée un nouveau projet.
    /// </summary>
    /// <param name="dto">Données du projet à créer.</param>
    /// <response code="201">Projet créé avec succès.</response>
    /// <response code="400">Données invalides.</response>
    [HttpPost]
    public async Task<ActionResult<ProjectDetailDto>> CreateProject(ProjectCreateDto dto)
    {
        var project = dto.ToEntity();
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project.ToDetailDto());
    }

    /// <summary>
    /// Met à jour le statut d'un projet.
    /// </summary>
    /// <param name="id">Identifiant du projet.</param>
    /// <param name="dto">Nouveau statut du projet.</param>
    /// <response code="204">Statut mis à jour avec succès.</response>
    /// <response code="404">Projet introuvable.</response>
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, ProjectStatusUpdateDto dto)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project is null)
        {
            return NotFound();
        }

        project.Status = dto.Status;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
