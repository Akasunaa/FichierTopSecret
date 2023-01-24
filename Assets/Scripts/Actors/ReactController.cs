using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;
[RequireComponent(typeof(Rigidbody2D))]

public class ReactController : MonoBehaviour
{
    [SerializeField] private Image imagePanel;
    [SerializeField] private TextMeshProUGUI reactionText;
    private bool inReaction = false;
    private UnityAction<(string, object)> listener;

    void Awake()
    {
        listener = ((string, object) propertyChange) =>
        {
            if (!inReaction)
            {
                inReaction = true;
                string[] possibility = {};
                switch (propertyChange.Item1)
                {
                    case "position":
                        possibility = new []{ "Trying to move objects ?" };
                        break;
                    case "color":
                        possibility = new []{ "It looks weird isn't it ?", "Did I put some sunglasses ?" };
                        break;
                    case "money":
                        possibility = new []{ "Looks like someone is greedy" };
                        break;
                    case "speed":
                        possibility = new []{ "Vroum vroum" };
                        break;
                    default:
                        possibility = new []{ "Seems like " + propertyChange.Item1 + " has changed..." };
                        break;
                }

                if (possibility.Length > 0)
                {
                    StartCoroutine(StartReaction(possibility[Random.Range(0, possibility.Length)]));
                }
            }
        };
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out ModifiableController mc))
        {
            mc.fileChange.AddListener(listener);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.TryGetComponent(out ModifiableController mc))
        {
            mc.fileChange.RemoveListener(listener);
        }
    }

    private IEnumerator StartReaction(string text, float fadeDuration = 0.1f)
    {
        inReaction = true;
        while (imagePanel.color.a < 1)
        {
            imagePanel.color = new Color(1, 1, 1, Mathf.Clamp01(imagePanel.color.a + 0.01f / fadeDuration));
            reactionText.color = new Color(1, 1, 1, Mathf.Clamp01(reactionText.color.a + 0.01f / fadeDuration));
            yield return new WaitForSeconds(0.01f);
        }

        reactionText.text = text;
        yield return new WaitForSeconds(3f);
        reactionText.text = "";
        while (imagePanel.color.a > 0)
        {
            imagePanel.color = new Color(1, 1, 1, Mathf.Clamp01(imagePanel.color.a - 0.01f / fadeDuration));
            reactionText.color = new Color(1, 1, 1, Mathf.Clamp01(reactionText.color.a - 0.01f / fadeDuration));
            yield return new WaitForSeconds(0.01f);
        }
        inReaction = false;
    }
}
