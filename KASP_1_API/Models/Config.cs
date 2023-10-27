using YamlDotNet.Serialization;

namespace KASP_1_API.Models;

public class Config
{
    [YamlMember(Alias = "rules")]
    public Dictionary<string, Rule> Rules { get; set; }
}