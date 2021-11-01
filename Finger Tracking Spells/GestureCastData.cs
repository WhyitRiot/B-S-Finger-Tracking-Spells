using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureCasting
{
    public class GestureCastData
    {
        public bool levelLoaded = false;
        public bool menuLoaded = false;
        public bool castOnPose;
        public bool waitForIntermediate;
        public float cooldownIntermediary;
        public float cooldownBetweenCastTime;
        public float cooldownBetweenSwapTime;
    }
}
