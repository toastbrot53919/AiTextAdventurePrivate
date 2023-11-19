using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
public class Main : MonoBehaviour
{
    public WorldLoreGenerator worldLoreGenerator;
    public WorldLore worldLore;
    public TextGeneratorClient textGenerator;
    public ImageGenerator imageGenerator;
    public EntityExtractorClient entityExtractor;
    public TreeUi TreeUi;
    public RawImage MainCanvas;

    public GameEngine gameEngine;
    Task mainTask;
    public void Start()
    {
        try
        {
            mainTask = Task.Run(() => run());
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async Task testtest()
    {
        Debug.Log("testtest");
        await Task.Delay(1000);
        Debug.Log("testtest2");

    }

    public async Task testRunRelations()
    {
        // string test = await textGenerator.SendPrompt("<s>[INST] generate a random long sentence[INST]", 60);
        // Debug.Log(test);
        string testSimple = await gameEngine.simplifyText("'I am a doctoc who lives in new york city and is a friend of Tony Stark and Bruce Banner',says Peter Parker");

        List<string> listResult = textPharse.ParseList(testSimple);
        string newString = ""  ;
        string helpString = "";
                resultExtraction result;
        List<Entity> entities = new List<Entity>() ;
        List<Relation> relations = new List<Relation>();
        List<string> resultsss = new List<string>();
        List<ParsedPhrase> parsedPhrases = new List<ParsedPhrase>();
        foreach (string s in listResult)
        {
            helpString = textPharse.ParseRemoveNumber(s);
            //helpString = await gameEngine.SimplifyString(helpString);
            Debug.Log(helpString);
            resultsss.Add(await gameEngine.extract(helpString));
           
        }
        foreach(string s in resultsss)
        {
            parsedPhrases.Add( textPharse.ParsePhrase(s));
        }
        foreach (ParsedPhrase s in parsedPhrases)
        {
            Debug.Log("RELATION: "+s.Subject +" "+s.Verb+" "+s.Object);
        }
    }

    public async void run()
    {

        await testtest();
        await testRunRelations();
        return;
        string anwser;
        //  anwser = await worldLoreGenerator.GenerateDetailedWorldDescription("a fantasy world everythig Huge Breasts.furrys,lolis,fereals,anthro anything has huge breasts.");
        //  Debug.Log(anwser);
        //  worldLore.WorldDescriptionBloated = anwser;
        //  anwser = await worldLoreGenerator.GenerateShortSummaryWorldDescription();
        //  Debug.Log(anwser);
        // worldLore.WorldDescriptionShortend = anwser;

        Debug.Log("Main Start");
        try
        {
            await entityExtractor.ExtractEntities("hello are you there ,Peter parker is a doctor who lives in new york city and is a friend of Tony Stark and Bruce Banner");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        Debug.Log("Entity Extraction End");
        Debug.Log("World Generation Start");

        string test = await textGenerator.SendPrompt("Hello, this is a test prompt", 20);
        worldLore.WorldHeadNode = new TreeNode("World", "Medival Fantasy world");
        TreeUi.rootNode = worldLore.WorldHeadNode;
        Debug.Log("World Generation Start11");

        worldLore.WorldHeadNode.Description = textGenerator.SendPrompt(worldLoreGenerator.GeneratePrompt("World", worldLore.WorldHeadNode, 1), 200).Result;
        Debug.Log("World Generation Start222");
        anwser = await worldLoreGenerator.GenerateShortSummaryWorldDescription(worldLore.WorldHeadNode.Description);
        Debug.Log("World Generation Start333");
        worldLore.WorldHeadNode.shortDesc = anwser;
        Debug.Log("World Generation Start444");
        await worldLoreGenerator.FillTree(worldLore.WorldHeadNode);
        Debug.Log("World Generation Star555t");
        TreeManager treeManager = worldLore.treeManager;


        Debug.LogError("World Generation End");


        await gameEngine.SwitchArea(treeManager.returnRandomLocal(worldLore.WorldHeadNode));

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