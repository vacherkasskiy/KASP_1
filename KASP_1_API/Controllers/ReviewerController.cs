using KASP_1_API.Requests;
using KASP_1_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace KASP_1_API.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewerController : ControllerBase
{
    private long _counter = 1;
    private readonly Dictionary<long, Task<string[]>> _tasks;
    private readonly ReviewerService _service;

    public ReviewerController(ReviewerService service)
    {
        _tasks = new Dictionary<long, Task<string[]>>();
        _service = service;
    }

    [HttpPost]
    [Route("/review/add")]
    public async Task<IActionResult> AddTask(AddTaskRequest request)
    {
        var taskId = _counter++;
        var task = _service.GetReviewers(request.YamlContent, request.CheckPath);
        _tasks.Add(taskId, task);

        return Ok($"Task created with ID: {taskId}");
    }

    [HttpGet]
    [Route("/review/get")]
    public IActionResult GetTaskStatus(long taskId)
    {
        var status = _tasks[taskId].Status;
        
        return Ok(status);
    }
}