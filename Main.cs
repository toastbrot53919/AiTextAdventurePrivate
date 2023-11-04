using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
public class Main : MonoBehaviour 
{
    public WorldLoreGenerator worldLoreGenerator;
    public WorldLore worldLore;
    public TextGeneratorClient textGenerator;
    public ImageGenerator imageGenerator;
    public TreeUi TreeUi;
    public RawImage MainCanvas;

    public GameEngine gameEngine;
    Task mainTask;
    public void Start()
    {
       mainTask= Task.Run(() => run());
    }
    public void OnDisable()
    {
        mainTask.Dispose();
    }
    public async void run()
    {
        string anwser;
       //  anwser = await worldLoreGenerator.GenerateDetailedWorldDescription("a fantasy world everythig Huge Breasts.furrys,lolis,fereals,anthro anything has huge breasts.");
      //  Debug.Log(anwser);
      //  worldLore.WorldDescriptionBloated = anwser;
      //  anwser = await worldLoreGenerator.GenerateShortSummaryWorldDescription();
      //  Debug.Log(anwser);
       // worldLore.WorldDescriptionShortend = anwser;
        
        worldLore.WorldHeadNode = new TreeNode("World","Medival Fantasy world");
        TreeUi.rootNode = worldLore.WorldHeadNode;
        worldLore.WorldHeadNode.Description = await textGenerator.SendPrompt(worldLoreGenerator.GeneratePrompt("World",worldLore.WorldHeadNode,1),200);
        anwser = await worldLoreGenerator.GenerateShortSummaryWorldDescription(worldLore.WorldHeadNode.Description);
        worldLore.WorldHeadNode.shortDesc = anwser;
        await worldLoreGenerator.FillTree(worldLore.WorldHeadNode);

        TreeManager treeManager = worldLore.treeManager;

      
        Debug.LogError("World Generation End");


        await gameEngine.SwitchArea(treeManager.returnRandomLocal());

        Debug.LogError("Main END");

       
    }
    public void setMainCanvas(Texture2D text)
    {
        MainCanvas.texture = text;
    }

    public bool IsListOfKeywords(string input)
    {
        // Split the string by comma and remove leading/trailing whitespaces
        string[] keywords = input.Split(',').Select(k => k.Trim()).ToArray();

        // Define criteria for valid keywords
        int minLength = 3;  // Minimum length of a keyword
        int maxLength = 25; // Maximum length of a keyword for it to be considered valid for a brief world description

        foreach (var keyword in keywords)
        {
            if (keyword.Length < minLength || keyword.Length > maxLength)
            {
                return false;  // Invalid keyword found
            }
        }

        return true; // All keywords are valid
    }
}