using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Net;
using System.Text;
using System.Text.Json;
using KASP_1_Console.Models;

namespace KASP_1_Console;

static class Program
{
    static string GetFullPath(string relativePath)
    {
        string rootPath = Path.GetFullPath("../");
        string fullPath = Path.Combine(rootPath, relativePath);

        if (!File.Exists(fullPath)) throw new IOException("File does not exists");

        return fullPath;
    }

    static async Task<string> AddTask(
        string baseUrl,
        string yamlContent,
        string checkPath)
    {
        using var httpClient = new HttpClient();
        var requestObject = new AddTaskRequest(yamlContent, checkPath);
        string jsonRequest = JsonSerializer.Serialize(requestObject);
        var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var createTaskResponse = await httpClient.PostAsync($"{baseUrl}/review/add", requestContent);

        return await createTaskResponse.Content.ReadAsStringAsync();
    }

    static async Task<string> GetTaskStatus(
        string baseUrl,
        int taskId)
    {
        using var httpClient = new HttpClient();
        var getTaskResponse = await httpClient.GetAsync($"{baseUrl}/review/get?taskId={taskId}");
        string responseContent = await getTaskResponse.Content.ReadAsStringAsync();

        if (getTaskResponse.StatusCode == HttpStatusCode.OK)
        {
            responseContent = await getTaskResponse.Content.ReadAsStringAsync();
            TaskResponse taskResponse = JsonSerializer.Deserialize<TaskResponse>(responseContent)!;
            string reviewers = "";
            taskResponse.Reviewers!.ToList().ForEach(x => reviewers += $"{x}; ");
            return $"path: {taskResponse.Path}\n" +
                   $"reviewers: {reviewers}";
        }

        return responseContent;
    }

    static async Task PrintAddStatus(string baseUrl, string yamlPath, string checkPath)
    {
        try
        {
            var rulesPath = GetFullPath(yamlPath);
            string yamlContent = await File.ReadAllTextAsync(rulesPath);
            Console.WriteLine(await AddTask(baseUrl, yamlContent, checkPath));
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    static async Task Main(string[] args)
    {
        const string baseUrl = "https://localhost:7107"; // поменять на свой
        var rootCommand = new RootCommand("Review Utility");
        
        // Add task code
        var addCommand = new Command("add", "Add a new task");
        addCommand.AddArgument(new Argument<string>("yamlPath", "Yaml file location"));
        addCommand.AddArgument(new Argument<string>("checkPath", "Needed to check file location"));
        
        addCommand.Handler = CommandHandler.Create<string, string>(async (yamlPath, checkPath) =>
        {
            await PrintAddStatus(baseUrl, yamlPath, checkPath);
        });
        
        // Get task status code
        var statusCommand = new Command("status", "Get task status");
        statusCommand.AddArgument(new Argument<int>("taskId", "The task ID"));

        statusCommand.Handler = CommandHandler.Create<int>(async (taskId) =>
        {
            Console.WriteLine(await GetTaskStatus(baseUrl, taskId));
        });
        
        rootCommand.AddCommand(addCommand);
        rootCommand.AddCommand(statusCommand);
        await rootCommand.InvokeAsync(args);
    }
}