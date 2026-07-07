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
public class DeliverablesController : ControllerBase
{
    private readonly AppDbContext _db;

    public DeliverablesController(AppDbContext db)
    {
        _db = db;
    }


    /// <summary>
    /// Renvoie les livrables lié a un projet.
    /// </summary>
    /// <response code="200">envoie des informations sur les </response>
    /// <response code="404">Données invalides, id du projet invalide/inexistant </response>
    [HttpGet("api/projects/{projectId:guid}/deliverables")]
    public async Task<ActionResult<IEnumerable<DeliverableDto>>> GetDeliverables(Guid projectId)
    {
        if (!await _db.Projects.AnyAsync(p => p.Id == projectId))
        {
            return NotFound();
        }

        var deliverables = await _db.Deliverables
            .Where(d => d.ProjectId == projectId)
            .OrderBy(d => d.Name)
            .Select(d => new DeliverableDto
            {
                Id = d.Id,
                Name = d.Name,
                Type = d.Type,
                PathOrUrl = d.PathOrUrl,
                Status = d.Status,
                Comment = d.Comment,
                ProjectId = d.ProjectId
            })
            .ToListAsync();

        return Ok(deliverables);
    }

    /// <summary>
    /// creer un livrable lié a un projet.
    /// </summary>
    /// <param name="projectId">Identifiant du projet.</param>
    /// <param name="dto">Données du livrable à créer.</param>
    /// <response code="201">créé avec succès</response>
    /// <response code="404">Données invalides, id du projet invalide/inexistant </response>
    [HttpPost("api/projects/{projectId:guid}/deliverables")]
    public async Task<ActionResult<DeliverableDto>> CreateDeliverable(Guid projectId, DeliverableCreateDto dto)
    {
        if (!await _db.Projects.AnyAsync(p => p.Id == projectId))
        {
            return NotFound();
        }

        var deliverable = dto.ToEntity(projectId);
        _db.Deliverables.Add(deliverable);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDeliverables), new { projectId }, deliverable.ToDto());
    }

    /// <summary>
    /// Met à jour le statut d'un livrable.
    /// </summary>
    /// <param name="id">Identifiant du livrable.</param>
    /// <param name="dto">Nouveau statut du livrable (et commentaire optionnel).</param>
    /// <response code="204">Statut mis à jour avec succès.</response>
    /// <response code="404">Livrable introuvable.</response>
    [HttpPatch("api/deliverables/{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, DeliverableStatusUpdateDto dto)
    {
        var deliverable = await _db.Deliverables.FindAsync(id);
        if (deliverable is null)
        {
            return NotFound();
        }

        deliverable.Status = dto.Status;
        if (dto.Comment is not null)
        {
            deliverable.Comment = dto.Comment;
        }
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
