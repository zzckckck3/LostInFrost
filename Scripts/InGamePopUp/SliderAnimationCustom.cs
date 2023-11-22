// Copyright (C) 2015-2021 gamevanilla - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateClean
{
    /// <summary>
    /// This component is used to provide idle slider animations in the demos.
    /// </summary>
    public class SliderAnimationCustom : MonoBehaviour
    {
        public TextMeshProUGUI text;

        public float duration = 1;

        private Image image;
        private SlicedFilledImage slicedImage;

        private StringBuilder strBuilder = new StringBuilder(4);
        private int lastPercentage = -1;

        private void Awake()
        {
            image = GetComponent<Image>();
            slicedImage = GetComponent<SlicedFilledImage>();

            if (duration > 0)
                StartCoroutine(Animate());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private IEnumerator Animate()
        {
            while (true)
            {
                var ratio = float.Parse(text.text);
                image.fillAmount = ratio / 100;
                slicedImage.fillAmount = ratio;

                yield return null;
            }
        }
    }
}