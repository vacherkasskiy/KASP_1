using YamlDotNet.Serialization;

namespace KASP_1_1_Console.Models;

public class Config
{
    [YamlMember(Alias = "rules")]
    public Dictionary<string, Rule> Rules { get; set; }
}