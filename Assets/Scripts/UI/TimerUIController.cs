using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI
{
    public class TimerUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text timerMSText;
        [SerializeField] private Image timerBackground;
        public Canvas timerCanvas;

        private void Awake()
        {
            Assert.IsNotNull(timerText);
            Assert.IsNotNull(timerBackground);
            Assert.IsNotNull(timerCanvas);
            timerCanvas.gameObject.SetActive(false);
        }

        public void DisplayTime(float time)
        {
            var minutes = Mathf.Floor(time / 60f);
            var seconds = Mathf.Floor(time % 60f);
            var ms = Mathf.Floor((time - Mathf.Floor(time)) * 100);

            var minutesText = (minutes < 10f ? "0" : "") + minutes;
            var secondsText = (seconds < 10f ? "0" : "") + seconds;
            var msText = (ms < 10f ? "0" : "") + ms;
            
            timerText.SetText(minutesText + ":" + secondsText);
            timerMSText.SetText("." + msText);
        }
    }
}