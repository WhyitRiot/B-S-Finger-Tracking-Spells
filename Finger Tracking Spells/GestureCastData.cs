using System.Linq;

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
        
        public float[][] RecompilePoses(float[] newPose)
        {
            float[][] newHandPoses = new float[this.handPoses.Length + 1][];
            for (int i = 0; i < handPoses.Length; i++)
            {
                if (Enumerable.SequenceEqual(handPoses[i], newPose) || waitForIntermediate && Enumerable.SequenceEqual(intermediaryPose, newPose))
                {
                    return null;
                }
                else
                {
                    newHandPoses[i] = handPoses[i];
                }
            }
            newHandPoses[this.handPoses.Length] = newPose;
            return newHandPoses;
        }
        public string[] ChangeSpellIdSize()
        {
            string[] spellIds = new string[this.handPoses.Length];
            for (int i = 0; i < this.spellIdAtPose.Length; i++)
            {
                if (this.spellIdAtPose[i] != null)
                {
                    spellIds[i] = this.spellIdAtPose[i];
                }
                else
                {
                    spellIds[i] = null;
                }
            }
            return spellIds;
        }
        public float[][] DeletePose(int index)
        {
            float[][] newHandPoses = new float[this.handPoses.Length - 1][];
            bool reachedIndex = false;
            for (int i = 0; i < newHandPoses.Length; i++)
            {
                if (i == index)
                {
                    reachedIndex = true;
                }
                if (reachedIndex)
                {
                    newHandPoses[i] = this.handPoses[i + 1];
                }
                else
                {
                    newHandPoses[i] = this.handPoses[i];
                }
            }
            return newHandPoses;
        }
        public string[] DeleteSpellId(int index)
        {
            string[] newSpellIds = new string[this.spellIdAtPose.Length - 1];
            bool reachedIndex = false;
            for (int i = 0; i < newSpellIds.Length; i++)
            {
                if (i == index)
                {
                    reachedIndex = true;
                }
                if (reachedIndex)
                {
                    newSpellIds[i] = this.spellIdAtPose[i + 1];
                }
                else
                {
                    newSpellIds[i] = this.spellIdAtPose[i];
                }
            }
            return newSpellIds;
        }
      
        public string[] ChangeSpellIndex(string spell, int index)
        {
            string[] newSpellIds = new string[this.spellIdAtPose.Length];
            newSpellIds[index] = spell;
            for (int i = 0; i < handPoses.Length; i++)
            {
                if (this.spellIdAtPose[i] != null && i != index)
                {
                    newSpellIds[i] = this.spellIdAtPose[i];
                }
                else if (i != index)
                {
                    newSpellIds[i] = null;
                }
            }
            return newSpellIds;
        }
        public string HandPoseToString(float[] handPose)
        {
            string pose = "[";
            for (int i = 0; i < handPose.Length; i++)
            {
                if (i == 4)
                {
                    pose += handPose[i].ToString();
                }
                else
                {
                    pose += handPose[i].ToString() + ", ";
                }
            }
            pose += "]";
            return pose;
        }
    }
}
