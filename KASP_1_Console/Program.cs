using System.Text;
using System.Text.Json;
using KASP_1_Console.Models;

// var command = Console.ReadLine();
// if (command == null || command.Split().Length != 2)
// {
//     Console.WriteLine("Wrong input");
//     return;
// }

var rulesPathRelative = "reviewers.yaml"; // command.Split()[0];
var checkPath = "folder1/readme.md"; // command.Split()[1];

string rootPath = Path.GetFullPath("../../../../");
string rulesPath = Path.Combine(rootPath, rulesPathRelative);

if (!File.Exists(rulesPath))
{
    Console.WriteLine($"file '{rulesPath}' not found.");
    return;
}

string yamlContent = await File.ReadAllTextAsync(rulesPath);
string baseUrl = "https://localhost:7107";

using (var httpClient = new HttpClient())
{
    var requestObject = new AddTaskRequest(yamlContent, checkPath);
    string jsonRequest = JsonSerializer.Serialize(requestObject);
    var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
    var createTaskResponse = await httpClient.PostAsync($"{baseUrl}/review/add", requestContent);

    if (createTaskResponse.IsSuccessStatusCode)
    {
        string responseContent = await createTaskResponse.Content.ReadAsStringAsync();
        Console.WriteLine(responseContent);
    }
    else
    {
        Console.WriteLine($"Failed to create task. Status code: {createTaskResponse.StatusCode}");
    }
    
    long taskId = 1;
    var getTaskResponse = await httpClient.GetAsync($"{baseUrl}/review/get?taskId={taskId}");
    
    if (getTaskResponse.IsSuccessStatusCode)
    {
        string responseContent = await getTaskResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Task status: {responseContent}");
    }
    else
    {
        Console.WriteLine($"Failed to get task status. Status code: {getTaskResponse.StatusCode}");
    }
}
