using CustomExtensions;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Modules.UIScreen
{
    public class UIScreenWave : UIScreen
    {
        // Fields ----------------------------------------------------------------------------------
        [SerializeField] private TMP_Text m_waveText;
        [SerializeField] private TMP_Text m_timeText;

        // Public methods --------------------------------------------------------------------------

        public void UpdateWaveText(int wave)
        {
            m_waveText.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f).SetEase(Ease.OutBack);
            m_waveText.DOText($"Wave: {wave}", 0.5f).SetEase(Ease.Flash);
        }

        public void SetTimer(uint time)
        {
            // convert timer to format mm:ss
            var minutes = time / 60;
            var seconds = time % 60;
            var timeString = $"{minutes:00}:{seconds:00}";
            m_timeText.text = timeString;
        }
    }
}