using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace ControlBoxForCustom
{
    public partial class CaptionMaximizeButton : CaptionButtonBase
    {
        static CaptionMaximizeButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CaptionMaximizeButton),
                new FrameworkPropertyMetadata(typeof(CaptionMaximizeButton)));
        }

        public CaptionMaximizeButton()
        {
            Content = "□";
        }

        protected override int HitTestCode => HTMAXBUTTON;

        protected override void OnOwnerWindowStateChanged(WindowState state)
        {
            Content = state == WindowState.Maximized ? "❐" : "□";
        }

        protected override void ExecuteCaptionAction()
        {
            if (OwnerWindow is null)
                return;

            OwnerWindow.WindowState = OwnerWindow.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }
}
