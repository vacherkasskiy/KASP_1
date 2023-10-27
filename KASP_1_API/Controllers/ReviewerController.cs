using KASP_1_API.Requests;
using KASP_1_API.Responses;
using KASP_1_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace KASP_1_API.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewerController : ControllerBase
{
    private static long _counter = 1;
    private static readonly Dictionary<long, Task<GetTaskStatusResponse>> Tasks = new ();
    private readonly ReviewerService _service;

    public ReviewerController(ReviewerService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [Route("/review/add")]
    public IActionResult AddTask(AddTaskRequest request)
    {
        var taskId = _counter++;
        var reviewers = _service.GetReviewers(request.YamlContent, request.CheckPath);
        var task = Task.FromResult(new GetTaskStatusResponse(request.CheckPath, reviewers.Result));
        Tasks.Add(taskId, task);

        return Ok($"Task created with ID: {taskId}");
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(202)]
    [ProducesResponseType(400)]
    [Route("/review/get")]
    public IActionResult GetTaskStatus(long taskId)
    {
        if (!Tasks.ContainsKey(taskId))
            return StatusCode(
                StatusCodes.Status400BadRequest,
                $"Wrong task id");
        if (!Tasks[taskId].IsCompleted)
            return StatusCode(
                StatusCodes.Status202Accepted,
                $"Task {taskId} in progress");

        return Ok(Tasks[taskId].Result);
    }
}