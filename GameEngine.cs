using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using NUnit.Framework;
using Unity.Profiling.Editor;
using System.Text;

public class GameEngine : MonoBehaviour
{
    // Start is called before the first frame update


    public ShortMemory shortMemory;
    public WorldLore worldLore;

    public WorldLoreGenerator worldLoreGenerator;

    public TextGeneratorClient textGeneratorClient;
    public ImageGenerator imageGenerator;

    public EntityExtractorClient entityExtractorClient;
    public Ui ui;

    public bool isReadyforInput = false;
    public Area playerArea;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ProgressGameByAi()
    {

    }
    public async Task<bool> ProgressGameByPlayer(string playerinput)
    {
        Debug.Log("ProgressGameByPlayer");
        isReadyforInput = false;
        string anwserComplex = await InterpretPlayerInput(playerinput);
        string anwserSimple = await simplifyText(anwserComplex);


        await ProcessTextForWorldChanges(anwserSimple);



        // Texture2D img = await imageGenerator.GenerateImage(prompt, 17);
        // ui.setImageMainCanvas(img);
        shortMemory.addToConversation(anwserSimple);
        try
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() => { ui.addChatText(anwserComplex); });
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        isReadyforInput = true;
        return true;
    }

    public async Task<string> InterpretPlayerInput(string playerinput)
    {
        StringBuilder promptBuilder = new StringBuilder();
        promptBuilder.AppendLine("<s>[INST] Text Adventure Interpretation [/INST]");
        promptBuilder.AppendLine($"World Context: {worldLore.treeManager.returnWorldString(playerArea)}");
        promptBuilder.AppendLine($"Previous Events: {shortMemory.returnLongMemory()}");
        promptBuilder.AppendLine($"Recent Conversation: {shortMemory.returnConversation()}");
        promptBuilder.AppendLine("[INST] Interpretation Rules: ");
        promptBuilder.AppendLine("1. The player decides their actions.");
        promptBuilder.AppendLine("2. Narrate the direct results of the player's action.");
        promptBuilder.AppendLine("3. Then, pose a question for the player's next action.");
        promptBuilder.AppendLine("4. Do not progress the story without the player's input.");
        promptBuilder.AppendLine("5. Respond based on the player's action or relevant story elements.");
        promptBuilder.Append($"Player's Input: '{playerinput}'[/INST]");

        string anwser = await textGeneratorClient.SendPrompt(promptBuilder.ToString(), 120);

        return anwser;
    }
    public async Task<string> simplifyText(string text)
    {
        string prompt = $"Original text: '{text}'. <s>[INST]Rewrite the text for NLP analysis to clearly state the relationships. Break it into distinct sentences with explicit subject, verb, and object. Include implied details, ensuring each sentence is simple and uses active voice.[/INST]";
        string result = await textGeneratorClient.SendPrompt(prompt, 100, 0.3f);
        return result;
    }
