using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ThunderRoad;
using System.Linq;

namespace GestureCasting
{
    public class GestureCastMenu : MenuModule
    {
        private GameObject leftPage1;
        private GameObject leftPage2;
        private GameObject leftPage3;
        private GameObject leftPage4;
        private GameObject rightPage1;
        private GameObject rightPage2;
        private GameObject rightPage3;
        private UnityEngine.UI.Slider EquipSlider;
        private UnityEngine.UI.Slider CastSlider;
        private UnityEngine.UI.Slider IntermSlider;
        private Text EquipSliderLabel;
        private Text CastSliderLabel;
        private Text IntermSliderLabel;
        private Text leftPage3Countdown;
        private Text leftPage3Pose;
        private Text messageL;
        private Text messageR;
        private Text leftPage4Pose;
        private Toggle CastOnPoseToggle;
        private Toggle WaitForIntermediaryToggle;
        private Button EquipSliderRight;
        private Button EquipSliderLeft;
        private Button CastSliderRight;
        private Button CastSliderLeft;
        private Button IntermSliderRight;
        private Button IntermSliderLeft;
        private Button back;
        private Button assign;
        private Button forward;
        private Button newPose;
        private Button deletePose;
        private Button setInterm;
        private Button page4Back;
        private GameObject poseContent;
        private GameObject spellContent;
        private GameObject rightPage3Content;
        private bool CastOnPose;
        private bool WaitForIntermediary;
        public GestureCastController controller;
        GameObject gcButtonL;
        GameObject gcButtonR;
        GameObject gcButton3;
        List<Button> poses = new List<Button>();
        List<Button> spellIds = new List<Button>();
        // Configure pose data
        int spellIndex;
        float[] handPose;
        public override void Init(MenuData menuData, Menu menu)
        {
            base.Init(menuData, menu);
            //Sliders, Toggle, Text
            EquipSlider = menu.GetCustomReference("EquipSlider").GetComponent<UnityEngine.UI.Slider>();
            CastSlider = menu.GetCustomReference("CastSlider").GetComponent<UnityEngine.UI.Slider>();
            IntermSlider = menu.GetCustomReference("IntermSlider").GetComponent<UnityEngine.UI.Slider>();
            EquipSliderLabel = menu.GetCustomReference("EquipSliderLabel").GetComponent<Text>();
            CastSliderLabel = menu.GetCustomReference("CastSliderLabel").GetComponent<Text>();
            IntermSliderLabel = menu.GetCustomReference("IntermSliderLabel").GetComponent<Text>();
            CastOnPoseToggle = menu.GetCustomReference("CastOnPoseToggle").GetComponent<Toggle>();
            WaitForIntermediaryToggle = menu.GetCustomReference("WaitForIntermediaryToggle").GetComponent<Toggle>();
            leftPage3Countdown = menu.GetCustomReference("LeftPage3Countdown").GetComponent<Text>();
            leftPage3Pose = menu.GetCustomReference("LeftPage3Pose").GetComponent<Text>();
            messageL = menu.GetCustomReference("MessageL").GetComponent<Text>();
            messageR = menu.GetCustomReference("MessageR").GetComponent<Text>();
            leftPage4Pose = menu.GetCustomReference("LeftPage4Pose").GetComponent<Text>();
            //Buttons
            gcButtonL = menu.GetCustomReference("buttonPrefLeft").gameObject;
            gcButtonR = menu.GetCustomReference("buttonPrefRight").gameObject;
            gcButton3 = menu.GetCustomReference("buttonPref3").gameObject;
            EquipSliderRight = menu.GetCustomReference("EquipSliderRight").GetComponent<Button>();
            EquipSliderLeft = menu.GetCustomReference("EquipSliderLeft").GetComponent<Button>();
            CastSliderRight = menu.GetCustomReference("CastSliderRight").GetComponent<Button>();
            CastSliderLeft = menu.GetCustomReference("CastSliderLeft").GetComponent<Button>();
            IntermSliderLeft = menu.GetCustomReference("IntermSliderRight").GetComponent<Button>();
            IntermSliderRight = menu.GetCustomReference("IntermSliderLeft").GetComponent<Button>();
            back = menu.GetCustomReference("buttonBack").GetComponent<Button>();
            assign = menu.GetCustomReference("buttonAssign").GetComponent<Button>();
            forward = menu.GetCustomReference("buttonPoseMenu").GetComponent<Button>();
            newPose = menu.GetCustomReference("buttonNewPose").GetComponent<Button>();
            deletePose = menu.GetCustomReference("btnDelete").GetComponent<Button>();
            setInterm = menu.GetCustomReference("btnSetIterm").GetComponent<Button>();
            page4Back = menu.GetCustomReference("LeftPage4Back").GetComponent<Button>();
            //Pages
            leftPage1 = menu.GetCustomReference("LeftPage1").gameObject;
            leftPage2 = menu.GetCustomReference("LeftPage2").gameObject;
            leftPage3 = menu.GetCustomReference("LeftPage3").gameObject;
            leftPage4 = menu.GetCustomReference("LeftPage4").gameObject;
            rightPage1 = menu.GetCustomReference("RightPage1").gameObject;
            rightPage2 = menu.GetCustomReference("RightPage2").gameObject;
            rightPage3 = menu.GetCustomReference("RightPage3").gameObject;
            //Scroll View content
            poseContent = menu.GetCustomReference("LeftPage2Content").gameObject;
            spellContent = menu.GetCustomReference("RightPage2Content").gameObject;
            rightPage3Content = menu.GetCustomReference("RightPage3Content").gameObject;
            //Slider Listeners
            EquipSlider.onValueChanged.AddListener(delegate { UpdateEquipText(); });
            CastSlider.onValueChanged.AddListener(delegate { UpdateCastText(); });
            IntermSlider.onValueChanged.AddListener(delegate { UpdateIntermText(); });
            CastOnPoseToggle.onValueChanged.AddListener(delegate { ToggleCastOnPose(); });
            WaitForIntermediaryToggle.onValueChanged.AddListener(delegate { ToggleWaitForIterm(); });
            //Button Listeners
            EquipSliderRight.onClick.AddListener(delegate { eSliderRight(); });
            EquipSliderLeft.onClick.AddListener(delegate { eSliderLeft(); });
            CastSliderRight.onClick.AddListener(delegate { cSliderRight(); });
            CastSliderLeft.onClick.AddListener(delegate { cSliderLeft(); });
            IntermSliderRight.onClick.AddListener(delegate { iSliderLeft(); });
            IntermSliderLeft.onClick.AddListener(delegate { iSliderRight(); });
            back.onClick.AddListener(delegate { PreviousPage(); });
            forward.onClick.AddListener(delegate { NextPage(); });
            assign.onClick.AddListener(delegate { AssignBtnClick(); });
            newPose.onClick.AddListener(delegate { NewPoseBtn(); });
            deletePose.onClick.AddListener(delegate { BtnDeletePose(); });
            setInterm.onClick.AddListener(delegate { BtnSetInterm(); });
            page4Back.onClick.AddListener(delegate { BtnPoseBack(); });
            //Data link
            controller = GameManager.local.gameObject.AddComponent<GestureCastController>();
            controller.Deserialize();
            loadButtons(controller.data.spellIdAtPose, controller.data.handPoses);
            UpdateMenuData();
            EventManager.onCreatureSpawn += LoadSpellButtons;
            Debug.Log("GestureCastMenu loaded");
        }
        public void NewPoseBtn()
        {
            messageR.text = "";
            leftPage3.SetActive(true);
            leftPage2.SetActive(false);
            rightPage2.SetActive(false);
            controller.StartCoroutine(WaitForNewPose());
        }
        public void AssignEmptyBtn(GameObject btn)
        {
            controller.data.spellIdAtPose = controller.data.ChangeSpellIndex(btn.GetComponent<GestureCastBtnData>().spellId, this.spellIndex);
            ReloadButtons();
            BtnReturnSpellId();
        }
        public void LoadSpellButtons(Creature creature)
        {
            if (creature.isPlayer)
            {
                CreateButton(-1, rightPage3Content.transform, gcButton3, "Empty");
                foreach (ContainerData.Content content in Player.currentCreature.container.contents)
                {
                    ItemModuleSpell module = content.itemData.GetModule<ItemModuleSpell>();
                    if (module != null && module.spellData is SpellCastData && content.itemData.categoryPath.Length == 0)
                    {
                        CreateButton(-1, rightPage3Content.transform, gcButton3, module.spellId);
                    }
                }
            }
            EventManager.onCreatureSpawn -= LoadSpellButtons;
        }
        public void OnBtnClick(GameObject btn)
        {
            GestureCastBtnData data = btn.GetComponent<GestureCastBtnData>();
            if (data.btnType == 0)
            {
                if (data.spellIndex != -1)
                {
                    this.spellIndex = data.spellIndex;
                    this.handPose = data.handPose;
                    BtnPoseOptions(btn);
                }
                else
                {
                    messageL.text = "";
                }
            }
            else
            {
                this.spellIndex = data.spellIndex;
                BtnGetSpellId();
            }
        }
        public void ReloadButtons()
        {
            for (int i = 0; i < poses.Count; i++)
            {
                if (i != 0)
                {
                    GameObject.Destroy(poses[i].gameObject);
                }
                gcButtonL = poses[0].gameObject;
                gcButtonL.SetActive(false);
            }
            poses.Clear();
            for (int i = 0; i < spellIds.Count; i++)
            {
                if (i != 0)
                {
                    GameObject.Destroy(spellIds[i].gameObject);
                }
                gcButtonR = spellIds[0].gameObject;
                gcButtonR.SetActive(false);
            }
            spellIds.Clear();
            loadButtons(controller.data.spellIdAtPose, controller.data.handPoses);
        }

