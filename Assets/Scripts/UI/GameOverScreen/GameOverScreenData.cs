using UnityEngine;

[CreateAssetMenu(fileName ="GameOverScreenData")]
public class GameOverScreenData : ScriptableObject
{
    
    [SerializeField] private GameOverScreenController.GameOverType gameOverReason;
    
    [SerializeField] private string gameOverScreenTitle;
    
    [SerializeField] private string gameOverReasonText;
    
    [TextArea(15, 20)]
    [SerializeField] private string endGameLore;
    
    public GameOverScreenController.GameOverType reason => gameOverReason;
    public string reasonText => gameOverReasonText;
    public string title => gameOverScreenTitle;
    public string lore => endGameLore;

    public GameOverScreenData()
    {
        gameOverReason = (GameOverScreenController.GameOverType)(-1);
        gameOverReasonText = "";
        gameOverScreenTitle = "";
        endGameLore = "";
    }
}
