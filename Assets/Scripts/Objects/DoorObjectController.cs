using System.Collections;
using System.Collections.Generic;
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

    [Header("Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    //[SerializeField] private Animation transitionAnimation;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openedSprite;

    private bool isOpened;
    public bool canBeInteracted { get; set; }

    private void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);
        //if (!transitionAnimation) transitionAnimation = GetComponent<Animation>();
        //Assert.IsNotNull(transitionAnimation);
        Assert.IsNotNull(closedSprite);
        Assert.IsNotNull(openedSprite);
        isOpened = false;
    }

    public void Interact()
    {
        if (TryGet("locked", out bool locked) && !locked)
        {
            if (TryGet("direction", out string dir))
            {
                if (SceneUtility.GetBuildIndexByScenePath(dir) >= 0)
                {
                    print("Door interact");
                    LevelManager.Instance.LoadScene(dir);
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
            print("status is " + properties["status"] + " state is " + isOpened);
            if (properties["status"] == "open" && !isOpened)
            {
                spriteRenderer.sprite = openedSprite;
                isOpened = true;

                // door is closed but should be opened, open it
                //transitionAnimation["spaceport-door-front"].speed = 1;
                //transitionAnimation.Play();

                //float animDuration = transitionAnimation.GetClip("spaceport-door-front").length;
                //StartCoroutine(ToggleStateAfterAnim(animDuration));
            }
            else if (properties["status"] == "closed" && isOpened)
            {
                spriteRenderer.sprite = closedSprite;
                isOpened = false;
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
        properties.Add("locked", "false");
        properties.Add("status", "closed");
        properties.Add("direction", direction);
    }
}
