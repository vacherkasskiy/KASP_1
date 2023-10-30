using System.Text.RegularExpressions;
using reviewer_service.Models;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace reviewer_service.Services;

public class ReviewerService
{
    private readonly IDeserializer _deserializer;

    public ReviewerService()
    {
        _deserializer = new DeserializerBuilder().Build();
    }
    
    private async Task<Config> GetYamlObject(string yamlRelativePath)
    {
        string yamlPath = Path.Combine(
            Path.GetPathRoot(Environment.SystemDirectory)!,
            yamlRelativePath);

        if (!File.Exists(yamlPath)) 
            throw new IOException("File does not exists");
        
        var yamlContent = await File.ReadAllTextAsync(yamlPath);
        
        try
        {
            return _deserializer.Deserialize<Config>(yamlContent);
        }
        catch (YamlException _)
        {
            throw new YamlException("Provided file is not yaml");
        }
    }
    
    private static bool IsAlign(string path, string pattern)
    {
        var splitPattern = pattern.Split('/');
        var splitPath = path.Split('/');
        var n = splitPath.Length;
        var regex = @"\*\.[a-zA-Z]+";

        if (splitPath.Length != splitPattern.Length) return false;

        for (var i = 0; i < n; ++i)
        {
            if (splitPattern[i] == "*") continue;
            if (Regex.IsMatch(splitPattern[i], regex))
            {
                if (i != n - 1 || !splitPath[i].Contains('.')) return false;
                var extension = splitPath[i].Split('.')[^1];
                return extension == splitPattern[i].Split('.')[^1];
            }

            if (splitPath[i] != splitPattern[i]) return false;
        }

        return true;
    }

    public async Task<TaskResponse> GetAddTask(string yamlPath, string checkPath)
    {
        Config config;

        try
        {
            config = await GetYamlObject(yamlPath);
        }
        catch (IOException e)
        {
            throw new ArgumentException(e.Message);
        }
        catch (YamlException e)
        {
            throw new ArgumentException(e.Message);
        }

        var reviewers = new HashSet<string>();
        var tasks = new List<Task>();

        foreach (var item in config.Rules)
        foreach (var includedPath in item.Value.IncludedPaths)
        {
            var task = Task.Run(() =>
            {
                if (IsAlign(checkPath, includedPath))
                    lock (reviewers)
                    {
                        item.Value.Reviewers.ForEach(x => reviewers.Add(x));
                    }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        return new TaskResponse(checkPath, reviewers.ToArray());
    }
}