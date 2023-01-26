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
    private UnityAction<(string, object, object)> listener; // (string, object, object) : (keyName, newValue, oldValue)

    void Awake()
    {
        listener = ((string, object, object) propertyChange) =>
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
                        if (ModifiableController.TryParse(propertyChange.Item2, out Color newColor))
                        {
                            Color purpleColor = ColorUtility.TryParseHtmlString("purple", out Color purpleColorTmp) ? purpleColorTmp : purpleColorTmp;
                            Color orangeColor = ColorUtility.TryParseHtmlString("orange", out Color orangeColorTmp) ? orangeColorTmp : orangeColorTmp;
                            Color yellowColor = ColorUtility.TryParseHtmlString("yellow", out Color yellowColorTmp) ? yellowColorTmp : yellowColorTmp;
                            Color greenColor = ColorUtility.TryParseHtmlString("green", out Color greenColorTmp) ? greenColorTmp : greenColorTmp;
                            if (newColor == Color.black)
                            {
                                possibility = new[] { "Edgy too ?", "Kohuro would love that" };
                            }
                            else if (newColor == Color.red || newColor == yellowColor || newColor == greenColor || newColor == Color.blue)
                            {
                                possibility = new[] { "Do you know about the DiSC personality test ?" };
                            }
                            else if (newColor == purpleColor)
                            {
                                possibility = new[] { "Raphiki would love that", "BiscuitPrime would love that" };
                            }
                            else if (newColor == orangeColor)
                            {
                                possibility = new[] { "Akasuna would love that" };
                            }
                            else if (newColor == Color.cyan)
                            {
                                possibility = new[] { "PtiBouchon would love that" };
                            }
                            else
                            {
                                possibility = new[] { "WoW, I love the new style" };
                            }
                        }
                        else
                        {
                            possibility = new[] { "It looks weird isn't it ?", "Did I put some sunglasses ?" };
                        }

                        break;
                    case "money":
                        if (ModifiableController.TryParse(propertyChange.Item2, out int newMoney) &&
                            ModifiableController.TryParse(propertyChange.Item3, out int oldMoney))
                        {
                            if (newMoney > oldMoney)
                            {
                                possibility = new[] { "Looks like someone is greedy" };
                            }
                            else
                            {
                                possibility = new[] { "Trying to get rid of some N$ ?" };
                            }
                        }
                        else
                        {
                            possibility = new []{ "Money is a reward for solving problems" };
                        }
                        
                        break;
                    case "speed":
                        if (ModifiableController.TryParse(propertyChange.Item2, out float newSpeed) &&
                            ModifiableController.TryParse(propertyChange.Item3, out float oldSpeed))
                        {
                            if (newSpeed > oldSpeed)
                            {
                                possibility = new[] { "There is more to life than increasing one's speed", "Vroum vroum" };
                            }
                            else
                            {
                                possibility = new[] { "Feeling slow maybe ?" };
                            }
                        }
                        else
                        {
                            possibility = new[] { "Speed or not speed ? This is the question" };
                        }

                        break;
                    case "health":
                        if (ModifiableController.TryParse(propertyChange.Item2, out int newHealth) &&
                            ModifiableController.TryParse(propertyChange.Item3, out int oldHealth))
                        {
                            if (newHealth <= 0)
                            {
                                possibility = new[] { "Murder is like potato chips: you can't stop with just one" };
                            }
                            else if (newHealth > oldHealth)
                            {
                                possibility = new[] { "It might help in case of explosive danger" };
                            }
                            else
                            {
                                possibility = new[] { "Not looking good" };
                            }
                        }
                        else
                        {
                            possibility = new[] { "The first wealth is health", "Health is not valuable until sickness comes" };
                        }

                        break;
                    case "name":
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
