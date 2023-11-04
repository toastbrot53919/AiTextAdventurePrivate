using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TreeUi : MonoBehaviour
{
    public GameObject NodePrefab;
    public Transform ContentTransform;
    public float HorizontalSpacing = 100f;
    public float VerticalSpacing = 50f;

    public TreeNode rootNode;

    private Dictionary<TreeNode, RectTransform> nodeRectTransforms = new Dictionary<TreeNode, RectTransform>();

    public void RenderTree()
    {
        ClearTree();
        RenderNode(rootNode, ContentTransform.position, 0);
    }

    private void RenderNode(TreeNode node, Vector3 position, int depth)
    {
        var nodeInstance = Instantiate(NodePrefab, position, Quaternion.identity, ContentTransform);
        var rectTransform = nodeInstance.GetComponent<RectTransform>();
        nodeRectTransforms[node] = rectTransform;

        nodeInstance.GetComponentInChildren<treeUiNodeHelper>().name.text = node.Name;
        nodeInstance.GetComponentInChildren<treeUiNodeHelper>().description.text = node.Description;
        
        nodeInstance.GetComponentInChildren<treeUiNodeHelper>().shortDesc.text = node.shortDesc;
        

        float offset = 0f;
        foreach (var childNode in node.Children)
        {
            var childPosition = position + new Vector3(offset, -VerticalSpacing, 0);
            RenderNode(childNode, childPosition, depth + 1);
            offset += HorizontalSpacing / (depth + 1);
        }

        // Optional: Adjust the size of the RectTransform based on the content
        // This might require additional logic or a ContentSizeFitter component
        // rectTransform.sizeDelta = ...
    }

    private void ClearTree()
    {
        foreach (var rectTransform in nodeRectTransforms.Values)
        {
            Destroy(rectTransform.gameObject);
        }
        nodeRectTransforms.Clear();
    }
}
