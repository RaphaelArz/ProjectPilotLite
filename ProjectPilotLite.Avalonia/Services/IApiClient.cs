using ProjectPilotLite.Core.Dtos;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Avalonia.Services;

public interface IApiClient
{
    // Projets
    Task<IReadOnlyList<ProjectSummaryDto>> GetProjectsAsync(CancellationToken ct = default);
    Task<ProjectDetailDto?> GetProjectAsync(Guid id, CancellationToken ct = default);
    Task<ProjectDetailDto> CreateProjectAsync(ProjectCreateDto dto, CancellationToken ct = default);
    Task UpdateProjectStatusAsync(Guid id, ProjectStatus status, CancellationToken ct = default);

    // Tâches
    Task<IReadOnlyList<TaskDto>> GetTasksAsync(Guid projectId, CancellationToken ct = default);
    Task<TaskDto> CreateTaskAsync(Guid projectId, TaskCreateDto dto, CancellationToken ct = default);
    Task UpdateTaskStatusAsync(Guid taskId, ProjectTaskStatus status, CancellationToken ct = default);

    // Livrables
    Task<IReadOnlyList<DeliverableDto>> GetDeliverablesAsync(Guid projectId, CancellationToken ct = default);
    Task<DeliverableDto> CreateDeliverableAsync(Guid projectId, DeliverableCreateDto dto, CancellationToken ct = default);
    Task UpdateDeliverableStatusAsync(Guid deliverableId, DeliverableStatus status, string? comment, CancellationToken ct = default);

    // Tableau de bord
    Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default);
}
