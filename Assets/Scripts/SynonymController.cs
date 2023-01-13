using System.Collections.Generic;
using System.Linq;

public static class SynonymController
{
    private static readonly string[][] SynonymGroups = {
        new[] { "chair", "stool" },
        new[] { "desk", "desktop"},
        new[] { "lamp", "light", "streetlight"},
        new[] { "door", "exit", "gate"},
        new[] { "teleport", "teleporter", "portal"}
    };

    public static IEnumerable<string> SearchSynonym(string word)
    {
        word = word.Trim().ToLower();
        word = new string(word.Where(char.IsLetter).ToArray());
        var synonym = new[] { word };
        foreach(var group in SynonymGroups)
        {
            if (group.Any(synWord => synWord == word))
                synonym = group;
            
        }
        return synonym ;
    }
}
