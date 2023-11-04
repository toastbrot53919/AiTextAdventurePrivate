using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class TextGeneratorClient : MonoBehaviour
{
    private string url = "http://192.168.2.34:5000/generate";  // Replace with your server's URL


    public void Start()
    {
        
        //SendPrompt("hello are you there");
    }

    
    // Method to send a prompt to the server
    public async Task<string> SendPrompt(string prompt,int maxtokens)
    {
        string anwser;
        anwser = await SendPromptCoroutine(prompt,maxtokens);
        return anwser;
    }

     


    public class textGeneratorMassageBody
    {
        public string prompt;
        public int maxtokens;
    }
    // Coroutine to handle the HTTP request
    private async Task<string> SendPromptCoroutine(string prompt,int maxTokens)
    {
        Debug.Log(prompt);
        // Create a new UnityWebRequest and set the method to POST
        var request = new UnityWebRequest(url, "POST");

        // Create a JSON object with the prompt
        textGeneratorMassageBody body = new textGeneratorMassageBody();
        body.prompt = prompt;
        body.maxtokens = maxTokens;

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(body));

        // Set the request's body data and headers
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        if (Application.isPlaying == false)
        {
            return ""; // stop execution of current async method. May have to handle calling methods properly.
        }
        // Send the request and wait for a response
        await request.SendWebRequest();
   
        if (request.result != UnityWebRequest.Result.Success)
        {
            // Log any errors
            Debug.LogError(request.error);
            return " ";
        }
        else
        {
            // Parse and log the response
            string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            JsonResponse jsonResponse = JsonUtility.FromJson<JsonResponse>(response);
            string actualResponse = jsonResponse.response;
            Debug.Log(actualResponse);
            return actualResponse;
        }
    }

    [System.Serializable]
    public class JsonResponse
    {
        public string response;
    }

}
