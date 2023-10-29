using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Net;
using System.Text;
using System.Text.Json;
using reviewer_util.Models;

namespace reviewer_util;

internal static class Program
{
    private static string GetFullPath(string relativePath)
    {
        string path = Path.Combine(
            Path.GetPathRoot(Environment.SystemDirectory)!,
            relativePath);

        if (!File.Exists(path)) throw new IOException("File does not exists");

        return path;
    }

    private static async Task<string> AddTask(
        string baseUrl,
        string yamlContent,
        string checkPath)
    {
        using var httpClient = new HttpClient();
        var requestObject = new AddTaskRequest(yamlContent, checkPath);
        var jsonRequest = JsonSerializer.Serialize(requestObject);
        var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var createTaskResponse = await httpClient.PostAsync($"{baseUrl}/review/add", requestContent);

        return await createTaskResponse.Content.ReadAsStringAsync();
    }

    private static async Task<string> GetTaskStatus(
        string baseUrl,
        int taskId)
    {
        using var httpClient = new HttpClient();
        var getTaskResponse = await httpClient.GetAsync($"{baseUrl}/review/get?taskId={taskId}");
        var responseContent = await getTaskResponse.Content.ReadAsStringAsync();

        if (getTaskResponse.StatusCode == HttpStatusCode.OK)
        {
            responseContent = await getTaskResponse.Content.ReadAsStringAsync();
            var taskResponse = JsonSerializer.Deserialize<TaskResponse>(responseContent)!;
            var reviewers = "";
            taskResponse.Reviewers!.ToList().ForEach(x => reviewers += $"{x}; ");
            return $"path: {taskResponse.Path}\n" +
                   $"reviewers: {reviewers}";
        }

        return responseContent;
    }

    private static async Task PrintAddStatus(string baseUrl, string yamlPath, string checkPath)
    {
        try
        {
            var rulesPath = GetFullPath(yamlPath);
            var yamlContent = await File.ReadAllTextAsync(rulesPath);
            Console.WriteLine(await AddTask(baseUrl, yamlContent, checkPath));
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static async Task Main(string[] args)
    {
        const string baseUrl = "https://localhost:7107"; // поменять на свой
        var rootCommand = new RootCommand("Review Utility");
        
        var addCommand = new Command("add", "Add a new task");
        addCommand.AddArgument(new Argument<string>("yamlPath", "Yaml file location"));
        addCommand.AddArgument(new Argument<string>("checkPath", "Needed to check file location"));
        addCommand.Handler = CommandHandler.Create<string, string>(async (yamlPath, checkPath) =>
        {
            await PrintAddStatus(baseUrl, yamlPath, checkPath);
        });
        
        var statusCommand = new Command("status", "Get task status");
        statusCommand.AddArgument(new Argument<int>("taskId", "The task ID"));
        statusCommand.Handler = CommandHandler.Create<int>(async taskId =>
        {
            Console.WriteLine(await GetTaskStatus(baseUrl, taskId));
        });

        rootCommand.AddCommand(addCommand);
        rootCommand.AddCommand(statusCommand);
        await rootCommand.InvokeAsync(args);
    }
}