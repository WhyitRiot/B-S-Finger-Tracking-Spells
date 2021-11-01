using UnityEngine;
using UnityEngine.UI;
using ThunderRoad;
using System;
using System.Collections;

namespace GestureCasting
{
    public class GestureCastMenu : MenuModule
    {
        private UnityEngine.UI.Slider EquipSlider;
        private UnityEngine.UI.Slider CastSlider;
        private UnityEngine.UI.Slider IntermSlider;
        private Text EquipSliderLabel;
        private Text CastSliderLabel;
        private Text IntermSliderLabel;
        private Toggle CastOnPoseToggle;
        private Toggle WaitForIntermediaryToggle;
        private bool CastOnPose;
        private bool WaitForIntermediary;
        public GestureCastController controller;
        public override void Init(MenuData menuData, Menu menu)
        {
            base.Init(menuData, menu);
            EquipSlider = menu.GetCustomReference("EquipSlider").GetComponent<UnityEngine.UI.Slider>();
            CastSlider = menu.GetCustomReference("CastSlider").GetComponent<UnityEngine.UI.Slider>();
            IntermSlider = menu.GetCustomReference("IntermSlider").GetComponent<UnityEngine.UI.Slider>();
            EquipSliderLabel = menu.GetCustomReference("EquipSliderLabel").GetComponent<Text>();
            CastSliderLabel = menu.GetCustomReference("CastSliderLabel").GetComponent<Text>();
            IntermSliderLabel = menu.GetCustomReference("IntermSliderLabel").GetComponent<Text>();
            CastOnPoseToggle = menu.GetCustomReference("CastOnPoseToggle").GetComponent<Toggle>();
            WaitForIntermediaryToggle = menu.GetCustomReference("WaitForIntermediaryToggle").GetComponent<Toggle>();
            //Listeners
            EquipSlider.onValueChanged.AddListener(delegate { UpdateEquipText(); });
            CastSlider.onValueChanged.AddListener(delegate { UpdateCastText(); });
            IntermSlider.onValueChanged.AddListener(delegate { UpdateIntermText(); });
            CastOnPoseToggle.onValueChanged.AddListener(delegate { ToggleCastOnPose(); });
            WaitForIntermediaryToggle.onValueChanged.AddListener(delegate { ToggleWaitForIterm(); });
            //Data link
            controller = GameManager.local.gameObject.AddComponent<GestureCastController>();
            controller.data.menuLoaded = true;
            WaitForUpdate();
        }
        private IEnumerator WaitForUpdate()
        {
            while (!controller.data.levelLoaded)
            {
                yield return null;
            }
        }

        public void UpdateMenuData()
        {
            EquipSlider.value = controller.data.cooldownBetweenSwapTime;
            CastSlider.value = controller.data.cooldownBetweenCastTime;
            IntermSlider.value = controller.data.cooldownIntermediary;
            CastOnPoseToggle.isOn = controller.data.castOnPose;
            WaitForIntermediaryToggle.isOn = controller.data.waitForIntermediate;
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