public async Task<string> SimplifyString(string text)
{
    string prompt = $"Text: '{text}'. [INST] Rewrite in simple 'subject-verb-object' structure. Keep one idea per sentence, clear and direct. Preserve original meaning.[/INST]";
    string result = await textGeneratorClient.SendPrompt(prompt, 55, 0.3f);
    result = textPharse.ParseRemoveNumber(result);
    return result;
}
public async Task<string> extract(string text)
{
    string prompt = $"Sentence: '{text}'. [INST]Return Only the subject of the Sentence,the Verb And the Object.input the information about their relaition inside the Verb.[/INST]";
    string result = await textGeneratorClient.SendPrompt(prompt, 55, 0.3f);
    result = textPharse.ParseRemoveNumber(result);
    return result;
}

    public async Task ProcessTextForWorldChanges(string machineText)
    {
        Debug.Log("ProcessTextForWorldChanges");

        await checkForEntities(machineText);

        //await CheckForChangesHappend(machineText);


    }
    public async Task updateNodeInfoWithText(TreeNode node, string updateText)
    {
        Debug.Log("updateNodeInfoWithText " + node.Name);
        string prompt = $"<s>[INST]Here is a Description of {node.Name} :{node.Description}. The following happend: {updateText}. return a updated description. keep yourelf short without losing details. [/INST]";
        string result = await textGeneratorClient.SendPrompt(prompt, 80);
        node.Description = result;

    }
    public async Task CheckForChangesHappend(string machineText)
    {
        Debug.Log("CheckForChangesHappend");
        string prompt = $" <s>[INST]Here is a text from a Novel. Did something happen that changed? If yes, return a list of names of Regions/Characters/objects that changed in this text. anwser in this format:'Name,Description on how changed'[/INST]. if nothing changed anwser with 'no'";
        string result = await textGeneratorClient.SendPrompt(prompt, 50);
        TreeNode treeNode;
        if (!textPharse.checkForNoChanges(result))
        {

            List<(string, string)> resultList = textPharse.ParseDoubleEntryList(result);
            foreach ((string, string) entry in resultList)
            {
                treeNode = worldLore.treeManager.FindNode(entry.Item1);
                if (treeNode != null)
                {
                    await updateNodeInfoWithText(treeNode, entry.Item2);
                    Debug.Log("Updated Node");
                }
                else
                {
                    await insertNewTreeNodeFormText(entry.Item1, entry.Item2);
                }

            }
        }
        else
        {
            Debug.Log("No Changes");
        }


    }

    public async Task insertNewTreeNodeFormText(string name, string desc)
    {
        Debug.Log("insertNewTreeNodeFormText" + name + " " + desc);

        string prompt = $"<s>[INST] Decide if the following text is talking of a Region,Area,Character or Object.The Text:'{name},{desc}'  only anwser with one of the following: 'Region','Area','Character','Object'[/INST]";
        string result = await textGeneratorClient.SendPrompt(prompt, 15);

        string parentPrompt = $"<s>[INST] Identify the parent node for the following text: '{name},{desc}'. Return the name of the parent node, or 'none' if no parent node can be identified. [/INST]";
        string parentName = await textGeneratorClient.SendPrompt(parentPrompt, 50);

        TreeNode parentNode = worldLore.treeManager.FindNode(parentName);
        if (parentNode != null)
        {
            TreeNode newNode;
            switch (result)
            {
                case "Region":
                    newNode = new Region(name, desc);
                    break;
                case "Area":
                    newNode = new Area(name, desc);
                    break;
                case "Character":
                    newNode = new CharacterNode(name, desc);
                    break;
                case "Object":
                    newNode = new ObjectNode(name, desc);
                    break;
                default:
                    throw new ArgumentException("Unknown node type.");
            }

            parentNode.AddChild(newNode);
            Debug.Log("Added new Node");
        }
        else
        {
            Debug.LogError("No Parent Node found" + parentPrompt);
        }

    }

    public async Task checkForEntities(string machineText)
    {
        Debug.Log("checkForEntities");
        resultExtraction entities = await getEntities(machineText);
        CharacterNode help;

        Area area;




      /*  foreach (Entity entity in entities.entities)
        {
            Debug.Log(entity.text + " " + entity.label);
            if (entity.label == "PERSON")
            {
                help = worldLore.charactersManager.searchByName(entity.text);
                if (help != null)
                {
                    await updateNodeInfoWithText(help, machineText);
                }
                else
                {
                    help = await generateNewCharacter(machineText, worldLore.treeManager.returnWorldString(playerArea), entity.text, "");
                    playerArea.Characters.Add(help);
                }
            }
*/

            //did something happen that he needs to remember
        
        TreeNode subject;
        TreeNode objects;
        string verb;
        foreach (Relation relation in entities.relationships)
        {
            subject = worldLore.treeManager.FindNode(relation.subject);
            objects = worldLore.treeManager.FindNode(relation.objects);
            verb = relation.verb;
            if (subject == null || objects == null)
            {
                Debug.LogError("Nodes for relatios not found!!!" + relation.subject + " " + relation.objects);
                continue;
            }
            else
            {
                worldLore.treeManager.addRelationsship(subject, objects, verb);
            }
        }
    }

    public async Task<CharacterNode> generateNewCharacter(string lastHappenings, string worldString, string Name, string otherInfos)
    {
        string prompt = $"{worldString} . {lastHappenings} . {otherInfos} . <s>[INST] generate a short description for the Character: {Name} [/INST]";

        CharacterNode characterNode = new CharacterNode("", "");
        string result = await textGeneratorClient.SendPrompt(prompt, 70);
        string promptVisuall = $"<s>[INST] {result}. Generate a short Visual Description of '{Name}'. keep your focus on details. try to use keywords. [/INST]";
        string visual = await textGeneratorClient.SendPrompt(promptVisuall, 50);
        characterNode.Name = Name;
        characterNode.Description = result;
        characterNode.shortDesc = result;
        characterNode.VisualDescription = visual;

        worldLore.charactersManager.addNewCharacters(characterNode);

        return characterNode;
    }

    public async Task<resultExtraction> getEntities(string s)
    {
        resultExtraction result = await entityExtractorClient.ExtractEntities(s);

        return result;
    }
    public async Task SwitchArea(Area treeNode)
    {
        playerArea = treeNode;
        bool a = await ProgressGameByPlayer($"We arrive at {playerArea.Name} . Descibe the Area and what we see.");
    }
}
