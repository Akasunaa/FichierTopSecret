using System.Linq;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject interactionPrompt;
    // var used to remember the state of the prompt between alt-tabs
    private bool _interactionPromptState ; // = false;
    private Grid _grid;

    //Variables used to save the last target and direction to use in defensive procedures to check if object still remains everytime the game comes back in focus
    private Vector3 _lastTarget;
    private Vector2Int _lastDirection;
    private static readonly int PowerExitTrigger = Animator.StringToHash("PowerExitTrigger");
    private static readonly int PowerTrigger = Animator.StringToHash("PowerTrigger");

    public Interactable lastInteractable { get; private set; }

    private void Awake()
    {
        if (!playerMovement) playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        _grid = SceneData.Instance.grid;
    }

    /**
     * Method verifying if there is an object at given position, if it is interactable and start interaction prompt
     */
    public void CheckForInteraction(Vector3 target, Vector2Int direction) //look if player can interact with object or NPC
    {
        //We save the direction and target to allow check for when window comes back in focus
        _lastDirection = direction;
        _lastTarget = target;
        var hitObjects = Utils.CheckPresencesOnTile(_grid, target + (Vector3Int)direction);
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
            var hitObject = hitObjects.First();
            var component =  hitObject.TryGetComponent(out BinRestorationController restorationController) ? restorationController : hitObject.GetComponent(typeof(Interactable));
            
            if (!component) return;
            
            var interactable = component as Interactable;
            //Debug.Log(component.GetType().Name);
            interactionPrompt.SetActive(true);
                
            if (interactable == null) return;
                
            interactable.canBeInteracted = true;
            lastInteractable = interactable;
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
            // if (_lastTarget != null && _lastDirection != null) de comment if you have nulls here (should NEVER occur)
                CheckForInteraction(_lastTarget, _lastDirection);
            
            print("set value at " + _interactionPromptState);
            interactionPrompt.SetActive(_interactionPromptState);
            playerMovement.GetAnimator().SetTrigger(PowerExitTrigger);
        }
        else
        {
            // when losing focus we store the state of the interaction prompt
            _interactionPromptState = interactionPrompt.activeInHierarchy;
            print("store state with value " + _interactionPromptState);

            // deactivate the interaction prompt and start power animation
            interactionPrompt.SetActive(false);
            playerMovement.GetAnimator().SetTrigger(PowerTrigger);
        }
    }

    private void DisplayInteractionPrompt(Vector3 position)
    {
        interactionPrompt.transform.position = position;
        interactionPrompt.SetActive(true);
    }
}