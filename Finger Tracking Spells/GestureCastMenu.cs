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
        private Button EquipSliderRight;
        private Button EquipSliderLeft;
        private Button CastSliderRight;
        private Button CastSliderLeft;
        private Button IntermSliderRight;
        private Button IntermSliderLeft;
        private bool CastOnPose;
        private bool WaitForIntermediary;
        public GestureCastController controller;
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
            //Buttons
            EquipSliderRight = menu.GetCustomReference("EquipSliderRight").GetComponent<Button>();
            EquipSliderLeft = menu.GetCustomReference("EquipSliderLeft").GetComponent<Button>();
            CastSliderRight = menu.GetCustomReference("CastSliderRight").GetComponent<Button>();
            CastSliderLeft = menu.GetCustomReference("CastSliderLeft").GetComponent<Button>();
            IntermSliderLeft = menu.GetCustomReference("IntermSliderRight").GetComponent<Button>();
            IntermSliderRight = menu.GetCustomReference("IntermSliderLeft").GetComponent<Button>();
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
            //Data link
            controller = GameManager.local.gameObject.AddComponent<GestureCastController>();
            controller.Deserialize();
            UpdateMenuData();
            Debug.Log("GestureCastMenu loaded");
        }

        public void UpdateMenuData()
        {
            EquipSlider.value = controller.data.cooldownBetweenSwapTime;
            CastSlider.value = controller.data.cooldownBetweenCastTime;
            IntermSlider.value = controller.data.cooldownIntermediary;
            CastOnPoseToggle.isOn = controller.data.castOnPose;
            WaitForIntermediaryToggle.isOn = controller.data.waitForIntermediate;
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
