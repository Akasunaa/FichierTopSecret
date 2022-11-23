using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Inherited ModifiableController class of the player
 */
public class PlayerObjectController : ModifiableController
{
    public override void SetDefaultProperties()
    {
        properties.Add("name", "Bob");
        properties.Add("health", "10");
    }
}
