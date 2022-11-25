using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using System.Linq;
using System;

public static class SynonymController
{
   
    public static void SetSynonym()
    {
        string[][] synonymGroups = new string[][]
        {
                new string[] { "chair", "stool" },
                new string[] { "desk", "desktop"}
        };
    }

    public static string[] SearchSynonym(string word)
    {
        word = word.Trim().ToLower();
        word = new string(word.Where(Char.IsLetter).ToArray());
        string[] syno = new string[] { word };
        /*string[] synonyms = index.Dictionaries.SynonymDictionary.GetSynonyms(word);
        synonyms = synonyms.Concat(new[] { word }).ToArray();*/
        return syno ;
    }
}
