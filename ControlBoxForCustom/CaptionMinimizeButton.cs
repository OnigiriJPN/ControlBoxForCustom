using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace ControlBoxForCustom
{
    public partial class CaptionMinimizeButton : CaptionButtonBase
    {
        static CaptionMinimizeButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CaptionMinimizeButton),
                new FrameworkPropertyMetadata(typeof(CaptionMinimizeButton)));
        }

        public CaptionMinimizeButton()
        {
            Content = "ー";
        }

        protected override int HitTestCode => HTMINBUTTON;

        protected override void ExecuteCaptionAction()
        {
            if(OwnerWindow is not null)
            {
                OwnerWindow.WindowState = WindowState.Minimized;
            }
        }
    }
}
