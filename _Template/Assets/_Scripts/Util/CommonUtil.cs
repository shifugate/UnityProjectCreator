using DG.Tweening;
using Assets._Scripts.Manager.Language.Token;
using Assets._Scripts.Manager.Popup;
using Assets._Scripts.Manager.Popup.Modal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Util
{
    public static class CommonUtil
    {
        private static string dataPath = Application.dataPath;
        public static string DataPath { get { return dataPath.Replace("/Assets", "").Replace("\\Assets", ""); } }

        public static Tweener SetValue(int currentValue, int newValue, float delay = 0, float time = 1, Action<int> valueCallback = null)
        {
            int value = currentValue;

            Tweener tweener = DOTween.To(() => value, result => value = result, newValue, time)
                .SetDelay(delay)
                .OnUpdate(() => valueCallback?.Invoke(value))
                .OnComplete(() => valueCallback?.Invoke(value));

            return tweener;
        }

        public static Tweener SetValue(float currentValue, int newValue, float delay = 0, float time = 1, Action<float> valueCallback = null)
        {
            float value = currentValue;

            Tweener tweener = DOTween.To(() => value, result => value = result, newValue, time)
                .SetDelay(delay)
                .OnUpdate(() => valueCallback?.Invoke(value))
                .OnComplete(() => valueCallback?.Invoke(value));

            return tweener;
        }

        public static string GetTimeMMSS(double seconds)
        {
            if (seconds < 0)
                seconds = 0;

            TimeSpan time = TimeSpan.FromSeconds(seconds);

            return time.ToString("mm\\:ss");
        }

        public static void ShuffleList<T>(IList<T> list, int seed)
        {
            int index = list.Count;

            System.Random random = new System.Random(seed);

            while (index > 1)
            {
                index--;
                int k = random.Next(index + 1);
                T value = list[k];
                list[k] = list[index];
                list[index] = value;
            }
        }

        public static void ShowError(string error)
        {
            PopupManager.Instance.ShowModal<PopupMessage1Modal>()
                .Setup(LanguageManagerToken.common.error_token,
                    error,
                    LanguageManagerToken.common.close_token);
        }
    }
}