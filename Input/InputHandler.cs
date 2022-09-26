﻿using OpenTK;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using System.Windows.Forms;

namespace BrewLib.Input
{
    public interface InputHandler
    {
        void OnFocusChanged(FocusChangedEventArgs e);
        bool OnClickDown(MouseButtonEventArgs e);
        bool OnClickUp(MouseButtonEventArgs e);
        bool OnMouseWheel(MouseWheelEventArgs e);
        void OnMouseMove(MouseMoveEventArgs e);
        bool OnKeyDown(KeyboardKeyEventArgs e);
        bool OnKeyUp(KeyboardKeyEventArgs e);
        bool OnTextInput(TextInputEventArgs e);
    }
}
