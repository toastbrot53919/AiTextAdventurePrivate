using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NewlineAtWidth : MonoBehaviour
{
    public float maxWidth = 300f; // Width at which to break the line

    private TextMeshProUGUI tmpText;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    public void SetTextWithNewlines(string text)
    {
        tmpText.text = InsertNewlines(text);
    }

    private string InsertNewlines(string text)
    {
        string[] words = text.Split(' ');
        string result = "";
        string line = "";

        foreach (string word in words)
        {
            // Test adding the next word to the line
            string testLine = line + word + " ";

            tmpText.text = testLine;
            tmpText.ForceMeshUpdate();

            if (tmpText.preferredWidth > maxWidth)
            {
                // Add current line to result and reset line
                result += line + "\n";
                line = word + " ";
            }
            else
            {
                line = testLine;
            }
        }

        // Add the last line to result
        result += line;

        return result.Trim(); // Trim to remove any trailing spaces
    }
}
