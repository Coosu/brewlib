using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace BrewLib.Input
{
    public class InputManager : IDisposable
    {
        private InputHandler handler;
        private GameWindow window;

        private bool hadMouseFocus;
        private bool hasMouseHover;
        public bool HasMouseFocus => window.IsFocused && hasMouseHover;
        public bool HasWindowFocus => window.IsFocused;

        // Helpers
        public Vector2 MousePosition { get; private set; }

        public bool Control { get; private set; }
        public bool Shift { get; private set; }
        public bool Alt { get; private set; }

        public bool ControlOnly => Control && !Shift && !Alt;
        public bool ShiftOnly => !Control && Shift && !Alt;
        public bool AltOnly => !Control && !Shift && Alt;

        public bool ControlShiftOnly => Control && Shift && !Alt;
        public bool ControlAltOnly => Control && !Shift && Alt;
        public bool ShiftAltOnly => !Control && Shift && Alt;

        public InputManager(GameWindow window, InputHandler handler)
        {
            this.window = window;
            this.handler = handler;

            window.FocusedChanged += window_FocusedChanged;
            window.MouseEnter += window_MouseEnter;
            window.MouseLeave += window_MouseLeave;

            window.MouseUp += window_MouseUp;
            window.MouseDown += window_MouseDown;
            window.MouseWheel += window_MouseWheel;
            window.MouseMove += window_MouseMove;
            window.KeyDown += window_KeyDown;
            window.KeyUp += window_KeyUp;
            //window.key += window_KeyPress;
        }

        public void Dispose()
        {
            window.FocusedChanged -= window_FocusedChanged;
            window.MouseEnter -= window_MouseEnter;
            window.MouseLeave -= window_MouseLeave;

            window.MouseUp -= window_MouseUp;
            window.MouseDown -= window_MouseDown;
            window.MouseWheel -= window_MouseWheel;
            window.MouseMove += window_MouseMove;
            window.KeyDown -= window_KeyDown;
            window.KeyUp -= window_KeyUp;
            //window.KeyPress -= window_KeyPress;
        }

        private void updateMouseFocus()
        {
            if (hadMouseFocus != HasMouseFocus)
                hadMouseFocus = HasMouseFocus;

            handler.OnFocusChanged(new FocusChangedEventArgs(HasMouseFocus));
        }
        private void window_MouseEnter()
        {
            hasMouseHover = true;
            updateMouseFocus();
        }
        private void window_MouseLeave()
        {
            // https://github.com/opentk/opentk/issues/301
            return;

            hasMouseHover = false;
            updateMouseFocus();
        }
        private void window_FocusedChanged(FocusedChangedEventArgs e) => updateMouseFocus();

        private void window_MouseDown(MouseButtonEventArgs e) => handler.OnClickDown(e);
        private void window_MouseUp(MouseButtonEventArgs e) => handler.OnClickUp(e);
        private void window_MouseMove(MouseMoveEventArgs e)
        {
            MousePosition = new Vector2(e.X, e.Y);
            handler.OnMouseMove(e);
        }

        private void updateModifierState(KeyboardKeyEventArgs e)
        {
            Control = e.Modifiers.HasFlag(KeyModifiers.Control);
            Shift = e.Modifiers.HasFlag(KeyModifiers.Shift);
            Alt = e.Modifiers.HasFlag(KeyModifiers.Alt);
        }

        private void window_KeyDown(KeyboardKeyEventArgs e)
        {
            HandleStandardKeyPress(e);
            updateModifierState(e);
            handler.OnKeyDown(e);
        }
        private void window_KeyUp(KeyboardKeyEventArgs e) { updateModifierState(e); handler.OnKeyUp(e); }
        private void window_KeyPress(KeyPressEventArgs e) => handler.OnKeyPress(e);

        private bool dedupeMouseWheel;
        private void window_MouseWheel(MouseWheelEventArgs e)
        {
            if (dedupeMouseWheel = !dedupeMouseWheel)
                handler.OnMouseWheel(e);
        }

        private void HandleStandardKeyPress(KeyboardKeyEventArgs e)
        {
            if (e.Alt || e.Control || e.Command) return;
            Debug.WriteLine(JsonSerializer.Serialize(e));
            bool capsLock = System.Windows.Forms.Control.IsKeyLocked(
                System.Windows.Forms.Keys.CapsLock);
            bool upperCase = capsLock;
            char c;
            var i = (int)e.Key;
            if (i is >= 65 and <= 90)
            {
                if (e.Shift) upperCase = !upperCase;
                if (!upperCase) i += 32;
                c = (char)i;
            }
            else if (e.Key == Keys.Space) c = ' ';
            else if (e.Shift && e.Key == Keys.D1) c = '!';
            else if (e.Shift && e.Key == Keys.D2) c = '@';
            else if (e.Shift && e.Key == Keys.D3) c = '#';
            else if (e.Shift && e.Key == Keys.D4) c = '$';
            else if (e.Shift && e.Key == Keys.D5) c = '%';
            else if (e.Shift && e.Key == Keys.D6) c = '^';
            else if (e.Shift && e.Key == Keys.D7) c = '&';
            else if (e.Shift && e.Key == Keys.D8) c = '*';
            else if (e.Shift && e.Key == Keys.D9) c = '(';
            else if (e.Shift && e.Key == Keys.D0) c = ')';
            else if (e.Shift && i == 39) c = '"';
            else if (e.Shift && i == 44) c = '<';
            else if (e.Shift && i == 45) c = '_';
            else if (e.Shift && i == 46) c = '>';
            else if (e.Shift && i == 47) c = '?';
            else if (e.Shift && i == 59) c = ':';
            else if (e.Shift && i == 61) c = '+';
            else if (e.Shift && i == 91) c = '{';
            else if (e.Shift && i == 92) c = '|';
            else if (e.Shift && i == 93) c = '}';
            else if (e.Shift && i == 96) c = '~';
            else if (i is < 32 or > 126) return;
            else c = (char)i;

            window_KeyPress(new KeyPressEventArgs(c));
        }
    }

    public class FocusChangedEventArgs : EventArgs
    {
        private bool hasFocus;
        public bool HasFocus => hasFocus;

        public FocusChangedEventArgs(bool hasFocus)
        {
            this.hasFocus = hasFocus;
        }
    }
}
