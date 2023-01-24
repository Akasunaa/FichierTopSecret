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
    [SerializeField] protected string direction;
    [SerializeField] protected bool isLockedByDefault;

    [Header("Door Special Interactions")]
    [TextArea(3,10)]
    [SerializeField] protected string noDirectionDialogue;

    [Header("Animation")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    //[SerializeField] private Animation transitionAnimation;
    [SerializeField] protected Sprite closedSprite;
    [SerializeField] protected Sprite openedSprite;

    protected ObjectInteractionController _interactionController;
    protected bool _displayingDialogue;

    protected bool _isOpened;
    public bool canBeInteracted { get; set; }


    protected virtual void Awake()
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
        if(noDirectionDialogue==null || noDirectionDialogue == "")
        {
            noDirectionDialogue = "This door doesn't lead to anywhere... Like, a unending void of darkness is behind its frame...";
        }
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
        if (!_displayingDialogue && TryGet("locked", out locked) && !locked && TryGet("direction", out string dir))
        {
            if (SceneUtility.GetBuildIndexByScenePath(dir) >= 0)
            {   
                if(PositionInSceneController.hasInstance) { PositionInSceneController.instance.OnPlayerExitedLevel(); }

                ChangeSceneAnalyserController.Instance.SetLoadingState(true); //we indicate to the system message analyser that we changed scene through a door
                LevelManager.Instance.LoadScene(dir);
            }
            else
            {
                _displayingDialogue = true;
                _interactionController.DisplayDialogue(noDirectionDialogue);
                GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().StopMovement();
                Debug.LogWarning("Scene: " + dir + " does not exists or is not in build");
            }
        } 
    }

    public override void UpdateModification(bool firstRead = false)
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
        if (Application.isEditor) //by default, if we're in the editor, all the doors are unlocked
        {
            properties.TryAdd("locked", new DicoValueProperty { IsImportant = true, Value = false });
        }
        else
        {
            properties.TryAdd("locked", new DicoValueProperty { IsImportant = true, Value = isLockedByDefault });
        }
        // properties.Add("status", "closed");
        properties.TryAdd("direction", new DicoValueProperty {IsImportant = true, Value = direction});
    }

}
