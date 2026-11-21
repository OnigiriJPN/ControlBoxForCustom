using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;

namespace ControlBoxForCustom
{
    public abstract partial class CaptionButtonBase : Button
    {
        protected const int WM_NCHITTEST = 0x0084;
        protected const int WM_NCLBUTTONDOWN = 0x00A1;
        protected const int WM_NCLBUTTONUP = 0x00A2;

        protected const int HTMINBUTTON = 8;
        protected const int HTMAXBUTTON = 9;
        protected const int HTCLOSE = 20;
        private static readonly Uri GenericDictionaryUri =
            new("/ControlBoxForCustom;component/Themes/Generic.xaml", UriKind.Relative);
        private Window? _ownerWindow;
        private HwndSource? _hwndSource;
        protected CaptionButtonBase()
        {
            EnsureResourceDictionaryLoaded();
            WindowChrome.SetIsHitTestVisibleInChrome(this, true);
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }
        private void EnsureResourceDictionaryLoaded()
        {
            if (Application.Current?.Resources is not null)
            {
                if (!ContainsDictionary(Application.Current.Resources, GenericDictionaryUri))
                {
                    var dict = (ResourceDictionary)Application.LoadComponent(GenericDictionaryUri);
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }
                return;
            }

            if (!ContainsDictionary(this.Resources, GenericDictionaryUri))
            {
                var dict = (ResourceDictionary)Application.LoadComponent(GenericDictionaryUri);
                this.Resources.MergedDictionaries.Add(dict);
            }
        }

        private static bool ContainsDictionary(ResourceDictionary container, Uri uri)
        {
            return container.MergedDictionaries.Any(d => d.Source == uri);
        }

        protected Window? OwnerWindow => _ownerWindow ?? Window.GetWindow(this);
        protected abstract int HitTestCode { get; }

        protected abstract void ExecuteCaptionAction();

        protected virtual void OnOwnerWindowStateChanged(WindowState state) { }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Detach();

            _ownerWindow = Window.GetWindow(this);
            if (_ownerWindow is not null)
            {
                _ownerWindow.StateChanged += HandleOwnerStateChanged;
                _ownerWindow.SourceInitialized += HandleOwnerSourceInitialized;
                OnOwnerWindowStateChanged(_ownerWindow.WindowState);

                if (PresentationSource.FromVisual(_ownerWindow) is HwndSource source)
                {
                    AttachHook(source);
                }
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Detach();
        }

        private void HandleOwnerSourceInitialized(object? sender, EventArgs e)
        {
            if (_ownerWindow is not null && PresentationSource.FromVisual(_ownerWindow) is HwndSource source)
            {
                AttachHook(source);
            }
        }

        private void AttachHook(HwndSource source)
        {
            if (_hwndSource is not null)
                return;

            _hwndSource = source;
            _hwndSource.AddHook(WndProc);
        }

        private void Detach()
        {
            if (_hwndSource is not null)
            {
                _hwndSource.RemoveHook(WndProc);
                _hwndSource = null;
            }

            if (_ownerWindow is not null)
            {
                _ownerWindow.StateChanged -= HandleOwnerStateChanged;
                _ownerWindow.SourceInitialized -= HandleOwnerSourceInitialized;
                _ownerWindow = null;
            }
        }

        private void HandleOwnerStateChanged(object? sender, EventArgs e)
        {
            if (_ownerWindow is not null)
            {
                OnOwnerWindowStateChanged(_ownerWindow.WindowState);
            }
        }

        protected override void OnClick()
        {
            base.OnClick();
            ExecuteCaptionAction();
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_NCHITTEST)
            {
                int x = (short)(lParam.ToInt64() & 0xFFFF);
                int y = (short)((lParam.ToInt64() >> 16) & 0xFFFF);
                if (IsScreenPointOverButton(new Point(x, y)))
                {
                    handled = true;
                    return new IntPtr(HitTestCode);
                }
            }
            else if (msg == WM_NCLBUTTONDOWN && wParam.ToInt32() == HitTestCode)
            {
                handled = true;
                return IntPtr.Zero;
            }
            else if (msg == WM_NCLBUTTONUP && wParam.ToInt32() == HitTestCode)
            {
                ExecuteCaptionAction();
                handled = true;
                return IntPtr.Zero;
            }

            return IntPtr.Zero;
        }

        private bool IsScreenPointOverButton(Point screenPoint)
        {
            if (!IsVisible || !IsEnabled || ActualWidth <= 0 || ActualHeight <= 0)
                return false;

            if (PresentationSource.FromVisual(this)?.CompositionTarget is null)
                return false;

            Point topLeft = PointToScreen(new Point(0, 0));
            Point bottomRight = PointToScreen(new Point(ActualWidth, ActualHeight));

            double left = Math.Min(topLeft.X, bottomRight.X);
            double right = Math.Max(topLeft.X, bottomRight.X);
            double top = Math.Min(topLeft.Y, bottomRight.Y);
            double bottom = Math.Max(topLeft.Y, bottomRight.Y);

            return screenPoint.X >= left && screenPoint.X <= right &&
                   screenPoint.Y >= top && screenPoint.Y <= bottom;
        }
    }
}
