using ProjectPilotLite.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectPilotLite.Core.Entities;

public class Project
{
    public int Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Responsable { get; set; } = string.Empty;

    public DateTime DateDebut { get; set; }

    public DateTime DateLimite { get; set; }

    public ProjectStatus Statut { get; set; }

    public List<Task> Taches { get; set; } = new();

    public List<Deliverable> Livrables { get; set; } = new();
}