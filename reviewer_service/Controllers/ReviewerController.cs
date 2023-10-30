using Microsoft.AspNetCore.Mvc;
using reviewer_service.Exceptions;
using reviewer_service.Models;
using reviewer_service.Requests;
using reviewer_service.Services;
using reviewer_service.Services.Interfaces;

namespace reviewer_service.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewerController : ControllerBase
{
    private static long _counter = 1;
    private static readonly Dictionary<long, Task<TaskResponse>> Tasks = new();
    private readonly IReviewerService _service;

    public ReviewerController(IReviewerService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [Route("/review/add")]
    public IActionResult AddTask(AddTaskRequest request)
    {
        var taskId = _counter++;
        var response = _service.GetAddTask(request.YamlPath, request.CheckPath);
        Tasks.Add(taskId, response);

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
                "Wrong task id");
        if (!Tasks[taskId].IsCompleted)
            return StatusCode(
                StatusCodes.Status202Accepted,
                $"Task {taskId} in progress");
        if (Tasks[taskId].Exception != null && 
            Tasks[taskId].Exception!.InnerException is FileNotFoundException)
            return StatusCode(
                StatusCodes.Status400BadRequest, 
                "Rules file not found");
        if (Tasks[taskId].Exception != null &&
            Tasks[taskId].Exception!.InnerException is ProvidedFileIsNotYamlException)
            return StatusCode(
                StatusCodes.Status400BadRequest, 
                "Provided rules file is not yaml");

        return Ok(Tasks[taskId].Result);
    }
}