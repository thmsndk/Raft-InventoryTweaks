using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace thmsn.InventoryTweaks
{
    public static class UIHelper
    {
        // Taken from the statistics mod by Aidanamite
        public static GameObject CreateText(Transform canvas_transform, float x, float y, string text_to_print, int font_size, Color text_color, float width, float height, Font font, string name = "Text")
        {
            GameObject UItextGO = new GameObject("Text");
            UItextGO.transform.SetParent(canvas_transform, false);
            RectTransform trans = UItextGO.AddComponent<RectTransform>();
            trans.sizeDelta = new Vector2(width, height);
            trans.anchoredPosition = new Vector2(x, y);
            Text text = UItextGO.AddComponent<Text>();
            text.text = text_to_print;
            text.font = font;
            text.fontSize = font_size;
            text.color = text_color;
            text.name = name;
            Shadow shadow = UItextGO.AddComponent<Shadow>();
            shadow.effectColor = new Color();
            return UItextGO;
        }

        public static void AddTextShadow(GameObject textObject, Color shadowColor, Vector2 shadowOffset)
        {
            Shadow shadow = textObject.AddComponent<Shadow>();
            shadow.effectColor = shadowColor;
            shadow.effectDistance = shadowOffset;
        }
        public static void CopyTextShadow(GameObject textObject, GameObject shadowSource)
        {
            Shadow sourcesShadow = shadowSource.GetComponent<Shadow>();
            if (sourcesShadow == null)
                sourcesShadow = shadowSource.GetComponentInChildren<Shadow>();
            AddTextShadow(textObject, sourcesShadow.effectColor, sourcesShadow.effectDistance);
        }
    }
}
