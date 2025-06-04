using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class InfoRegex
{
    private readonly string[] _types;
    private readonly Regex _regex;

    public InfoRegex(string[] types){
        _types = types ?? throw new ArgumentNullException(nameof(types));
        
        if (types.Length == 0){
            throw new ArgumentException("请输入类型");
        }

        // 构建动态正则表达式
        string typesPattern = string.Join("|", types);
        string regex = $@"^(?<type>{typesPattern}):(?<content>[^(]+)(?:\((?<args>[^)]*)\))?$";
        _regex = new Regex(regex, RegexOptions.Compiled);
    }

    public (string type, string content, string[] args)? Parse(string input){
        if (string.IsNullOrWhiteSpace(input)){
            Debug.LogWarning("输入为空");
            return null;
        }
        var match = _regex.Match(input);
        if (!match.Success){
            Debug.LogError($"输入格式错误. Expected: [{string.Join("|", _types)}]:content(arg1,arg2)");
            return null;
        }

        return (
            match.Groups["type"].Value,
            match.Groups["content"].Value.Trim(),
            match.Groups["args"].Success 
                ? match.Groups["args"].Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToArray()
                : Array.Empty<string>()
        );
    }
}
