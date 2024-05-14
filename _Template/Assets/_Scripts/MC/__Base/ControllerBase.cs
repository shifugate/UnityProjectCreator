using UnityEngine;

namespace Assets._Scripts.MC.__Base
{
    public class ControllerBase<TModel> : MonoBehaviour
    {
        private TModel model;
        public TModel Model
        {
            get
            {
                if (model == null)
                    model = GetComponent<TModel>();

                return model;
            }
        }
    }
}
