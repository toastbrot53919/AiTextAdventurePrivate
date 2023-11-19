using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;
using System;
[System.Serializable]
public class TextData
{
    public string text;
}
public class EntityExtractorClient : MonoBehaviour
{
    public void Start()
    {
        // Ensure UnityMainThreadDispatcher is initialized
        if (UnityMainThreadDispatcher.Instance == null)
        {
            gameObject.AddComponent<UnityMainThreadDispatcher>();
        }
    }

    public Task<resultExtraction> ExtractEntities(string text)
    {
        TaskCompletionSource<resultExtraction> tcs = new TaskCompletionSource<resultExtraction>();

        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            StartCoroutine(ExtractEntitiesCoroutine(text, tcs));
        });

        return tcs.Task;
    }

    private IEnumerator ExtractEntitiesCoroutine(string textContent, TaskCompletionSource<resultExtraction> tcs)
    {
        string url = "http://192.168.2.34:5000/extract_entities";
            // Create an instance of the TextData class with the text content
            TextData textData = new TextData() { text = textContent };
            // Serialize the TextData instance to a JSON string
            string json = JsonUtility.ToJson(textData);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            Debug.Log("Sending entity extraction request");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError ||
                www.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError(www.error);
                tcs.SetException(new Exception(www.error));
            }
            else if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string responseString = www.downloadHandler.text;
                    EntityListWrapper wrapper = JsonUtility.FromJson<EntityListWrapper>(responseString);
                    
                    List<Relation> relations = wrapper.relationships;
                    foreach (var relation in relations)
                    {
                        Debug.Log($"{relation.subject} {relation.verb} {relation.objects}");
                    }
                    resultExtraction result = new resultExtraction();
                    result.relationships = relations;

                    tcs.SetResult(result);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error processing response: {e}");
                    tcs.SetException(e);
                }
            }
            else
            {
                tcs.SetException(new Exception("Unknown error occurred."));
            }
        }
    }
}
[System.Serializable]
public class resultExtraction{
    public List<Relation> relationships;
}
[System.Serializable]
public class Entity
{
    public string text ;
    public string label;
}
[System.Serializable]
public class Relation
{
        public string objects;
        public string subject;
        public string verb;

}


[System.Serializable]
public class EntityListWrapper
{
    public List<Relation> relationships;

}
