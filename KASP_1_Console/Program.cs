using YamlDotNet.Serialization;
using System.Text.RegularExpressions;
using KASP_1_Console.Models;

static bool IsAlign(string path, string pattern)
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
            return (extension == splitPattern[i].Split('.')[^1]);
        }
        if (splitPath[i] != splitPattern[i]) return false;
    }

    return true;
}

var command = Console.ReadLine();

if (command == null || command.Split().Length != 2)
{
    Console.WriteLine("Wrong input");
    return;
}

var deserializer = new DeserializerBuilder().Build();
var rulesPathRelative = command.Split()[0];
var checkPath = command.Split()[1];

string rootPath = Path.GetFullPath("../../../../");
string rulesPath = Path.Combine(rootPath, rulesPathRelative);

if (!File.Exists(rulesPath))
{
    Console.WriteLine($"file '{rulesPath}' not found.");
    return;
}

string yamlContent = await File.ReadAllTextAsync(rulesPath);

var config = deserializer.Deserialize<Config>(yamlContent);
var reviewers = new HashSet<string>();

var tasks = new List<Task>();

foreach (var item in config.Rules)
{
    foreach (var includedPath in item.Value.IncludedPaths)
    {
        var task = Task.Run(() =>
        {
            if (IsAlign(checkPath, includedPath))
            {
                lock (reviewers)
                {
                    item.Value.Reviewers.ForEach(x => reviewers.Add(x));
                }
            }
        });

        tasks.Add(task);
    }
}

await Task.WhenAll(tasks);

var stringReviewers = string.Join(";", reviewers.Distinct());
Console.WriteLine($"path: {checkPath}");
Console.WriteLine($"reviewers: {stringReviewers}");
