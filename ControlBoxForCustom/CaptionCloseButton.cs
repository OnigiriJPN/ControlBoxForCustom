using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace ControlBoxForCustom
{
    public partial class CaptionCloseButton : CaptionButtonBase
    {
        static CaptionCloseButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CaptionCloseButton),
            new FrameworkPropertyMetadata(typeof(CaptionCloseButton)));
        }
        public CaptionCloseButton()
        {
            Content = "×";
        }

        protected override int HitTestCode => HTCLOSE;

        protected override void ExecuteCaptionAction()
        {
            OwnerWindow?.Close();
        }
    }
}
