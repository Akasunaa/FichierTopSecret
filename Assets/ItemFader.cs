using System.Collections;
using UnityEngine;

public class ItemFader : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform player;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Variables")]
    [SerializeField] private bool startAsFaded;
    [SerializeField] private float range;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float minAlphaValue;
    private bool isFaded;

    private void Awake()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();

        PlayerMovement playerMovement;
        if (player.TryGetComponent(out playerMovement))
        {
            playerMovement.onMovementFinish.AddListener(CheckPositions);
        }
        if (range == 0) range = 10;
        isFaded = false;
    }

    private void Start()
    {
        if (startAsFaded) FadeOut();
    }


    private void CheckPositions()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        // sprite is hidden and player walk in front of it
        if (isFaded && player.position.y < transform.position.y || distance > range)
        {
            // we make the sprite appear
            FadeIn();
        } else if (!isFaded && player.position.y > transform.position.y && distance <= range)
        {
            // we make the sprite disappear
            FadeOut();
        }
    }

    /*
     * Method that make the sprite appear
     */
    private void FadeIn()
    {
        StartCoroutine(FadeCoroutine(1));
        isFaded = false;
    }

    /*
     * Method that make the sprite disappear
     */
    private void FadeOut()
    {
        StartCoroutine(FadeCoroutine(minAlphaValue));
        isFaded = true;
    }

    private IEnumerator FadeCoroutine(float targetAlpha)
    {
        Color initialColor = spriteRenderer.color;
        Color targetColor = initialColor;
        targetColor.a = targetAlpha;
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            Color currentColor = Color.Lerp(initialColor, targetColor, timer / fadeDuration);
            spriteRenderer.color = currentColor;
            yield return null;
        }
    }
}
