using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureCasting
{
    public class GestureCastData
    {
        public float[][] handPoses;
        public string[] spellIdAtPose;
        public bool waitForIntermediate;
        public float[] intermediaryPose;
        public bool castOnPose;
        public float cooldownBetweenCastTime;
        public float cooldownBetweenSwapTime;
        public float cooldownIntermediary;
    }
}
