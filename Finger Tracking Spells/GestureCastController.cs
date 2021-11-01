using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GestureCasting
{
    public class GestureCastController : MonoBehaviour
    {
        public GestureCastData data = new GestureCastData();
        public delegate void ControllerCreatedEventHandler(object source, EventArgs e);
        public event ControllerCreatedEventHandler ControllerCreated;
        public void Start()
        {
            OnControllerCreated();
        }
        protected virtual void OnControllerCreated()
        {
            if (ControllerCreated != null)
            {
                ControllerCreated(this, EventArgs.Empty);
            }
        }
    }
}
