using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LRule
{
    public string predecessor;
    public string replacement;
    public float probability = 1f;
}

public static class LSystemGenerator
{
    public static string Generate(string axiom, List<LRule> rules, int iterations)
    {
        string current = axiom;

        for (int i = 0; i < iterations; i++)
            current = Rewrite(current, rules);

        return current;
    }

    private static string Rewrite(string s, List<LRule> rules)
    {
        StringBuilder next = new StringBuilder();
        int i = 0;

        while (i < s.Length)
        {
            bool replaced = false;

            // match rules (longest first)
            foreach (var rule in rules.OrderByDescending(r => r.predecessor.Length))
            {
                // check if we can match rule at position i
                if (i + rule.predecessor.Length <= s.Length)
                {
                    string substring = s.Substring(i, rule.predecessor.Length);

                    if (substring == rule.predecessor)
                    {
                        next.Append(rule.replacement);
                        i += rule.predecessor.Length;
                        replaced = true;
                        break;
                    }
                }
            }

            if (!replaced)
            {
                next.Append(s[i]);
                i++;
            }
        }

        return next.ToString();
    }
}