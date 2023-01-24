using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Inherited ModifiableController class of the player
 */
public class PlayerObjectController : ModifiableController
{
    [Header("Player.txt elements")]
    [SerializeField] private string playerName;

    [Header("Audio elements")]
    [SerializeField] AudioClip interactSound;
    [SerializeField] AudioClip createSound;
    [SerializeField] AudioClip deleteSound;
    [SerializeField] AudioSource audioSource;

  

    public override void SetDefaultProperties()
    {
        properties.TryAdd("name", new DicoValueProperty {IsImportant = true, Value = playerName });
        properties.TryAdd("health", new DicoValueProperty {IsImportant = true, Value = 10});
        properties.TryAdd("money", new DicoValueProperty {IsImportant = true, Value = 0});
        if(Application.isEditor) { properties.TryAdd("speed", new DicoValueProperty { IsImportant = true, Value = 10f }); }
        else { properties.TryAdd("speed", new DicoValueProperty { IsImportant = true, Value = 3f }); }
        
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

        if (TryGet("speed", out float speed) && TryGetComponent(out PlayerMovement playerMovement))
        {
            playerMovement.SetSpeed(Mathf.Clamp(speed, 0.1f, 10f));
        }
    }
}
