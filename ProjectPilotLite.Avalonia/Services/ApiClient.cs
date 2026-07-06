using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectPilotLite.Core.Dtos;
using ProjectPilotLite.Core.Enums;

namespace ProjectPilotLite.Avalonia.Services;

public class ApiClient : IApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _http;

    public ApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<ProjectSummaryDto>> GetProjectsAsync(CancellationToken ct = default)
        => await GetAsync<List<ProjectSummaryDto>>("api/projects", ct) ?? [];

    public async Task<ProjectDetailDto?> GetProjectAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await Send(() => _http.GetAsync($"api/projects/{id}", ct));
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        await EnsureSuccess(response);
        return await response.Content.ReadFromJsonAsync<ProjectDetailDto>(JsonOptions, ct);
    }

    public Task<ProjectDetailDto> CreateProjectAsync(ProjectCreateDto dto, CancellationToken ct = default)
        => PostAsync<ProjectCreateDto, ProjectDetailDto>("api/projects", dto, ct);

    public Task UpdateProjectStatusAsync(Guid id, ProjectStatus status, CancellationToken ct = default)
        => PatchAsync($"api/projects/{id}/status", new ProjectStatusUpdateDto { Status = status }, ct);

    public async Task<IReadOnlyList<TaskDto>> GetTasksAsync(Guid projectId, CancellationToken ct = default)
        => await GetAsync<List<TaskDto>>($"api/projects/{projectId}/tasks", ct) ?? [];

    public Task<TaskDto> CreateTaskAsync(Guid projectId, TaskCreateDto dto, CancellationToken ct = default)
        => PostAsync<TaskCreateDto, TaskDto>($"api/projects/{projectId}/tasks", dto, ct);

    public Task UpdateTaskStatusAsync(Guid taskId, ProjectTaskStatus status, CancellationToken ct = default)
        => PatchAsync($"api/tasks/{taskId}/status", new TaskStatusUpdateDto { Status = status }, ct);

    public async Task<IReadOnlyList<DeliverableDto>> GetDeliverablesAsync(Guid projectId, CancellationToken ct = default)
        => await GetAsync<List<DeliverableDto>>($"api/projects/{projectId}/deliverables", ct) ?? [];

    public Task<DeliverableDto> CreateDeliverableAsync(Guid projectId, DeliverableCreateDto dto, CancellationToken ct = default)
        => PostAsync<DeliverableCreateDto, DeliverableDto>($"api/projects/{projectId}/deliverables", dto, ct);

    public Task UpdateDeliverableStatusAsync(Guid deliverableId, DeliverableStatus status, string? comment, CancellationToken ct = default)
        => PatchAsync($"api/deliverables/{deliverableId}/status", new DeliverableStatusUpdateDto { Status = status, Comment = comment }, ct);

    public async Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default)
        => await GetAsync<DashboardDto>("api/dashboard", ct) ?? new DashboardDto();

    // ---- helpers ----
    private async Task<T?> GetAsync<T>(string uri, CancellationToken ct)
    {
        using var response = await Send(() => _http.GetAsync(uri, ct));
        await EnsureSuccess(response);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
    }

    private async Task<TResult> PostAsync<TBody, TResult>(string uri, TBody body, CancellationToken ct)
    {
        using var response = await Send(() => _http.PostAsJsonAsync(uri, body, JsonOptions, ct));
        await EnsureSuccess(response);
        return (await response.Content.ReadFromJsonAsync<TResult>(JsonOptions, ct))!;
    }

    private async Task PatchAsync<TBody>(string uri, TBody body, CancellationToken ct)
    {
        using var response = await Send(() => _http.PatchAsJsonAsync(uri, body, JsonOptions, ct));
        await EnsureSuccess(response);
    }

    private static async Task<HttpResponseMessage> Send(Func<Task<HttpResponseMessage>> send)
    {
        try { return await send(); }
        catch (HttpRequestException ex)
        {
            throw new ApiException("Impossible de contacter l'API. Vérifie qu'elle est démarrée et l'URL configurée.", inner: ex);
        }
    }

    private static async Task EnsureSuccess(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;
        throw new ApiException($"Erreur {(int)response.StatusCode} ({response.ReasonPhrase}).", response.StatusCode);
    }
}