        public void loadButtons(string[] spellIds, float[][] handPoses)
        {
            for (int i = 0; i < handPoses.Length; i++)
            {
                this.poses.Add(CreateButton(0, poseContent.transform, gcButtonL, controller.data.HandPoseToString(handPoses[i]), handPoses[i], i));
            }
            if (controller.data.waitForIntermediate)
            {
                this.poses.Add(CreateButton(0, poseContent.transform, gcButtonL, "Intermediate:\n" + controller.data.HandPoseToString(controller.data.intermediaryPose), controller.data.intermediaryPose));
            }
            for (int i = 0; i < spellIds.Length; i++)
            {
                if (spellIds[i] == null)
                {
                    this.spellIds.Add(CreateButton(1, spellContent.transform, gcButtonR, "Empty", index: i));
                }
                else
                {
                    this.spellIds.Add(CreateButton(1, spellContent.transform, gcButtonR, spellIds[i], index: i));
                }
            }
            foreach (Button i in poses)
            {
                Debug.Log(i.gameObject.GetComponentInChildren<Text>().text);
            }
            foreach (Button i in this.spellIds)
            {
                Debug.Log(i.gameObject.GetComponentInChildren<Text>().text + ", " + i.gameObject.GetComponent<GestureCastBtnData>().spellIndex);
            }
        }
        public Button CreateButton(int type, Transform parent, GameObject father, string name = null, float[] pose = null, int index = -1)
        {
            GameObject button;
            if (father.activeSelf)
            {
                button = GameObject.Instantiate<GameObject>(father) as GameObject;
                father = button;
            }
            else
            {
                button = father;
            }
            button = father;
            button.SetActive(true);
            button.transform.SetParent(parent, false);
            button.transform.localPosition = new Vector3(0f, -40.0f, 0f);
            button.GetComponentInChildren<Text>().text = name;
            button.AddComponent<GestureCastBtnData>();
            GestureCastBtnData data = button.GetComponent<GestureCastBtnData>();
            if (name.Equals("Empty"))
            {
                name = null;
            }
            data.spellId = name;
            data.handPose = pose;
            data.spellIndex = index;
            button.GetComponent<GestureCastBtnData>().btnType = type;
            Button newButton = button.GetComponent<Button>();
            if (type == 0 || type == 1)
            {
                newButton.onClick.AddListener(delegate { OnBtnClick(button); });
            }
            else
            {
                newButton.onClick.AddListener(delegate { AssignEmptyBtn(button); });
            }
            return newButton;
        }
        public float[] NewHandPose()
        {
            float[] pose = new float[5];
            pose[0] = PlayerControl.GetHand(Side.Right).thumbCurl;
            pose[1] = PlayerControl.GetHand(Side.Right).indexCurl;
            pose[2] = PlayerControl.GetHand(Side.Right).middleCurl;
            pose[3] = PlayerControl.GetHand(Side.Right).ringCurl;
            pose[4] = PlayerControl.GetHand(Side.Right).littleCurl;
            for (int i = 0; i < pose.Length; i++)
            {
                if (pose[i] > 0.8)
                {
                    pose[i] = 1f;
                }
                else
                {
                    pose[i] = 0f;
                }
            }
            return pose;
        }
        IEnumerator WaitForNewPose()
        {
            for (int i = 0; i < 4; i++)
            {
                this.handPose = NewHandPose();
                yield return new WaitForSeconds(0.5f);
                this.handPose = NewHandPose();
                yield return new WaitForSeconds(0.5f);
                leftPage3Countdown.text = "Time: " + (3 - i);
                leftPage3Pose.text = controller.data.HandPoseToString(this.handPose);
            }
            float[][] newPoseArray = controller.data.RecompilePoses(this.handPose);
            if (newPoseArray != null)
            {
                controller.data.handPoses = newPoseArray;
                controller.data.spellIdAtPose = controller.data.ChangeSpellIdSize();
            }
            else
            {
                messageL.text = "Hand pose already exists!";
            }
            ReloadButtons();
            leftPage2.SetActive(true);
            rightPage2.SetActive(true);
            leftPage3.SetActive(false);
        }
        public void UpdateMenuData()
        {
            EquipSlider.value = controller.data.cooldownBetweenSwapTime;
            CastSlider.value = controller.data.cooldownBetweenCastTime;
            IntermSlider.value = controller.data.cooldownIntermediary;
            CastOnPoseToggle.isOn = controller.data.castOnPose;
            WaitForIntermediaryToggle.isOn = controller.data.waitForIntermediate;
        }
        public void BtnPoseOptions(GameObject btn)
        {
            messageL.text = "";
            leftPage4.SetActive(true);
            leftPage2.SetActive(false);
            rightPage2.SetActive(false);
            this.spellIndex = btn.GetComponent<GestureCastBtnData>().spellIndex;
            leftPage4Pose.text = controller.data.HandPoseToString(this.handPose);
        }
        public void BtnDeletePose()
        {
            controller.data.handPoses = controller.data.DeletePose(this.spellIndex);
            controller.data.spellIdAtPose = controller.data.DeleteSpellId(this.spellIndex);
            ReloadButtons();
            leftPage2.SetActive(true);
            rightPage2.SetActive(true);
            leftPage4.SetActive(false);
        }
        public void BtnPoseBack()
        {
            leftPage2.SetActive(true);
            rightPage2.SetActive(true);
            leftPage4.SetActive(false);
        }
        public void BtnSetInterm()
        {
            if (controller.data.waitForIntermediate)
            {
                controller.data.intermediaryPose = this.handPose;
                controller.data.handPoses = controller.data.DeletePose(this.spellIndex);
                controller.data.spellIdAtPose = controller.data.DeleteSpellId(this.spellIndex);
                ReloadButtons();
            }
            else
            {
                messageL.text = "Wait for Intermediate is off!";
            }
            leftPage2.SetActive(true);
            rightPage2.SetActive(true);
            leftPage4.SetActive(false);
        }
        public void BtnGetSpellId()
        {
            rightPage3.SetActive(true);
            rightPage2.SetActive(false);
        }
        public void BtnReturnSpellId()
        {
            rightPage2.SetActive(true);
            rightPage3.SetActive(false);
        }

