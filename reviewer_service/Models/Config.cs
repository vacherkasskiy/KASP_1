using YamlDotNet.Serialization;

namespace reviewer_service.Models;

public class Config
{
    [YamlMember(Alias = "rules")] public Dictionary<string, Rule> Rules { get; set; }
}