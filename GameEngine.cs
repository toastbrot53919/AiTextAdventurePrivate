using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using NUnit.Framework;
using Unity.Profiling.Editor;

public class GameEngine : MonoBehaviour
{
    // Start is called before the first frame update


    public ShortMemory shortMemory;
    public WorldLore worldLore;

    public WorldLoreGenerator worldLoreGenerator;

    public TextGeneratorClient textGeneratorClient;
    public ImageGenerator imageGenerator;
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
        isReadyforInput = false;
        string prompt = $"<s> [INST] We are playing a Text Adventure! [/INST]. The World:'{worldLore.treeManager.returnWorldString(playerArea)}   What happend Befor: {shortMemory.returnLongMemory()} . {shortMemory.returnConversation()}" +
            $"[INST] here are the rules: The player descides it's actions. think about the players anwser and Narrate what the direct result is." +
            $"then ask the player for his next actions." +
            $"Anwser only with the result of the players action or other things.Don't Progress the story without the player Wait for his actions. [/INST] Player:'{playerinput}'";
        string anwser = await textGeneratorClient.SendPrompt(prompt, 60);

        await ProcessTextForWorldChanges(anwser);


       // Texture2D img = await imageGenerator.GenerateImage(prompt, 17);
       // ui.setImageMainCanvas(img);
        shortMemory.addToConversation(anwser);
        ui.addChatText(anwser);
        isReadyforInput = true;
        return true;
    }
    public async Task ProcessTextForWorldChanges(string machineText)
    {
        await checkForCharacters(machineText);

        await CheckForChangesHappend(machineText);


    }
    public async Task updateNodeInfoWithText(TreeNode node, string updateText)
    {
        string prompt = $"<s>[INST]Here is a Description of {node.Name} :{node.Description}. The following happend: {updateText}. return a updated description. keep yourelf short without losing details. [/INST]";
        string result = await textGeneratorClient.SendPrompt(prompt, 80);
        node.Description = result;

    }
    public async Task CheckForChangesHappend(string machineText)
    {
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
        else{
            Debug.Log("No Changes");
        }


    }

    public async Task insertNewTreeNodeFormText(string name, string desc)
    {
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

    public async Task checkForCharacters(string machineText)
    {
        List<string> allNames = await checkForCharactersGetnames(machineText);
        CharacterNode help;
        Area area;
        foreach (string name in allNames)
        {
            help = worldLore.charactersManager.searchByName(name);
            if (help != null)
            {
                area = (Area)help.Parent;
                area.Characters.Remove(help);
                playerArea.Characters.Add(help);
            }
            else
            {
                help = await generateNewCharacter(machineText, worldLore.treeManager.returnWorldString(playerArea), name, "");
                playerArea.Characters.Add(help);
            }


            //did something happen that he needs to remember
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

    public async Task<List<string>> checkForCharactersGetnames(string s)
    {
        string prompt = $"<s>[INST] Hier is ein Text:'{s}'wurden charactere oder andere wesen erwähnt?oder Reden Charactere?, sie müssen nicht intelligent sein. Wenn ja: Gebe eine Liste aller Namen aus. Wenn Nein antworte mit 'Nein'. [/INST]";
        string result = await textGeneratorClient.SendPrompt(prompt, 100);
        List<string> ret = textPharse.ParseList(result);
        return ret;
    }
    public async Task SwitchArea(Area treeNode)
    {
        playerArea = treeNode;
        bool a = await ProgressGameByPlayer($"We arrive at {playerArea.Name}");
    }
}
