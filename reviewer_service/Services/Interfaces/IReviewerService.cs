using reviewer_service.Models;

namespace reviewer_service.Services.Interfaces;

public interface IReviewerService
{
    public Task<TaskResponse> GetAddTask(string yamlPath, string checkPath);
}