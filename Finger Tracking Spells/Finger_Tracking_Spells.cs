using System;
using System.Collections;
using UnityEngine;
using ThunderRoad;
using System.Collections.Generic;

namespace Finger_Tracking_Spells
{
    public class Load : LevelModule
    {
        public float[][] handPoses;
        public string[] spellIdAtPose;
        public float cooldownTime;
        float timeOfSwap = Time.time;
        List<WheelMenu.Orb> list;
        WheelMenu icons;
        WheelMenu.Orb orb;

        float[] leftHand = new float[5];
        float[] rightHand = new float[5];
        public override IEnumerator OnLoadCoroutine(Level level)
        {
            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
            return base.OnLoadCoroutine(level);
        }

        private void EventManager_onCreatureSpawn(Creature creature)
        {
            if (creature.isPlayer)
            {
                Debug.Log("Finger Tracking Spells loaded");
                try
                {
                    foreach (string i in spellIdAtPose)
                    {
                        Debug.Log(i);
                    }
                    foreach (float[] i in handPoses)
                    {
                        string debugShit = "";
                        foreach (float j in i)
                        {
                            debugShit += " " + i.ToString();
                        }
                        Debug.Log(debugShit);
                    }
                    Debug.Log(cooldownTime);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }

        public override void Update(Level level)
        {
            base.Update(level);
            UpdateFingers();
            if (PlayerControl.controller == PlayerControl.Controller.Index)
            {
                Creature creature = Player.currentCreature;
                {
                    if (Time.time - timeOfSwap >= cooldownTime && orb != null)
                    {
                        DespawnOrb(orb);
                        orb = null;
                    }
                    for (int i = 0; i < handPoses.Length; i++)
                    {
                        if (CheckForPose(leftHand, handPoses[i]))
                        {
                            if (creature.handLeft.grabbedHandle == null && Time.time - timeOfSwap >= cooldownTime)
                            {
                                LoadSpell(creature.mana.casterLeft, Side.Left, GetSpell(spellIdAtPose[i]));
                                timeOfSwap = Time.time;
                            }
                        }
                        if (CheckForPose(rightHand, handPoses[i]))
                        {
                            if (creature.handRight.grabbedHandle == null && Time.time - timeOfSwap >= cooldownTime)
                            {
                                LoadSpell(creature.mana.casterRight, Side.Right, GetSpell(spellIdAtPose[i]));
                                timeOfSwap = Time.time;
                            }
                        }
                    }
                }
            }
        }

        bool CheckForPose(float[] hand, float[] pose)
        {
            for (int i = 0; i < hand.Length; i++)
            {
                if (pose[i] == 1f)
                {
                    if (hand[i] < 0.8f)
                    {
                        return false;
                    }
                }
                if (pose[i] == 0f)
                {
                    if (hand[i] > 0.8f)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        void LoadSpell(SpellCaster hand, Side side, CatalogData spell)
        {
            if (spell != null)
            {
                if (hand.spellInstance != null && hand.spellInstance.hashId == spell.hashId)
                {
                    return;
                }
                hand.LoadSpell((SpellCastData)spell);
                this.list = PopulateSpellMenu();
                SpawnOrb(GetSpellOrb(list, GetSpellIconEffect(spell.id)), Player.currentCreature.GetHand(side));
                PlayerControl.GetHand(side).HapticShort(1f);
                Debug.Log(spell.ToString());
            }
            else
            {
                Debug.LogError("Null SpellID! Did you forget to add an ID in the JSON?");
                return;
            }
        }

        CatalogData GetSpell(string spellID)
        {
            return Catalog.GetData(Catalog.Category.Spell, spellID);
        }

        EffectData GetSpellIconEffect(string spellID)
        {
            foreach (ContainerData.Content content in Player.currentCreature.container.contents)
            {
                ItemModuleSpell module = content.itemData.GetModule<ItemModuleSpell>();
                if (module != null && module.spellData is SpellCastData && content.itemData.categoryPath.Length == 0)
                {
                    if (module.spellId == spellID)
                    {
                        return module.itemData.iconEffectData;
                    }
                }
            }
            return null;
        }

        WheelMenu.Orb GetSpellOrb(List<WheelMenu.Orb> list, EffectData iconEffect)
        {
            if (iconEffect != null)
            {
                foreach (WheelMenu.Orb orb in list)
                {
                    if (orb.effectData == iconEffect)
                    {
                        return orb;
                    }
                }
            }
            return null;
        }

        void SpawnOrb(WheelMenu.Orb orb, RagdollHand hand)
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
            orb.effectInstance = orb.effectData.Spawn(orb.transform, true, Array.Empty<Type>());
            orb.effectInstance.Play(0);
            orb.effectInstance.SetIntensity(0f);
            orb.effectInstance.SetLayer(5);
            this.orb = orb;
        }

        void DespawnOrb(WheelMenu.Orb orb)
        {
            orb.effectInstance.End(false, -1f);
            orb.effectInstance.onEffectFinished += OnEffectFinish;
            UnityEngine.Object.Destroy(orb.transform.gameObject, 5f);
            list.Clear();
            List<Transform> debug = new List<Transform>();
            for (int i = 0; i < Player.currentCreature.handLeft.transform.childCount; i++)
            {
                debug.Add(Player.currentCreature.handLeft.transform.GetChild(i));
            }
            foreach (Transform i in debug)
            {
                Debug.Log(i);
            }
        }

        List<WheelMenu.Orb> PopulateSpellMenu()
        {
            List<WheelMenu.Orb> list = new List<WheelMenu.Orb>();
            foreach (ContainerData.Content content in Player.currentCreature.container.contents)
            {
                ItemModuleSpell module = content.itemData.GetModule<ItemModuleSpell>();
                if (module != null && module.spellData is SpellCastData && content.itemData.categoryPath.Length == 0)
                {
                    list.Add(new WheelMenu.Orb(content.itemData.iconEffectData, content, icons));
                    Debug.Log(content.itemData.iconEffectData);
                }
            }
            return list;
        }

        void OnEffectFinish(EffectInstance effectInstance)
        {
            this.orb.effectInstance.Despawn();
            this.orb.effectInstance.onEffectFinished -= OnEffectFinish;
        }

        void UpdateFingers()
        {
            rightHand[0] = PlayerControl.handRight.thumbCurl;
            rightHand[1] = PlayerControl.handRight.indexCurl;
            rightHand[2] = PlayerControl.handRight.middleCurl;
            rightHand[3] = PlayerControl.handRight.ringCurl;
            rightHand[4] = PlayerControl.handRight.littleCurl;
            leftHand[0] = PlayerControl.handLeft.thumbCurl;
            leftHand[1] = PlayerControl.handLeft.indexCurl;
            leftHand[2] = PlayerControl.handLeft.middleCurl;
            leftHand[3] = PlayerControl.handLeft.ringCurl;
            leftHand[4] = PlayerControl.handLeft.littleCurl;
        }
    }
}
