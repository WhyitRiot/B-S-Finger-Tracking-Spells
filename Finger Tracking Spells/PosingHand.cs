using System;
using ThunderRoad;
using UnityEngine;

namespace GestureCasting
{
    class PosingHand
    {
        float[] hand = new float[5];
        public Side side;
        public WheelMenu.Orb orb;
        public float timeOfSwap = Time.time;
        public float timeOfCast = Time.time;
        public SpellCastData currentSpell;
        public bool isCasting = false;
        public bool intermediateTriggered = false;
        public bool intermFirstPose = false;
        public PosingHand(Side side)
        {
            this.side = side;
            hand[0] = PlayerControl.GetHand(side).thumbCurl;
            hand[1] = PlayerControl.GetHand(side).indexCurl;
            hand[2] = PlayerControl.GetHand(side).middleCurl;
            hand[3] = PlayerControl.GetHand(side).ringCurl;
            hand[4] = PlayerControl.GetHand(side).littleCurl;
        }

        public void UpdateFingers()
        {
            hand[0] = PlayerControl.GetHand(this.side).thumbCurl;
            hand[1] = PlayerControl.GetHand(this.side).indexCurl;
            hand[2] = PlayerControl.GetHand(this.side).middleCurl;
            hand[3] = PlayerControl.GetHand(this.side).ringCurl;
            hand[4] = PlayerControl.GetHand(this.side).littleCurl;
        }

        public bool CheckForPose(float[] pose)
        {
            for (int i = 0; i < hand.Length; i++)
            {
                if (pose[i] == 1f)
                {
                    if (this.hand[i] < 0.8f)
                    {
                        return false;
                    }
                }
                if (pose[i] == 0f)
                {
                    if (this.hand[i] > 0.8f)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public float[] recordPose()
        {
            float[] newPose = new float[5];
            for (int i = 0; i < newPose.Length; i++)
            {
                if (this.hand[i] < 0.8f)
                {
                    newPose[i] = 0f;
                }
                else
                {
                    newPose[i] = 1f;
                }
            }
            return newPose;
        }

        public void SpawnOrb(WheelMenu.Orb orb, RagdollHand hand)
        {
            if (orb == null)
            {
                Debug.LogError("Null Spell Icon!");
                return;
            }
            orb.transform = new GameObject("SpellIcon").transform;
            orb.transform.SetParent(hand.transform, false);
            orb.transform.localPosition += new Vector3(-0.1f, 0.0f, 0.06f);
            orb.transform.localEulerAngles = new Vector3(180f, orb.transform.localEulerAngles.y, 90f);
            orb.effectInstance = orb.effectData.Spawn(orb.transform, true, null, false, Array.Empty<Type>());
            orb.effectInstance.Play(0);
            orb.effectInstance.SetIntensity(0f);
            orb.effectInstance.SetLayer(5);
            this.orb = orb;
        }

        public void DespawnOrb(WheelMenu.Orb orb)
        {
            orb.effectInstance.End(false, -1f);
            UnityEngine.Object.Destroy(orb.transform.gameObject, 5f);
            this.orb = null;
        }
    }
}
