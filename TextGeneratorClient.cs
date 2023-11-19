using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

public class TextGeneratorClient : MonoBehaviour
{
    private string url = "http://192.168.2.34:5000/generate";  // Replace with your server's URL



    public void Start()
    {
        // Ensure UnityMainThreadDispatcher is initialized
        if (UnityMainThreadDispatcher.Instance == null)
        {
            gameObject.AddComponent<UnityMainThreadDispatcher>();
        }
    }

    // Method to send a prompt to the server
    // Method to send a prompt to the server, now just awaits the coroutine task
    public Task<string> SendPrompt(string prompt, int maxTokens, float temperature = 0.7f)
    {
        TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            StartCoroutine(SendPromptCoroutine(prompt, maxTokens, temperature, tcs));
        });

        return tcs.Task;
    }

    // Coroutine to handle the HTTP request
    private IEnumerator SendPromptCoroutine(string prompt, int maxTokens, float temperature, TaskCompletionSource<string> tcs)
    {
        Debug.Log("Sending prompt to server");
        using (var request = new UnityWebRequest(url, "POST"))
        {
            // Create the request body
            var body = new textGeneratorMassageBody
            {
                prompt = prompt,
                maxtokens = maxTokens,
                temperature = temperature
            };

            // Convert the body to a JSON string
            string requestBody = JsonUtility.ToJson(body);
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(requestBody);

            // Set up the UnityWebRequest
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
                Debug.Log("Sending text generation request"+requestBody);
            // Send the request and await the response
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                tcs.SetException(new Exception(request.error));
            }
            else
            {
                string responseText = request.downloadHandler.text;
                JsonResponse jsonResponse = JsonUtility.FromJson<JsonResponse>(responseText);
                Debug.Log("Received response from server: " + jsonResponse.response);
                tcs.SetResult(jsonResponse.response);
            }
        }
    }


    public class textGeneratorMassageBody
    {
        public string prompt;
        public int maxtokens;

        public float temperature;
    }
    // Coroutine to handle the HTTP request

    [System.Serializable]
    public class JsonResponse
    {
        public string response;
    }

}
