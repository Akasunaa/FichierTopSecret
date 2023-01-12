using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

/**
 *  Component handling the door's modifications and state according to its file
 *  Associated file : Door.txt
 *  Values in file :
 *      status : open/closed
 */
public class DoorObjectController : ModifiableController, Interactable
{
    [SerializeField] private string direction;
    [SerializeField] private bool isLockedByDefault;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    //[SerializeField] private Animation transitionAnimation;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openedSprite;

    private ObjectInteractionController _interactionController;
    private bool _displayingDialogue;

    private bool _isOpened;
    public bool canBeInteracted { get; set; }

    private void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);
        //if (!transitionAnimation) transitionAnimation = GetComponent<Animation>();
        //Assert.IsNotNull(transitionAnimation);
        Assert.IsNotNull(closedSprite);
        Assert.IsNotNull(openedSprite);
        _interactionController = GetComponent<ObjectInteractionController>();
        _isOpened = false;
        _displayingDialogue = false;
    }

    public void Interact()
    {
        if (TryGet("locked", out bool locked) && locked && _interactionController!=null && !_displayingDialogue) //UGLY, NEEDS TO BE REWRITTEN
        {
            _displayingDialogue = true;
            //Time.timeScale = 0f;
            _interactionController.DisplayInteractionDialogue();
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().StopMovement();

        }
        else if(/*TryGet("locked", out locked) && locked && */_interactionController != null && _displayingDialogue)
        {
            _displayingDialogue = false;
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().RestartMovement();
            _interactionController.EndDisplay();
            return;
            //Time.timeScale = 1f;
        }
        if (!_displayingDialogue && TryGet("locked", out locked) && !locked)
        {
            if (TryGet("direction", out string dir))
            {
                if (SceneUtility.GetBuildIndexByScenePath(dir) >= 0)
                {
                    print("Door interact");
                    LevelManager.instance.LoadScene(dir);
                }
                else
                {
                    Debug.LogWarning("Scene: " + dir + " does not exists or is not in build");
                }
            }
        } 
    }

    public override void UpdateModification()
    {
        base.UpdateModification();
        //For the door object, we test if its status is opened or closed
        if (properties.ContainsKey("status"))
        {
            print("status is " + properties["status"].Value + " state is " + _isOpened);
            if ((string)properties["status"].Value == "open" && !_isOpened)
            {
                spriteRenderer.sprite = openedSprite;
                _isOpened = true;

                // door is closed but should be opened, open it
                //transitionAnimation["spaceport-door-front"].speed = 1;
                //transitionAnimation.Play();

                //float animDuration = transitionAnimation.GetClip("spaceport-door-front").length;
                //StartCoroutine(ToggleStateAfterAnim(animDuration));
            }
            else if ((string)properties["status"].Value == "closed" && _isOpened)
            {
                spriteRenderer.sprite = closedSprite;
                _isOpened = false;
                // door is opened but should be closed, close it
                //transitionAnimation["spaceport-door-front"].speed = -1;
                //transitionAnimation.Play();

                //float animDuration = transitionAnimation.GetClip("spaceport-door-front").length;
                //StartCoroutine(ToggleStateAfterAnim(animDuration));
            }
        }
    }

    /*private IEnumerator ToggleStateAfterAnim(float duration)
    {
        yield return new WaitForSeconds(duration);

        isOpened = !isOpened;
        if (isOpened)
        {
            spriteRenderer.sprite = openedSprite;
        } else
        {
            spriteRenderer.sprite = closedSprite;
        }
    }*/

    public override void SetDefaultProperties()
    {
        base.SetDefaultProperties();
        properties.Add("locked", new DicoValueProperty {IsImportant = true, Value = isLockedByDefault});
        // properties.Add("status", "closed");
        properties.Add("direction", new DicoValueProperty {IsImportant = true, Value = direction});
    }
}
