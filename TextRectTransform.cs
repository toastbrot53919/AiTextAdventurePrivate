using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextRectTransform : MonoBehaviour
{
    private TMPro.TMP_Text tmpText;

    private void Awake()
    {
        tmpText = GetComponent<TMPro.TMP_Text>();
    }

    private void Update()
    {
        AdjustSize();
    }

    void AdjustSize()
    {
        tmpText.ForceMeshUpdate(); // Important to make sure text info is updated

        Vector2 newSize = new Vector2(tmpText.preferredWidth, tmpText.preferredHeight);
        tmpText.rectTransform.sizeDelta = newSize;
    }
}
