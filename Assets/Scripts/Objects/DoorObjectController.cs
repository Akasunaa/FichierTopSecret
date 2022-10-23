using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Component handling the door's modifications and state according to its file
 *  Associated file : Door.txt
 *  Values in file :
 *      status : open/closed
 */
public class DoorObjectController : ModifiableController
{
    [TextArea] private string direction;

    public override void UpdateModification()
    {
        //For the lamp object, we test if its power is on or off
        if (properties.ContainsKey("status"))
        {
            if (properties["status"] == "open")
            {
                print("DOOR OPEN");
                //CODE TO OPEN DOOR
            }
            else if (properties["status"] == "closed")
            {
                print("DOOR CLOSE");
                //CODE TO CLOSE DOOR
            }
        }
    }

    public override void setDefaultProperties()
    {
        properties.Add("locked", "yes");
        properties.Add("direction", direction);
    }
}
