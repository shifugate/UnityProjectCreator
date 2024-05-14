using System;
using UnityEngine;

namespace Assets._Scripts.Manager.Route.Transition.Base
{
    public class RouterManagerBaseTransition : MonoBehaviour
    {
        protected float progression;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public virtual void Initialize()
        {

        }

        public virtual void AnimationIn(Action callback)
        {
            
        }

        public virtual void AnimationOut(Action callback)
        {
            
        }

        public virtual void Progression(float progression)
        {
            this.progression = progression;
        }
    }
}
