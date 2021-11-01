using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;

namespace GestureCasting
{
    public class Load : LevelModule
    {
        public float[][] handPoses;
        public string[] spellIdAtPose;
        public bool waitForIntermediate;
        public float[] intermediaryPose;
        public float cooldownBetweenSwapTime;
        public float cooldownBetweenCastTime;
        public float cooldownIntermidiary;
        public bool castOnPose;
        private float intermTime = Time.time;
        GestureCastController controller;
        PosingHand[] hands = new PosingHand[] { new PosingHand(Side.Left), new PosingHand(Side.Right) };
        List<WheelMenu.Orb> list;
        Creature creature;
        private bool loaded = false;

        public override IEnumerator OnLoadCoroutine()
        {
            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
            //EventManager.onLevelLoad += OnLevelLoaded;
            yield break;
        }

        private void EventManager_onCreatureSpawn(Creature creature)
        {
            if (creature.isPlayer)
            {
                Debug.Log("Finger Tracking Spells loaded");
                Debug.Log("CastOnPose: " + castOnPose);
                Debug.Log("WaitForIntermediary: " + waitForIntermediate);
                try
                {
                    foreach (string i in spellIdAtPose)
                    {
                        Debug.Log(i);
                    }
                    Debug.Log("Hand Poses loaded: " + handPoses.Length);
                    Debug.Log("Swap Cooldown: " + cooldownBetweenSwapTime);
                    Debug.Log("Cast Cooldown: " + cooldownBetweenCastTime);
                    Debug.Log("Intermediary Cooldown: " + cooldownIntermidiary);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                this.creature = Player.currentCreature;
                this.controller = GameManager.local.gameObject.GetComponent<GestureCastController>();
                this.handPoses = controller.data.handPoses;
                this.intermediaryPose = controller.data.intermediaryPose;
                this.spellIdAtPose = controller.data.spellIdAtPose;
                loaded = true;
            }
        }

        public override void Update()
        {
            if (loaded)
            {
                UpdateData();
                foreach (PosingHand hand in hands)
                {
                    hand.UpdateFingers();
                    if (hand.orb != null && Time.time - hand.timeOfSwap >= cooldownBetweenSwapTime)
                    {
                        hand.DespawnOrb(hand.orb);
                    }
                    if (waitForIntermediate)
                    {
                        if (Time.time - intermTime >= cooldownIntermidiary)
                        {
                            if (!creature.mana.GetCaster(hand.side).isFiring)
                            {
                                hand.intermediateTriggered = false;
                            }
                        }
                        if (hand.CheckForPose(intermediaryPose))
                        {
                            if (!hand.intermFirstPose)
                            {
                                intermTime = Time.time;
                                hand.intermediateTriggered = true;
                                hand.intermFirstPose = true;
                            }
                        }
                        if (Time.time - hand.timeOfSwap >= cooldownBetweenSwapTime && !hand.CheckForPose(intermediaryPose))
                        {
                            hand.intermFirstPose = false;
                        }
                        if (hand.intermediateTriggered)
                        {
                            PoseCast(hand, creature);
                        }
                    }
                    else
                    {
                        PoseCast(hand, creature);
                    }
                }
            }
        }

        public void OnLevelLoaded(LevelData leveldata, EventTime eventTime)
        {
            if (this.controller != null)
            {
                this.controller.Serialize(this.controller.data);
            }
        }

        public void DebugSerialize()
        {
            this.controller.data.handPoses = handPoses;
            this.controller.data.intermediaryPose = intermediaryPose;
            this.controller.data.spellIdAtPose = spellIdAtPose;
            this.controller.data.castOnPose = castOnPose;
            this.controller.data.waitForIntermediate = waitForIntermediate;
            this.controller.data.cooldownBetweenCastTime = cooldownBetweenCastTime;
            this.controller.data.cooldownBetweenSwapTime = cooldownBetweenSwapTime;
            this.controller.data.cooldownIntermediary = cooldownIntermidiary;
            this.controller.Serialize(this.controller.data);
        }

        void UpdateData()
        {
            this.castOnPose = controller.data.castOnPose;
            this.waitForIntermediate = controller.data.waitForIntermediate;
            this.cooldownBetweenSwapTime = controller.data.cooldownBetweenSwapTime;
            this.cooldownBetweenCastTime = controller.data.cooldownBetweenCastTime;
            this.cooldownIntermidiary = controller.data.cooldownIntermediary;
        }

        void PoseCast(PosingHand hand, Creature creature)
        {
            for (int i = 0; i < handPoses.Length; i++)
            {
                if (hand.CheckForPose(handPoses[i]))
                {
                    if (!creature.mana.GetCaster(hand.side).isFiring && Time.time - hand.timeOfCast >= cooldownBetweenCastTime)
                    {
                        if (creature.GetHand(hand.side).grabbedHandle == null && Time.time - hand.timeOfSwap >= cooldownBetweenSwapTime)
                        {
                            LoadSpell(hand, creature.mana.GetCaster(hand.side), hand.side, GetSpell(spellIdAtPose[i]));
                            hand.timeOfSwap = Time.time;
                        }
                        if (castOnPose && !creature.mana.GetCaster(hand.side).isFiring)
                        {
                            hand.isCasting = true;
                            PlayerControl.GetHand(hand.side).gripPressed = false;
                            creature.mana.GetCaster(hand.side).Fire(true);
                            creature.mana.GetCaster(hand.side).FireAxis(1f);
                        }
                    }
                    else
                    {
                        if (!creature.mana.GetCaster(hand.side).isFiring)
                        {
                            hand.timeOfCast = Time.time;
                            hand.intermediateTriggered = false;
                        }
                    }
                }
                else if (castOnPose && creature.mana.GetCaster(hand.side).isFiring)
                {
                    if (hand.currentSpell.hashId == GetSpell(spellIdAtPose[i]).hashId)
                    {
                        creature.mana.GetCaster(hand.side).Fire(false);
                        creature.mana.GetCaster(hand.side).FireAxis(0f);
                        hand.isCasting = false;
                        hand.intermediateTriggered = false;
                    }
                }
            }
        }

        void LoadSpell(PosingHand hand, SpellCaster caster, Side side, CatalogData spell)
        {
            if (spell != null)
            {
                if (caster.spellInstance != null && caster.spellInstance.hashId == spell.hashId)
                {
                    return;
                }
                this.list = PopulateSpellMenu();
                caster.LoadSpell((SpellCastData)spell, new SpellData.Level(GetSpellOrb(list, GetSpellIconEffect(spell.id)).linkedObject as ContainerData.Content));
                hand.currentSpell = (SpellCastData)spell;
                hand.SpawnOrb(GetSpellOrb(list, GetSpellIconEffect(spell.id)), Player.currentCreature.GetHand(side));
                PlayerControl.GetHand(side).HapticShort(1f);
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

        List<WheelMenu.Orb> PopulateSpellMenu()
        {
            List<WheelMenu.Orb> list = new List<WheelMenu.Orb>();
            foreach (ContainerData.Content content in Player.currentCreature.container.contents)
            {
                ItemModuleSpell module = content.itemData.GetModule<ItemModuleSpell>();
                if (module != null && module.spellData is SpellCastData && content.itemData.categoryPath.Length == 0)
                {
                    list.Add(new WheelMenu.Orb(content.itemData.iconEffectData, content, null));
                }
            }
            return list;
        }
    }
}
