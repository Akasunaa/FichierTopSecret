using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Component used to manage the state of neon bars in the game. 
/// </summary>
public class NeonObjectController : ModifiableController
{
    [Tooltip("The sprite of the neon game object.")]
    [SerializeField] private GameObject sprite;
    
    [Tooltip("The list of the lights present on the neon game object.")]
    [SerializeField] private List<GameObject> lightList;

    /// <summary>
    /// The grey level to put in the off color when the neon bars are off.
    /// </summary>
    private const float OffGreyLevel = 240f / 255f;
    
    /// <summary>
    /// The color that the sprite takes when the neon bar is on.
    /// </summary>
    private readonly Color _onColor = Color.white;
    
    /// <summary>
    /// The color that the sprite takes when the neon bar in off.
    /// </summary>
    private readonly Color _offColor = new(OffGreyLevel, OffGreyLevel, OffGreyLevel, 1);

    public override void SetDefaultProperties()
    {
        // base.SetDefaultProperties();

        var hasAllElements = true;
        
        var lightOne = lightList[0];
        if (!lightOne)
        {
            SendWarning(WarningType.NoLight);
            hasAllElements = false;
        }

        var light2D = lightOne.GetComponent<Light2D>();
        if (hasAllElements && !light2D)
        {
            SendWarning(WarningType.NoLight2D, lightOne.name);
            hasAllElements = false;
        }
        
        properties.TryAdd("power", new DicoValueProperty { IsImportant = true, Value = hasAllElements && light2D.isActiveAndEnabled });
    }

    public override void UpdateModification()
    {
        base.UpdateModification();
        UpdateSpriteModification();
        UpdateLightsModification();
    }

    /// <summary>
    /// Updates the lights in the lightsList member according to the object's properties' states.
    /// In particular, modifies their Light2D component's intensity.
    /// </summary>
    private void UpdateLightsModification()
    {
        if (!TryGet("power", out bool power)) return;
        
        foreach (var localLight in lightList)
        {
            var light2D = localLight.GetComponent<Light2D>();
            if (!light2D)
            {
                SendWarning(WarningType.NoLight2D, localLight.name);
                continue;
            }

            light2D.intensity = power ? 1f : 0f;
        }
    }

    /// <summary>
    /// Update the sprite member according to the object's properties' states.
    /// In particular, changes its SpriteControllers' color parameter.
    /// </summary>
    private void UpdateSpriteModification()
    {
        var spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            SendWarning(WarningType.NoSpriteRenderer);
            return;
        }
        
        if (!TryGet("power", out bool power)) return;
        spriteRenderer.color = power ? _onColor : _offColor;
    }

    /// <summary>
    /// Calls Debug.LogWarn and adds a message in function of the warning parameter.
    /// </summary>
    /// <param name="warning"> The WarningType you want to warn the dev about. </param>
    /// <param name="sender"> The name of the sender of the warning, useful when the reason is caused by another GO. </param>
    private void SendWarning(WarningType warning, string sender = "")
    {
        var message = warning switch
        {
            WarningType.NoLight => "Has no light GameObject in its list and therefore cant work.",
            WarningType.NoLight2D => sender + " has no Light2D component and therefore cannot work.",
            WarningType.NoSpriteRenderer => "Has no sprite renderer.",
            _ => "Warning message not recognised."
        };

        Debug.LogWarning( "[" + gameObject.name + "] " + message);
    }

    private enum WarningType
    {
        NoLight,
        NoLight2D,
        NoSpriteRenderer
    }
}
