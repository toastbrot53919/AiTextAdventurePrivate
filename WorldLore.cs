
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity;
using System.Security.Cryptography;
using NUnit.Framework;
using System.Numerics;
using System.Security;
using UnityEngine;
using Unity.Mathematics;

public class WorldLore : MonoBehaviour
{
    public string PlayerWorldDescroptio;
    public string WorldLoreName;
    public string tags;

    public string WorldDescriptionBloated;
    public string WorldDescriptionShortend;

    public TreeManager treeManager;
    public CharactersManager charactersManager;
    public TreeNode WorldHeadNode;

    public void Start()
    {
        treeManager = new TreeManager(WorldHeadNode);
        charactersManager = new CharactersManager();
    }



}

[System.Serializable]
public class crossRef
{
    public TreeNode node;
    public string relationDescripton;

}

[System.Serializable]
public class TreeNode
{
    public string Name;
    public string Description;
    public string type;
    public string shortDesc;
    public List<TreeNode> Children;
    public string childrenSummary;
    public TreeNode Parent;

    public List<crossRef> relations;
    public TreeNode(string name, string description)
    {
        Name = name;
        Description = description;
        Children = new List<TreeNode>();
        Parent = null;
        relations = new List<crossRef>();
        type = default;
    }

    public void AddChild(TreeNode child)
    {
        Children.Add(child);
        child.Parent = this;
    }

    public void RemoveChild(TreeNode child)
    {
        Children.Remove(child);
        child.Parent = this;
    }

    // Weitere Methoden zum Verwalten und Abrufen von Informationen...
}


public class World : TreeNode
{
    public World(string name, string description) : base(name, description)
    {
        
    }

    // Spezifische Methoden und Eigenschaften für die Welt-Ebene...
}

public class Region : TreeNode
{
    public Region(string name, string description) : base(name, description)
    {
        type = "Region";
        // Spezifische Initialisierung für die regionale Ebene
    }

    // Spezifische Methoden und Eigenschaften für die regionale Ebene...
}

public class CharacterNode : TreeNode
{
    public string VisualDescription { get; set; }
    public ShortMemory Memory { get; set; }
    public CharacterNode(string name, string description)
        : base(name, description)
    {
        Memory = new ShortMemory();
    }
    public CharacterNode(string name, string description, string visualDescription, string memory)
        : base(name, description)
    {
        VisualDescription = visualDescription;
        Memory = new ShortMemory();
        Memory.addToConversation(memory);
    }
}

public class ObjectNode : TreeNode
{
    public ObjectNode(string name, string description) : base(name, description) { }
}

public class Area : TreeNode
{
    public List<CharacterNode> Characters { get; private set; }
    public List<ObjectNode> Objects { get; private set; }

    public Area(string name, string description) : base(name, description)
    {
        type = "Area";
        Characters = new List<CharacterNode>();
        Objects = new List<ObjectNode>();
    }

    public void AddCharacter(CharacterNode character)
    {
        Characters.Add(character);
        character.Parent = this;
    }

    public void AddObject(ObjectNode objectNode)
    {
        Objects.Add(objectNode);
        objectNode.Parent = this;
    }
}


public class TreeManager
{
    public TreeNode Root { get; private set; }

    public TreeManager(TreeNode root)
    {
        Root = root;
    }

    // Suchfunktion, um Knoten im Baum zu finden...
    public TreeNode FindNode(string name)
    {
        // Implementierung der Suchfunktion...
        return null;
    }
    public string returnWorldString(TreeNode leaveNode)
    {
        // If the leaveNode is the root, return its description immediately
        if (leaveNode.Parent == null)
        {
            return leaveNode.Description;
        }

        // Initialize the result string with the description of leaveNode
        string s = leaveNode.Description;

        // Start with the parent of leaveNode
        TreeNode akkTreeNode = leaveNode.Parent;

        // Traverse up the tree, appending the shortDesc of each node
        while (akkTreeNode != null)
        {
            s += " \n " + akkTreeNode.shortDesc;
            akkTreeNode = akkTreeNode.Parent;
        }

        return s;
    }

    public Area returnRandomLocal()
    {
        if (Root == null)
        {
            return null;  // or handle this case as appropriate
        }

        List<TreeNode> leaves = new List<TreeNode>();
        collectLeaves(Root, leaves);

        if (leaves.Count == 0)
        {
            Debug.LogError("No Children found");
            return null;  // or handle this case as appropriate
        }

        TreeNode randomLeaf = leaves[new System.Random().Next(leaves.Count)];
        if (randomLeaf is Area)
        {
            return randomLeaf as Area;
        }
        else
        {
            Debug.LogWarning("Random leaf node is not an Area: " + randomLeaf.GetType().Name);
            return null;
        }
    }

    private void collectLeaves(TreeNode node, List<TreeNode> leaves)
    {

        if (node.type == "Area")
        {
            leaves.Add(node);

            Debug.Log("Leaf node type: " + node.GetType().Name);
        }
        else
        {
            foreach (TreeNode child in node.Children)
            {
                collectLeaves(child, leaves);
            }
        }
    }


    // Weitere Verwaltungs- und Hilfsfunktionen...
}
public class CharactersManager
{
    public List<CharacterNode> allCharacters;

    public CharactersManager()
    {
        allCharacters = new List<CharacterNode>();
    }

    public void addNewCharacters(CharacterNode c)
    {
        allCharacters.Add(c);
    }
    public CharacterNode searchByName(string name)
    {
        foreach (CharacterNode c in allCharacters)
        {
            if (c.Name == name)
            {
                return c;
            }
        }
        return null;
    }
    public void removeCharacter()
    {
        Debug.LogError("NNNANANANNANANANNA");
    }
}