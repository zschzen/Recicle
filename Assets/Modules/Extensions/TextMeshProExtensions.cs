using DG.Tweening;
using TMPro;

namespace CustomExtensions
{
    public static class TextMeshProExtensions
    {
        
        /// <summary>
        /// Fade the text out, then update the text, then fade the text in.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="newText"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static Tween DOText(this TMP_Text text, string newText, float duration = 0.5f)
        {
            return text.DOFade(0, duration)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    text.text = newText;
                    text.DOFade(1, duration);
                });
        }
    }
}