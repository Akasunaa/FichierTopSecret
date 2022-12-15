using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;
using static UnityEngine.UI.Image;
using UnityEngine.Rendering.Universal;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject interactionPrompt;
    Grid grid;

    //Variables used to save the last target and direction to use in defensive procedures to check if object still remains everytime the game comes back in focus
    private Vector3 lastTarget;
    private Vector2Int lastDirection;

    public Interactable lastInteractable { get; private set; }

    private void Awake()
    {
        if (!playerMovement) playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        grid = SceneData.Instance.grid;
    }

    /**
     * Method verifying if there is an object at given position, if it is interactable and start interaction prompt
     */
    public void CheckForInteraction(Vector3 target, Vector2Int direction) //look if player can interact with object or NPC
    {
        //We save the direction and target to allow check for when window comes back in focus
        lastDirection = direction;
        lastTarget = target;
        GameObject hitObject = Utils.CheckPresenceOnTile(grid, target + (Vector3Int)direction);

        if (hitObject)
        {
            // either get interactable component or InteractableTrashController if in Cosmic Bin
            Component component =  hitObject.TryGetComponent(out BinRestorationController restorationController) ? restorationController : hitObject.GetComponent(typeof(Interactable));
            if (component)
            {
                Interactable interactable = component as Interactable;
                Debug.Log(component.GetType().Name);
                interactionPrompt.SetActive(true);
                interactable.canBeInteracted = true;
                lastInteractable = interactable;
            }
        }
        else
        {
            interactionPrompt.SetActive(false);
            if (lastInteractable != null)
            {
                lastInteractable.canBeInteracted = false;
            }
        }
    }

    /**
     *  Function called everytime the game comes in focus
     *  Tests if the last detected object can be detected again, i.e. has not moved nor been deleted
     */
    private void OnApplicationFocus(bool focus)
    {
        //maybe can be used to check for new object near to player 
        if (focus)
        {
            if (lastTarget != null && lastDirection != null)
            {
                CheckForInteraction(lastTarget, lastDirection);
            }
        }
    }

    private void DisplayInteractionPrompt(Vector3 position)
    {
        interactionPrompt.transform.position = position;
        interactionPrompt.SetActive(true);
    }
}