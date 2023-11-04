
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class Ui : MonoBehaviour
{
    public GameObject ViewPortChat;

    public TMP_Text t;
    public TMPro.TMP_InputField inputfield;
    public GameEngine engine;
    public RawImage MainCanvas;


    public void setImageMainCanvas(Texture2D tex)
    {
        MainCanvas.texture = tex;

    }
    public void addChatText(string s)
    {
        // Instantiate a new Text object from the prefab
        GameObject newTextObject = Instantiate(t.gameObject, ViewPortChat.transform);
        // Get the Text component and set the text
        Text textComponent = newTextObject.GetComponent<Text>();
        newTextObject.GetComponent<NewlineAtWidth>().SetTextWithNewlines(s);


    }
    public void pressSend()
    {
        if (engine.isReadyforInput == true){

            string input = inputfield.text;
            addChatText(input);
            engine.ProgressGameByPlayer(input);
        }
        else
        {
            Debug.LogWarning("ENGINE NOT READY PLS WAIT");
        }
    }
}


