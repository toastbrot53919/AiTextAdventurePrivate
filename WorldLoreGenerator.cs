using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class WorldLoreGenerator : MonoBehaviour
{
    public delegate void generatorOutputSetDelegate(string output);
    public generatorOutputSetDelegate generatorOutputSet;
    public TextGeneratorClient textGenerator;
    public ImageGenerator imageGenerator;
    public WorldLore worldLore;

    public TreeManager treeManager;
    public TreeUi treeUI;
    public int updateCounter = 0;



    public void Start(){
        treeManager = new TreeManager(worldLore.WorldHeadNode);
    }

    public async Task FillTree(TreeNode worldRoot)
    {
        TreeManager treeManager = new TreeManager(worldLore.WorldHeadNode);
        await FillLevel(worldRoot, 1, "Region");  // Angenommen, wir wollen 3 Regionen generieren
        foreach (Region regionNode in worldRoot.Children)
        {
            await FillLevel(regionNode, 1, "Area" );  // Angenommen, wir wollen 3 Lokale pro Region generieren
        }
    }

    // Ähnliche Methoden für die restlichen Ebenen...

    public async Task FillLevel(TreeNode parent, int numberOfChildren, string levelType)
    {
        string siblingsSummary = string.Empty;
 
        string prompt = GeneratePrompt(levelType, parent, numberOfChildren);
        if (Application.isPlaying == false)
        {
            return;
        }
        int tryCounter = 0;
        List<(string, string)> children = new List<(string, string)>();
        string childListText = "";
        while (tryCounter < 5)
        {
            childListText = await textGenerator.SendPrompt(prompt, 100);
            // Parsen der Kinderliste
            children = textPharse.ParseDoubleEntryList(childListText);
            if(children.Count==0){
                tryCounter++;
                Debug.LogError("List Reading ERROR TRY: "+tryCounter);
                
            }
            else{
                Debug.LogError("List Reading Succses");
                break;
            }
        }
        // Parsen der Kinderliste
        string helpString="";
        foreach (var childstring in children)
        {
            TreeNode childNode;
            switch (levelType)
            {
                case "Region":
                    childNode = new Region(childstring.Item1, childstring.Item2);
                    break;
                case "Area":
                    childNode = new Area(childstring.Item1, childstring.Item2);
                    break;
                default:
                    throw new ArgumentException("Unknown level type.");
            }
            parent.AddChild(childNode);
            helpString +=childstring.Item1+", "+childstring.Item2+"\n";
        }
        // Nachdem alle Kinder generiert wurden, erstellen Sie eine Zusammenfassung
       // parent.childrenSummary = await SummaryGenerator.GenerateChildSummary(textGenerator, helpString);
        parent.shortDesc = await SummaryGenerator.GenerateExtendedSummary(textGenerator, parent.Description, helpString);


        treeUI.RenderTree();
    }
    

    public string GeneratePrompt(string levelType, TreeNode parentNode, int numberOfChildren)
    {
        string prompt = levelType switch
        {
            "World" => $"<s>[INST] You are a Tool ,Please provide a brief description focusing on key properties, rules, and lifestyle of inhabitants from this text: '{parentNode.Description}'. Keep yourself short [/INST]",
            "Region" => $"<s>[INST] You are a Tool to Generate {numberOfChildren} different regions in a world described as '{parentNode.Description}'. Each description should focus on climate, cultures, species, and significant landmarks. Formate it like a list with 'Name - Description' [/INST]",
            "Area" => $"<s>[INST] You are a Tool to Generate {numberOfChildren} different areas in  a region called '{parentNode.Name}' described as '{treeManager.returnWorldString(parentNode)}'.  Each description should focus on a Location, smaller landmarks. Formate it like a list with 'Name - Description'. [/INST]",
            _ => throw new ArgumentException("Unknown level type.")
        };
        return prompt;
    }

    public async Task<string> GenerateDetailedWorldDescription(string worldDescription)
    {
        // Construct the prompt for the text generator using string interpolation
        string prompt = $"<s>[INST] Based on: \"{worldDescription}\", elaborate extensively. Highlight core elements, delve into lore, explore its laws, detail inhabitants, and vividly portray the setting. Ensure original details are expanded upon. Aim for a thorough world description. [/INST]</s>";
        // Set the target for the result and send the prompt to the generator
        string anwser = await textGenerator.SendPrompt(prompt, 100); // Increased length for a more detailed response
        return anwser;
    }


    /// <summary>
    /// Generates keywords to guide the visualization of a detailed world description. 
    /// The keywords will aid in creating a visual representation, like a book cover.
    /// </summary>
    public async Task<string> GenerateVisualKeywordsFromWorldDescription()
    {
        // Construct a strict prompt for the text generator
        string prompt = $"<s>[INST] Based on the provided detailed world description: \"{worldLore.WorldDescriptionShortend}\", produce ONLY keywords for a visual representation. Focus on ground lore, prominent landmarks, lighting effects, and other visual elements. STRICTLY keywords ONLY, without any explanations. Aim for around 50 keywords. [/INST]</s>";

        // Set the target for the result and send the prompt to the generator
        return await textGenerator.SendPrompt(prompt, 100);
        //the result gets catched by the calling function
    }


    /// <summary>
    /// Generates a concise summary of the provided world description.
    /// </summary>
    /// <param name="worldDescription">The detailed description of the world.</param>
    public async Task<string> GenerateShortSummaryWorldDescription(string longDesc)
    {
        // Construct a prompt for the text generator that emphasizes brevity
        string prompt = $"<s>[INST] Based on the provided detailed world description: \"{longDesc}\", produce a concise and brief summary. Capture the essence and key features of the world in a short paragraph. [/INST]</s>";

        // Set the target for the result and send the prompt to the generator
        string anwser = await textGenerator.SendPrompt(prompt, 50);  // Adjusted length to encourage a shorter response
        return anwser;
    }

}
