using System.Collections.Generic;
using System.Linq;

public static class SynonymController
{
    private static readonly string[][] SynonymGroups = {
        new[] { "chair", "stool" },
        new[] { "desk", "desktop"},
        new[] { "lamp", "light", "streetlight"},
        new[] { "door", "exit", "gate"}
    };

    public static IEnumerable<string> SearchSynonym(string word)
    {
        word = word.Trim().ToLower();
        word = new string(word.Where(char.IsLetter).ToArray());
        foreach(var group in SynonymGroups)
        {
            if (group.Any(synWord => synWord == word))
                return group;
            
        }
        var synonym = new[] { word };
        return synonym ;
    }
}
