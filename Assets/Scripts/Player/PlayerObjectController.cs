using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Inherited ModifiableController class of the player
 */
public class PlayerObjectController : ModifiableController
{
    [SerializeField] AudioClip interactSound;
    [SerializeField] AudioClip createSound;
    [SerializeField] AudioClip deleteSound;
    [SerializeField] AudioSource audioSource;

  

    public override void SetDefaultProperties()
    {
        properties.TryAdd("name", new DicoValueProperty {IsImportant = true, Value = "Bob"});
        properties.TryAdd("health", new DicoValueProperty {IsImportant = true, Value = 10});
    }

    public void DeleteSound()
    {
        audioSource.PlayOneShot(deleteSound);
    }

    public void CreateSound()
    {
        audioSource.PlayOneShot(createSound);
    }

    public void InteractSound()
    {
        audioSource.PlayOneShot(interactSound);
    }

    public override void UpdateModification(bool firstRead = false)
    {
        base.UpdateModification(firstRead);
        if (properties.ContainsKey("position"))
        {
            properties.Remove("position");
            if (TryGetComponent(out FileParser fp))
            {
                fp.WriteToFile();
            }
        }
    }
}
