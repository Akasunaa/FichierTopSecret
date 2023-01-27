using System;
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
        properties.TryAdd("speed", new DicoValueProperty { IsImportant = true, Value = 3f });
        
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
        
        if (TryGet("health", out int health) && health <= 0)
        {
            GameObject ui = GameObject.FindGameObjectWithTag("UI");
            GameOverScreenController gameOverScreenController = ui.GetComponent<GameOverScreenController>();

            if (ui != null && gameOverScreenController != null)
            {
                //we launch the right function :
                gameOverScreenController.OnGameOver(GameOverScreenController.GameOverType.ExplosiveDeath);
            
                return;
            }
        }
    }

    /// <summary>
    /// Function that will recuperates a certain value from the properties, if such value exists
    /// </summary>
    /// <param name="propertyName">name of the property being looked for in the file</param>
    /// <returns>value of said property as a string if found, "DATA NOT FOUND" otherwise</returns>
    public string GetPropertyValue(String propertyName)
    {
        if (properties.ContainsKey(propertyName))
        {
            return properties[propertyName].Value.ToString();
        }
        return "DATA NOT FOUND";
    }
}
