using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Simple on/off interrupter implementation.
/// Only work as on/off switches, therefore you cant use them to modify non-boolean properties.
/// </summary>
public class InterrupterObjectController : MonoBehaviour, Interactable
{

    /// <summary>
    /// Pair of values containing the name of a property and a list of ModifiableControllers that may have a value associated with the property.
    /// </summary>
    [Serializable] private struct PropertyAndActivable
    {
        [Tooltip("The name of the property this pair is supposed to manage.")]
        [SerializeField] private string propertyToManage;
        
        [Tooltip("The List of objects you want to modify the value of the associated property.")]
        [SerializeField] private List<ModifiableController> objectsToActivate;

        /// <summary>
        /// The name of the property this pair is supposed to manage.
        /// </summary>
        public string propertyName => propertyToManage;
        
        /// <summary>
        /// The List of objects you want to modify the value of the associated property.
        /// </summary>
        public List<ModifiableController> activableList => objectsToActivate;
    }


    [Tooltip("The list of the properties we need to apply on the associated ModifiableControllers.")]
    [SerializeField] private List<PropertyAndActivable> toActivate;

    public bool canBeInteracted { get; set; }
    public void Interact()
    {

        foreach (var propertyAndActivable in toActivate)
        {
            var propertyName = propertyAndActivable.propertyName;
            if (propertyName == null) continue;
            
            foreach (var modifiableController in propertyAndActivable.activableList.Where(modifiableController => modifiableController))
            {
                if (!modifiableController.TryGet(propertyName, out bool value)) continue;
                
                modifiableController.SetValue(propertyName, !value);
                modifiableController.OnChangeUpdateAll();
            }
        }
    }
}
