using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using System.Linq;
using System;

public static class SynonymController
{
    static string[][] synonymGroups = new string[][]
    {
        new string[] { "chair", "stool" },
        new string[] { "desk", "desktop"},
        new string[] { "lamp", "light", "streetlight"},
        new string[] { "door", "exit","gate"},
        new string[] { "teleport", "teleporter","portal"}
    };

    public static string[] SearchSynonym(string word)
    {
        word = word.Trim().ToLower();
        word = new string(word.Where(Char.IsLetter).ToArray());
        foreach(var group in synonymGroups)
        {
            foreach(var synword in group)
            {
                if(synword == word)
                {
                    return group;
                }
            }
        }
        string[] syno = new string[] { word };
        return syno ;
    }
}
