using Newtonsoft.Json.Linq;
using System.Net.Http;
using UnityEngine.Networking;

namespace Assets._Scripts.Util
{
    public static class HttpUtil
    {
        public static bool HasRequestError(UnityWebRequest request)
        {
            return request.result == UnityWebRequest.Result.ConnectionError
                || request.result == UnityWebRequest.Result.ProtocolError
                || request.result == UnityWebRequest.Result.DataProcessingError;
        }
		
		public static void DisposeClient(HttpClient client)
		{
			try
			{
				if (client != null)
					client.Dispose();
			}
			catch
			{
			}
		}

		public static string GetError(string data)
		{
			try
			{
				JObject json = JObject.Parse(data);

				if (json != null && json.ContainsKey("error"))
					return json.Value<string>("error");
			}
			catch
			{

			}

			return null;
		}
    }
}
