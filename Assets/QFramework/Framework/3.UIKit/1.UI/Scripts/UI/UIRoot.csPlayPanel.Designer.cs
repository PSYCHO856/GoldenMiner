﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QFramework.Example
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    
    
    // Generate Id:b8c1373a-774c-483b-b8a5-3a7274b947c4
    public partial class PlayPanel
    {
        
        public const string NAME = "PlayPanel";
        
        [SerializeField()]
        public UnityEngine.UI.Text Txt_Success;
        
        [SerializeField()]
        public UnityEngine.UI.Text Txt_Fail;
        
        [SerializeField()]
        public UnityEngine.UI.Image Joystick;
        
        [SerializeField()]
        public UnityEngine.UI.Button Btn_CharacterOne;
        
        [SerializeField()]
        public UnityEngine.UI.Button Btn_CharacterTwo;
        
        [SerializeField()]
        public UnityEngine.UI.Button Btn_CharacterThree;
        
        [SerializeField()]
        public PlayPanelMap PlayPanelMap;
        
        private PlayPanelData mPrivateData = null;
        
        public PlayPanelData Data
        {
            get
            {
                return mData;
            }
        }
        
        PlayPanelData mData
        {
            get
            {
                return mPrivateData ?? (mPrivateData = new PlayPanelData());
            }
            set
            {
                mUIData = value;
                mPrivateData = value;
            }
        }
        
        protected override void ClearUIComponents()
        {
            Txt_Success = null;
            Txt_Fail = null;
            Joystick = null;
            Btn_CharacterOne = null;
            Btn_CharacterTwo = null;
            Btn_CharacterThree = null;
            PlayPanelMap = null;
            mData = null;
        }
    }
}
