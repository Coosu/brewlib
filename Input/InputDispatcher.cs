﻿using OpenTK;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BrewLib.Input
{
    public class InputDispatcher : InputHandler
    {
        private List<InputHandler> handlers = new List<InputHandler>();

        public InputDispatcher()
        {
        }

        public InputDispatcher(InputHandler[] handlers)
        {
            foreach (var handler in handlers)
                this.handlers.Add(handler);
        }

        public void Add(InputHandler handler)
        {
            handlers.Add(handler);
        }

        public void Remove(InputHandler handler)
        {
            handlers.Remove(handler);
        }

        public void Clear()
        {
            handlers.Clear();
        }

        public void OnFocusChanged(FocusChangedEventArgs e)
        {
            foreach (var handler in handlers)
                handler.OnFocusChanged(e);
        }

        public bool OnClickDown(MouseButtonEventArgs e)
        {
            foreach (var handler in handlers)
                if (handler.OnClickDown(e))
                    return true;
            return false;
        }

        public bool OnClickUp(MouseButtonEventArgs e)
        {
            foreach (var handler in handlers)
                if (handler.OnClickUp(e))
                    return true;
            return false;
        }

        public bool OnMouseWheel(MouseWheelEventArgs e)
        {
            foreach (var handler in handlers)
                if (handler.OnMouseWheel(e))
                    return true;
            return false;
        }

        public void OnMouseMove(MouseMoveEventArgs e)
        {
            foreach (var handler in handlers)
                handler.OnMouseMove(e);
        }

        public bool OnKeyDown(KeyboardKeyEventArgs e)
        {
            foreach (var handler in handlers)
                if (handler.OnKeyDown(e))
                    return true;
            return false;
        }

        public bool OnKeyUp(KeyboardKeyEventArgs e)
        {
            foreach (var handler in handlers)
                if (handler.OnKeyUp(e))
                    return true;
            return false;
        }

        public bool OnTextInput(TextInputEventArgs e)
        {
            foreach (var handler in handlers)
                if (handler.OnTextInput(e))
                    return true;
            return false;
        }
    }
}
