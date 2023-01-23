using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    
    public void ToMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        gameObject.SetActive(false);
    }
}
