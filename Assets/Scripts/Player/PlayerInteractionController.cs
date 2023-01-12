using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // var used to remember the state of the prompt between alt-tabs
    bool interactionPromptState = false;
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
        List<GameObject> hitObjects = Utils.CheckPresencesOnTile(grid, target + (Vector3Int)direction);
        hitObjects = hitObjects.FindAll(hitObject =>
            hitObject.TryGetComponent(out BinRestorationController _) ||
            hitObject.GetComponent(typeof(Interactable)) != null);
        if (hitObjects.Count > 1)
        {
            Debug.LogWarning("Multiple possible interaction");
        }

        if (hitObjects.Count > 0)
        {
            // either get interactable component or InteractableTrashController if in Cosmic Bin
            GameObject hitObject = hitObjects.First();
            Component component =  hitObject.TryGetComponent(out BinRestorationController restorationController) ? restorationController : hitObject.GetComponent(typeof(Interactable));
            if (component)
            {
                Interactable interactable = component as Interactable;
                //Debug.Log(component.GetType().Name);
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
            print("set value at " + interactionPromptState);
            interactionPrompt.SetActive(interactionPromptState);
            playerMovement.GetAnimator().SetTrigger("PowerExitTrigger");
        }
        else
        {
            // when losing focus we store the state of the interaction prompt
            interactionPromptState = interactionPrompt.activeInHierarchy;
            print("store state with value " + interactionPromptState);

            // deactivate the interaction prompt and start power animation
            interactionPrompt.SetActive(false);
            playerMovement.GetAnimator().SetTrigger("PowerTrigger");
        }
    }

    private void DisplayInteractionPrompt(Vector3 position)
    {
        interactionPrompt.transform.position = position;
        interactionPrompt.SetActive(true);
    }
}