using KASP_1_API.Requests;
using KASP_1_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace KASP_1_API.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewerController : ControllerBase
{
    private long _counter = 1;
    private readonly Dictionary<long, string[]?> _tasks;
    private readonly ReviewerService _service;

    public ReviewerController(ReviewerService service)
    {
        _tasks = new Dictionary<long, string[]?>();
        _service = service;
    }

    [HttpPost]
    [Route("/review/add")]
    public async Task<IActionResult> AddTask(AddTaskRequest request)
    {
        var taskId = _counter++;
        _tasks.Add(taskId, null);
        var reviewers = await _service.GetReviewers(request.YamlContent, request.CheckPath);
        _tasks[taskId] = reviewers;

        return Ok($"Task created with ID: {taskId}");
    }

    [HttpGet]
    [Route("/review/get")]
    public IActionResult GetTaskStatus(long taskId)
    {
        if (_tasks[taskId] == null)
            return BadRequest("Not ready yet");
        
        return Ok(_tasks[taskId]);
    }
}