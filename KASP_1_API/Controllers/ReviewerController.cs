using KASP_1_API.Requests;
using KASP_1_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace KASP_1_API.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewerController : ControllerBase
{
    private static long _counter = 1;
    private static readonly Dictionary<long, string[]?> Tasks = new ();
    private readonly ReviewerService _service;

    public ReviewerController(ReviewerService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("/review/add")]
    public async Task<IActionResult> AddTask(AddTaskRequest request)
    {
        var taskId = _counter++;
        Tasks.Add(taskId, null);
        var reviewers = await _service.GetReviewers(request.YamlContent, request.CheckPath);
        Tasks[taskId] = reviewers;

        return Ok($"Task created with ID: {taskId}");
    }

    [HttpGet]
    [Route("/review/get")]
    public IActionResult GetTaskStatus(long taskId)
    {
        if (Tasks[taskId] == null)
            return BadRequest("Not ready yet");
        
        return Ok(Tasks[taskId]);
    }
}