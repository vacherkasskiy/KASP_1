﻿using YamlDotNet.Serialization;

namespace KASP_1_API.Models;

public class Rule
{
    [YamlMember(Alias = "included_paths")]
    public List<string> IncludedPaths { get; set; }
    [YamlMember(Alias = "reviewers")]
    public List<string> Reviewers { get; set; }
}