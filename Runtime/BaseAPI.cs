using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace TaloGameServices
{
    public class BaseAPI
    {
        protected TaloManager manager;
        protected string baseUrl;

        public BaseAPI(TaloManager manager, string service)
        {
            this.manager = manager;
            baseUrl = $"{manager.settings.apiUrl}/{service}";
        }

        protected async Task<string> Call(Uri uri, string method, string content = "")
        {
            byte[] json = new System.Text.UTF8Encoding().GetBytes(content);

            using (UnityWebRequest www = new UnityWebRequest(uri, method))
            {
                if (json.Length > 0) www.uploadHandler = new UploadHandlerRaw(json);
                www.downloadHandler = new DownloadHandlerBuffer();

                www.SetRequestHeader("Authorization", $"Bearer {manager.settings.accessKey}");
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Accept", "application/json");
                www.SetRequestHeader("X-Talo-Dev-Build", Debug.isDebugBuild ? "1" : "0");
                www.SetRequestHeader("X-Talo-Include-Dev-Data", Debug.isDebugBuild ? "1" : "0");

                var op = www.SendWebRequest();

                while (!op.isDone)
                {
                    await Task.Yield();
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    return www.downloadHandler.text;
                }
                else
                {
                    string message;

                    try
                    {
                        message = JsonUtility.FromJson<ErrorResponse>(www.downloadHandler.text).message;
                    }
                    catch (Exception)
                    {
                        message = www.error;
                    }
                    throw new Exception($"Request failed: {message}");
                }
            }
        }
    }
}
