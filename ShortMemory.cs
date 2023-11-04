
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

public class ShortMemory : MonoBehaviour
{
    
    public List<string> conversation = new List<string>();
    
    public List<string> longMemoryConversation = new List<string>();


    public TextGeneratorClient textGenerator;
    public int maxtokens;
    public void addToConversation(string s)
    {
        conversation.Add(s);
        if (conversation.Sum(item => item.Length) > maxtokens)
        {
           generateSummaryOfConversation();
        }
    }

    public async void generateSummaryOfConversation()
    {
        string total = "";
        foreach(string s in conversation)
        {
            total += " \n" +s;
        }
        string prompt = $"<s>[INST] here is a Text: {total} . Your task is to reduce the Amount of Tokens as much as you can." +
            $"Only anwser with the Short version. [/INST]";
        string shortVersion = await textGenerator.SendPrompt(prompt, 50);
        longMemoryConversation.Add(shortVersion);
        conversation.Clear();
    }
    public string returnConversation()
    {
        string returns = "";
        foreach (string s in conversation)
        {
            returns += s +" ";
        }
        return returns;
    }
    public string returnLongMemory()
    {
        string returns = "";
        foreach (string s in longMemoryConversation)
        {
            returns += s + " ";
        }
        return returns;
    }

}


