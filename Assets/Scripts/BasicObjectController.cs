using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObjectController : ModifiableController
{
    public override void UpdateModification()
    {
        if (properties.ContainsKey("color"))
        { 
            if (ColorUtility.TryParseHtmlString(properties["color"], out Color color))
            {
                if (TryGetComponent(out SpriteRenderer spriteRenderer))
                {
                    spriteRenderer.color = color;
                }
            }
        }
    }
}
