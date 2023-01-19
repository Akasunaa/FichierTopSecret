using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

/// <summary>
/// Component used to create scriptable objects that will contain the portraits
/// </summary>
[CreateAssetMenu(fileName = "Portrait", menuName = "Portraits")]
public class PortraitObject : ScriptableObject
{
    public string portraitName;
    public Sprite portraitSprite;

    PortraitObject(string portraitName, Sprite portraitSprite)
    {
        this.portraitName = portraitName;
        this.portraitSprite = portraitSprite;
    }
}
