﻿using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

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

        private Dictionary<int, GamepadManager> gamepadManagers = new Dictionary<int, GamepadManager>();
        public IEnumerable<GamepadManager> GamepadManagers => gamepadManagers.Values;
        public GamepadManager GetGamepadManager(int gamepadIndex = 0) => gamepadManagers[gamepadIndex];

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
            foreach (var gamepadIndex in new List<int>(gamepadManagers.Keys))
                DisableGamepadEvents(gamepadIndex);
            gamepadManagers.Clear();

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

        public void EnableGamepadEvents(int gamepadIndex = 0)
        {
            var manager = new GamepadManager(gamepadIndex);
            manager.OnConnected += gamepadManager_OnConnected;
            manager.OnButtonDown += gamepadManager_OnButtonDown;
            manager.OnButtonUp += gamepadManager_OnButtonUp;
            gamepadManagers.Add(gamepadIndex, manager);
        }

        public void DisableGamepadEvents(int gamepadIndex = 0)
        {
            var manager = gamepadManagers[gamepadIndex];
            manager.OnConnected -= gamepadManager_OnConnected;
            manager.OnButtonDown -= gamepadManager_OnButtonDown;
            manager.OnButtonUp -= gamepadManager_OnButtonUp;
            gamepadManagers.Remove(gamepadIndex);
        }

        public void Update()
        {
            foreach (var gamepadManager in gamepadManagers.Values)
                gamepadManager.Update();
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
        private void window_KeyDown(KeyboardKeyEventArgs e) { updateModifierState(e); handler.OnKeyDown(e); }
        private void window_KeyUp(KeyboardKeyEventArgs e) { updateModifierState(e); handler.OnKeyUp(e); }
        //private void window_KeyPress(KeyPressEventArgs e) => handler.OnKeyPress(e);

        private bool dedupeMouseWheel;
        private void window_MouseWheel(MouseWheelEventArgs e)
        {
            if (dedupeMouseWheel = !dedupeMouseWheel)
                handler.OnMouseWheel(e);
        }

        private void gamepadManager_OnConnected(object sender, GamepadEventArgs e) => handler.OnGamepadConnected(e);
        private void gamepadManager_OnButtonDown(object sender, GamepadButtonEventArgs e) => handler.OnGamepadButtonDown(e);
        private void gamepadManager_OnButtonUp(object sender, GamepadButtonEventArgs e) => handler.OnGamepadButtonUp(e);
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
