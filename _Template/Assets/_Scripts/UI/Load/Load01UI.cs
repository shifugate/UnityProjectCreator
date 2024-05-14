using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.UI.Load
{
    public class Load01UI : MonoBehaviour
    {
        [SerializeField]
        private Image loadIcon;

        private void OnEnable()
        {
            StartCoroutine(LoadingCR());
        }

        private IEnumerator LoadingCR()
        {
            Vector3 rot = new Vector3(0, 0, 0);

            while (true)
            {
                rot -= Vector3.forward * 60 * Time.deltaTime;

                loadIcon.transform.rotation = Quaternion.Euler(rot);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
