using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Assets._Scripts.Manager.Route.Transition.Base;
using Assets._Scripts.MC.__Base;
using Assets._Scripts.Util;

namespace Assets._Scripts.Manager.Route
{
    public class RouteManager : MonoBehaviour
    {
        #region Singleton
        private static RouteManager instance;
        public static RouteManager Instance { get { return instance; } }

        public static IEnumerator InstanceCR(InitializerManager manager)
        {
            if (instance == null)
            {
                GameObject go = new GameObject("RouteManager");

                instance = go.AddComponent<RouteManager>();
            }

            yield return instance.InitializeCR(manager);
        }
        #endregion

        public enum Routes
        {
            SplashScene,
            HomeScene
        }

        private static bool loading;
        public static bool Loading { get { return loading; } }

        private bool initialized;
        private object[] args;
        private RouterManagerBaseTransition transition;

        private ModelBase modelBase;
        public ModelBase ModelBase { get { return modelBase; } }

        private IEnumerator InitializeCR(InitializerManager manager)
        {
            transform.SetParent(manager.transform);

            initialized = false;

            SetProperties();

            yield return new WaitUntil(() => initialized);
        }

        private void SetProperties()
        {
            LoadScene(Routes.SplashScene);
        }

        public void LoadScene(Routes route)
        {
            LoadScene(route, null);
        }

        public async void LoadScene(Routes route, object[] args)
        {
            if (loading)
                return;

            string scene = $"Scene/{route}";

            if (route.ToString() == SceneManager.GetActiveScene().name)
            {
                initialized = true;

                return;
            }

            loading = true;

            this.args = args;

            modelBase = FindAnyObjectByType<ModelBase>(FindObjectsInactive.Include);

            transition = await ContentUtil.LoadContent<RouterManagerBaseTransition>($"Transition/RouterManagerFadeTransition.prefab", null);
            transition.Initialize();
            transition.AnimationIn(() => LoadSceneAsync(scene));
        }

        private async void LoadSceneAsync(string route)
        {
            bool complete = false;

            SceneInstance scene = default(SceneInstance);

            float progression = 0;

            StartCoroutine(LoadSceneAsyncCR(route, (SceneInstance result) => 
            {
                complete = true;
                scene = result;
            },
            () => complete = true,
            (float progress) =>
            {
                progression = progress;

                transition.Progression(progression);
            }));

            while (!complete)
                await Task.Yield();

            if (progression > 0)
                transition.Progression(1);

            transition.AnimationOut(() =>
            {
                loading = false;
                initialized = true;
            });
        }

        private IEnumerator LoadSceneAsyncCR(string route, Action<SceneInstance> completeCallback, Action failCallback, Action<float> progressionCallback)
        {
            SceneInstance loadedScene = default(SceneInstance);

            yield return ContentUtil.LoadScene($"{route}.unity", (SceneInstance scene) => loadedScene = scene, failCallback, progressionCallback, true);
            yield return loadedScene.ActivateAsync();

            modelBase = FindAnyObjectByType<ModelBase>(FindObjectsInactive.Include);
            modelBase.args = args;

            completeCallback(loadedScene);
        }
    }
}