        public void AssignBtnClick()
        {
            messageL.text = "";
            messageR.text = "Changes saved to Settings.json";
            controller.Serialize(controller.data);
        }
        public void eSliderRight()
        {
            if (EquipSlider.value != EquipSlider.maxValue)
            {
                EquipSlider.value += 0.01f;
                UpdateEquipText();
            }
        }
        public void eSliderLeft()
        {
            if (EquipSlider.value != EquipSlider.minValue)
            {
                EquipSlider.value -= 0.01f;
                UpdateEquipText();
            }
        }
        public void cSliderRight()
        {
            if (CastSlider.value != CastSlider.maxValue)
            {
                CastSlider.value += 0.01f;
                UpdateCastText();
            }
        }
        public void cSliderLeft()
        {
            if (CastSlider.value != CastSlider.minValue)
            {
                CastSlider.value -= 0.01f;
                UpdateCastText();
            }
        }
        public void iSliderRight()
        {
            if (IntermSlider.value != IntermSlider.maxValue)
            {
                IntermSlider.value += 0.01f;
                UpdateIntermText();
            }
        }
        public void iSliderLeft()
        {
            if (IntermSlider.value != IntermSlider.minValue)
            {
                IntermSlider.value -= 0.01f;
                UpdateIntermText();
            }
        }
        public void ToggleCastOnPose()
        {
            CastOnPose = CastOnPoseToggle.isOn;
            controller.data.castOnPose = CastOnPose;
        }
        public void ToggleWaitForIterm()
        {
            WaitForIntermediary = WaitForIntermediaryToggle.isOn;
            controller.data.waitForIntermediate = WaitForIntermediary;
        }
        public void NextPage()
        {
            if (controller.data.waitForIntermediate)
            {
                for (int i = 0; i < controller.data.handPoses.Length; i++)
                {
                    if (Enumerable.SequenceEqual(controller.data.handPoses[i], controller.data.intermediaryPose))
                    {
                        controller.data.handPoses = controller.data.DeletePose(i);
                        controller.data.spellIdAtPose = controller.data.DeleteSpellId(i);
                    }
                }
            }
            ReloadButtons();
            messageL.text = "";
            messageR.text = "";
            leftPage2.SetActive(true);
            rightPage2.SetActive(true);
            leftPage1.SetActive(false);
            rightPage1.SetActive(false);
        }
        public void PreviousPage()
        {
            messageL.text = "";
            messageR.text = "";
            leftPage1.SetActive(true);
            rightPage1.SetActive(true);
            leftPage2.SetActive(false);
            rightPage2.SetActive(false);
        }
        public void UpdateEquipText()
        {
            EquipSliderLabel.text = EquipSlider.value.ToString("0.00");
            controller.data.cooldownBetweenSwapTime = (float) Math.Round(EquipSlider.value, 2);

        }
        public void UpdateCastText()
        {
            CastSliderLabel.text = CastSlider.value.ToString("0.00");
            controller.data.cooldownBetweenSwapTime = (float) Math.Round(CastSlider.value, 2);
        }
        public void UpdateIntermText()
        {
            IntermSliderLabel.text = IntermSlider.value.ToString("0.00");
            controller.data.cooldownIntermediary = (float) Math.Round(IntermSlider.value, 2);
        }
    }
}
