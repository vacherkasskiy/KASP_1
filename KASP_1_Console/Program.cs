using System.Net;
using System.Text;
using System.Text.Json;
using KASP_1_Console.Models;

static class Program
{
    static string GetFullPath(string relativePath)
    {
        string rootPath = Path.GetFullPath("../../../../");
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
            return // $"path: {checkPath}\n" +
                $"reviewers: {responseContent}";

        return responseContent;
    }

    static async Task PrintGetStatus(string baseUrl, string command)
    {
        var splitCommand = command.Split();

        if (!int.TryParse(splitCommand[1], out int taskId))
        {
            Console.WriteLine("Provided id is not a number");
            return;
        }

        Console.WriteLine(await GetTaskStatus(baseUrl, taskId));
    }

    static async Task PrintAddStatus(string baseUrl, string command)
    {
        var rulesPathRelative = command.Split()[0];
        var checkPath = command.Split()[1];

        try
        {
            var rulesPath = GetFullPath(rulesPathRelative);
            string yamlContent = await File.ReadAllTextAsync(rulesPath);
            Console.WriteLine(await AddTask(baseUrl, yamlContent, checkPath));
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    static async Task Main()
    {
        Console.WriteLine("Type \"EXIT\" to stop the program");
        const string baseUrl = "https://localhost:7107";

        while (true)
        {
            Console.WriteLine("\n---\n");
            var command = Console.ReadLine();
            if (command == null)
            {
                Console.WriteLine("Wrong input");
                continue;
            }

            if (command == "EXIT") return;

            var splitCommand = command.Split();
            if (splitCommand.Length != 2)
            {
                Console.WriteLine("Wrong input");
                continue;
            }

            if (command.Split()[0] == "status") await PrintGetStatus(baseUrl, command);
            else await PrintAddStatus(baseUrl, command);
        }
    }
}