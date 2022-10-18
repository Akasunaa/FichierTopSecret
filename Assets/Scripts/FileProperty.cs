using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FileProperty
{
    public struct Property
    {
        public string name;
        public string value;
    }
    
    public enum FileChangeType
    {
        New,
        Change,
        Delete,
    }
    
    public struct FileChange
    {
        public FileChangeType type;
        public string path;
        public Dictionary<Property, Property> propertiesModified; // If a Property is null is mean
                                                                  // it did not (new) or will not (delete) exist
    }
}
