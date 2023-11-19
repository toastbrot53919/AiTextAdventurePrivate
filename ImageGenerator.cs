using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ImageGenerator : MonoBehaviour
{
    [Serializable]
    public class RequestData
    {
        public string[] init_images;
        public string prompt;
        public int steps;
        public float denoising_strength;
    }

    [Serializable]
    public class ResponseData
    {
        public string[] images;
    }


    public void Start()
    {
   
    }
    private string api = "http://192.168.2.34:7860";
    public Texture2D inputTexture;

    public async Task<Texture2D> GenerateImage(string text, int steps)
    {
        return await GenerateImageCoroutine(text, steps);
    }
    private IEnumerator GenerateImageToImageCoroutine(string text, int steps, Texture2D inputTexture,float denoiseStr)
    {
        // Convert Texture2D to byte array
        byte[] imageBytes = inputTexture.EncodeToJPG();

        // Prepare request data
        var requestData = new RequestData
        {
            init_images = new string[] { Convert.ToBase64String(imageBytes) },
            prompt = text,
            steps = steps,
            denoising_strength = denoiseStr
        };
        string jsonPayload = JsonUtility.ToJson(requestData);

        Debug.Log("Image GEneration Promopt: "+text);
        // Send POST request
        UnityWebRequest www = new UnityWebRequest($"{api}/sdapi/v1/img2img", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        // Handle response
        if (www.result == UnityWebRequest.Result.Success)
        {
            var responseData = JsonUtility.FromJson<ResponseData>(www.downloadHandler.text);
            string encodedResult = responseData.images[0];
            byte[] resultData = Convert.FromBase64String(encodedResult);
            Texture2D outputTexture = new Texture2D(2, 2);
            outputTexture.LoadImage(resultData);

            // Set the output image

            Debug.Log("Image set to RawImage component");
        }
        else
        {
            Debug.LogError($"Error: {www.error}");
        }
    }
    private async Task<Texture2D> GenerateImageCoroutine(string text, int steps)
    {
        // Convert Texture2D to byte array
   
        // Prepare request data
        var requestData = new RequestData
        {
      
            prompt = text,
            steps = steps,
           
        };
        string jsonPayload = JsonUtility.ToJson(requestData);

        // Send POST request
        Debug.Log("Image generation Promp:"+text);

        UnityWebRequest www = new UnityWebRequest($"{api}/sdapi/v1/txt2img", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        await www.SendWebRequest();

        // Handle response
        if (www.result == UnityWebRequest.Result.Success)
        {
            var responseData = JsonUtility.FromJson<ResponseData>(www.downloadHandler.text);
            string encodedResult = responseData.images[0];
            byte[] resultData = Convert.FromBase64String(encodedResult);
            Texture2D outputTexture = new Texture2D(2, 2);
            outputTexture.LoadImage(resultData);
            // Set the output image
            return outputTexture;
        }
        else
        {
            Debug.LogError($"Error: {www.error}");
            return null;
        }
    }
}
