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


    Vector3 debugValue;                                            
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
     * Method verifying if there is an obstacle at given position
     */
    public bool IsColliding(Vector3Int target) //look if player can move to the target 
    {
        // using GetCellCenterWorld is very important to avoid locking on to the corner of the tile
        Collider2D hit = Physics2D.OverlapBox(grid.GetCellCenterWorld(target), grid.cellSize-Vector3.one * 0.1f,0);
        debugValue = grid.GetCellCenterWorld(target);
        if (hit) { return true;}
        return false;
    }


    /**
     * Method verifying if there is an object at given position, if it is interactable and start interaction prompt
     */
    public void IsInteracting(Vector3 target, Vector2Int direction) //look if player can interact with object or NPC
    {
        //We save the direction and target to allow check for when window comes back in focus
        lastDirection = direction;
        lastTarget = target;

        Collider2D hit = Physics2D.OverlapBox(target+(Vector3Int) direction, grid.cellSize - Vector3.one * 0.1f, 0);
        debugValue = target + new Vector3(direction.x, 0, direction.y);
        if (hit)
        {
            Component component = hit.gameObject.GetComponent(typeof(Interactable));
            if (component)
            {
                Interactable interactable = component as Interactable;
                interactionPrompt.SetActive(true);
                interactable.canBeInteracted = true;
                lastInteractable = interactable;
            }
        }
        else { 
            interactionPrompt.SetActive(false); 
            if (lastInteractable !=  null) { 
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
        if(lastTarget!=null && lastDirection != null)
        {
            IsInteracting(lastTarget, lastDirection);
        }
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(debugValue , grid.cellSize - Vector3.one * 0.1f);
    }*/

    private void DisplayInteractionPrompt(Vector3 position)
    {
        interactionPrompt.transform.position = position;
        interactionPrompt.SetActive(true);
    }
}
