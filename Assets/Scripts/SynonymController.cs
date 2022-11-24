using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using GroupDocs.Search.Options;
using GroupDocs.Search.Results;
using Index = GroupDocs.Search.Index;
using System;
using GroupDocs.Search;

public static class SynonymController
{
    static string indexFolder = @".\AdvancedUsage\ManagingDictionaries\SynonymDictionary\Index";
    static Index index = new Index(indexFolder);
    public static void SetSynonym()
    {
        string[][] synonymGroups = new string[][]
        {
                new string[] { "chair", "stool" },
                new string[] { "desk", "desktop"}
        };
        index.Dictionaries.SynonymDictionary.AddRange(synonymGroups);
    }

    public static string[] SearchSynonym(string word)
    {
        string[] synonyms = index.Dictionaries.SynonymDictionary.GetSynonyms(word);
        return synonyms;
    }
}
