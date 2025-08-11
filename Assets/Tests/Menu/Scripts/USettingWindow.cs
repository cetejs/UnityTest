using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace Tests
{
    public class UISettingWindow : UIWindow
    {
        private void Start()
        {
            // Test
            GetComponentInChildren<UIList>().SetCount(20);
        }
    }
}
