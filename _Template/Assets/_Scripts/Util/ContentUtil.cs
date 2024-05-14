using Assets._Scripts.Manager.Route;
using Assets._Scripts.Manager;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Assets._Scripts.Util
{
    public static class ContentUtil
    {
        public static T Copy<T>(T data)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));
        }

        public static async Task<IList<T>> LoadContents<T>(string key)
        {
            try
            {
                IList<T> asset = await Addressables.LoadAssetsAsync<T>(key, (result) => { }).Task;

                return asset;
            }
            catch (Exception)
            {

            }

            return default(IList<T>);
        }

        public static IEnumerator LoadContents<T>(string key, Action<IList<T>> completeCallback, Action failCallback, Action<float> progressionCallback = null)
        {
            AsyncOperationHandle<IList<T>> handle = default(AsyncOperationHandle<IList<T>>);

            Exception error = null;

            try
            {
                handle = Addressables.LoadAssetsAsync<T>(key, (result) => { });
            }
            catch (Exception ex)
            {
                Debug.Log($"{key} : {ex}");

                error = ex;
            }

            if (error != null)
            {
                completeCallback?.Invoke(default(IList<T>));

                yield break;
            }

            while (handle.Status == AsyncOperationStatus.None)
            {
                progressionCallback?.Invoke(handle.PercentComplete);

                yield return null;
            }

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                failCallback?.Invoke();

                yield break;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                completeCallback?.Invoke(handle.Result);

                yield break;
            }

            completeCallback?.Invoke(default(IList<T>));
        }

        public static async Task<T> LoadContent<T>(string file)
        {
            GameObject go = await Addressables.LoadAssetAsync<GameObject>($"Assets/_Addressables/{file}").Task;
            go.name = typeof(T).Name;

            if (typeof(T).Equals(typeof(GameObject)))
                return (T)Convert.ChangeType(go, typeof(T));

            return go.GetComponent<T>();
        }

        public static IEnumerator LoadContent<T>(string file, Action<T> completeCallback, Action failCallback, Action<float> progressionCallback = null)
        {
            AsyncOperationHandle<GameObject> handle = default(AsyncOperationHandle<GameObject>);

            Exception error = null;

            try
            {
                handle = Addressables.LoadAssetAsync<GameObject>($"Assets/_Addressables/{file}");
            }
            catch (Exception ex)
            {
                //Debug.Log($"{file} : {ex}");

                error = ex;
            }

            if (error != null)
            {
                completeCallback?.Invoke(default(T));

                yield break;
            }

            while (handle.Status == AsyncOperationStatus.None)
            {
                progressionCallback?.Invoke(handle.PercentComplete);

                yield return null;
            }

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                failCallback?.Invoke();

                yield break;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                handle.Result.name = typeof(T).Name;

                if (typeof(T).Equals(typeof(GameObject)))
                {
                    completeCallback?.Invoke((T)Convert.ChangeType(handle.Result, typeof(T)));

                    yield break;
                }

                completeCallback?.Invoke(handle.Result.GetComponent<T>());

                yield break;
            }

            completeCallback?.Invoke(default(T));
        }

        public static async Task<T> LoadContent<T>(string file, Transform parent)
        {
            GameObject go = await Addressables.InstantiateAsync($"Assets/_Addressables/{file}", parent).Task;
            go.name = typeof(T).Name;

            if (typeof(T).Equals(typeof(GameObject)))
                return (T)Convert.ChangeType(go, typeof(T));

            return go.GetComponent<T>();
        }

        public static IEnumerator LoadContent<T>(string file, Transform parent, Action<T> completeCallback, Action failCallback, Action<float> progressionCallback = null)
        {
            AsyncOperationHandle handle = Addressables.InstantiateAsync($"Assets/_Addressables/{file}", parent);

            while (handle.Status == AsyncOperationStatus.None)
            {
                progressionCallback?.Invoke(handle.PercentComplete);

                yield return null;
            }

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                failCallback?.Invoke();

                yield break;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject go = (GameObject)handle.Result;
                go.name = typeof(T).Name;

                if (typeof(T).Equals(typeof(GameObject)))
                    completeCallback?.Invoke((T)Convert.ChangeType(go, typeof(T)));
                else
                    completeCallback?.Invoke(go.GetComponent<T>());

                yield break;
            }

            completeCallback?.Invoke(default(T));
        }

        public static async Task<T> LoadContent<T>(string file, Vector3 position, Quaternion rotatation)
        {
            GameObject go = await Addressables.InstantiateAsync($"Assets/_Addressables/{file}", position, rotatation).Task;
            go.name = typeof(T).Name;

            if (typeof(T).Equals(typeof(GameObject)))
                return (T)Convert.ChangeType(go, typeof(T));

            return go.GetComponent<T>();
        }

        public static IEnumerator LoadContent<T>(string file, Vector3 position, Quaternion rotatation, Action<T> completeCallback, Action failCallback, Action<float> progressionCallback = null)
        {
            AsyncOperationHandle handle = Addressables.InstantiateAsync($"Assets/_Addressables/{file}", position, rotatation);

            while (handle.Status == AsyncOperationStatus.None)
            {
                progressionCallback?.Invoke(handle.PercentComplete);

                yield return null;
            }

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                failCallback?.Invoke();

                yield break;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject go = (GameObject)handle.Result;
                go.name = typeof(T).Name;

                if (typeof(T).Equals(typeof(GameObject)))
                    completeCallback?.Invoke((T)Convert.ChangeType(go, typeof(T)));
                else
                    completeCallback?.Invoke(go.GetComponent<T>());

                yield break;
            }

            completeCallback?.Invoke(default(T));
        }

        public static async Task<T> LoadContent<T>(string file, Vector3 position, Quaternion rotatation, Transform parent)
        {
            GameObject go = await Addressables.InstantiateAsync($"Assets/_Addressables/{file}", position, rotatation, parent).Task;
            go.name = typeof(T).Name;

            if (typeof(T).Equals(typeof(GameObject)))
                return (T)Convert.ChangeType(go, typeof(T));

            return go.GetComponent<T>();
        }

        public static IEnumerator LoadContent<T>(string file, Vector3 position, Quaternion rotatation, Transform parent, Action<T> completeCallback, Action failCallback, Action<float> progressionCallback = null)
        {
            AsyncOperationHandle handle = Addressables.InstantiateAsync($"Assets/_Addressables/{file}", position, rotatation, parent);

            while (handle.Status == AsyncOperationStatus.None)
            {
                progressionCallback?.Invoke(handle.PercentComplete);

                yield return null;
            }

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                failCallback?.Invoke();

                yield break;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject go = (GameObject)handle.Result;
                go.name = typeof(T).Name;

                if (typeof(T).Equals(typeof(GameObject)))
                    completeCallback?.Invoke((T)Convert.ChangeType(go, typeof(T)));
                else
                    completeCallback?.Invoke(go.GetComponent<T>());

                yield break;
            }

            completeCallback?.Invoke(default(T));
        }

        public static async Task<SceneInstance> LoadScene(string file, bool single = false)
        {
            SceneInstance scene = await Addressables.LoadSceneAsync($"Assets/_Addressables/{file}", !single ? UnityEngine.SceneManagement.LoadSceneMode.Additive : UnityEngine.SceneManagement.LoadSceneMode.Single, false).Task;

            return scene;
        }

        public static IEnumerator LoadScene(string file, Action<SceneInstance> completeCallback, Action failCallback, Action<float> progressionCallback = null, bool single = false)
        {
            AsyncOperationHandle handle = Addressables.LoadSceneAsync($"Assets/_Addressables/{file}", !single ? UnityEngine.SceneManagement.LoadSceneMode.Additive : UnityEngine.SceneManagement.LoadSceneMode.Single, false);

            while (handle.Status == AsyncOperationStatus.None)
            {
                progressionCallback?.Invoke(handle.PercentComplete);

                yield return null;
            }

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                failCallback?.Invoke();

                yield break;
            }

            completeCallback?.Invoke((SceneInstance)handle.Result);
        }

        public static async Task UnloadScene(SceneInstance scene)
        {
            await Addressables.UnloadSceneAsync(scene, true).Task;
        }

        public static IEnumerator UnloadScene(SceneInstance scene, Action completeCallback)
        {
            if (scene.Scene.isLoaded)
            {
                AsyncOperationHandle handle = Addressables.UnloadSceneAsync(scene, true);

                yield return handle;
            }

            completeCallback?.Invoke();
        }

        public static IEnumerator LoadSprite(string path, Action<Sprite> completeCallback)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture($"file:///{path}");

            yield return request.SendWebRequest();

            if (HttpUtil.HasRequestError(request))
            {
                SystemUtil.Log(request.error, SystemUtil.LogType.Exception);

                completeCallback?.Invoke(null);

                yield break;
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            if (texture == null)
            {
                completeCallback?.Invoke(null);

                yield break;
            }

            try
            {
                Sprite sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), new Vector2(0.5f, 0.5f));

                completeCallback?.Invoke(sprite);
            }
            catch(Exception ex)
            {
                SystemUtil.Log(ex, SystemUtil.LogType.Exception);

                completeCallback?.Invoke(null);
            }
        }

        public static Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
        {
            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

            RenderTexture.active = renderTexture;

            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = null;

            return texture2D;
        }

        public static Texture2D RenderTextureToTexture2D(RenderTexture renderTexture, int width, int divisor)
        {
            float scale = (float)width / divisor / renderTexture.width;

            width = Mathf.FloorToInt(width * scale);

            int height = Mathf.FloorToInt(renderTexture.height * scale);

            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);

            RenderTexture.active = renderTexture;

            texture2D.ReadPixels(new Rect(renderTexture.width / 2f - width / 2f, renderTexture.height / - height / 2f, width, height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = null;

            return texture2D;
        }

        public static IEnumerator WaitInitializerManager()
        {
            yield return new WaitUntil(() => InitializerManager.InitializeComplete);
        }

        public static IEnumerator WaitRouterManager()
        {
            yield return new WaitUntil(() => !RouteManager.Loading);
        }

        public static IEnumerator WaitInitializeAndRouterManager()
        {
            yield return new WaitUntil(() => InitializerManager.InitializeComplete && !RouteManager.Loading);
        }
    }
}
