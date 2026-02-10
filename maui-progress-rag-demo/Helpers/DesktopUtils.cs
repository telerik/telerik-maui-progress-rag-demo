using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System;

namespace MauiProgressRagDemo.Helpers;

public static class DesktopUtils
{
    public static readonly BindableProperty MouseOverCursorTypeProperty =
        BindableProperty.CreateAttached(nameof(MouseOverCursorType), typeof(MouseCursorType), typeof(DesktopUtils), MouseCursorType.Arrow, 
            propertyChanged: OnMouseCursorTypeChanged);

    private static readonly BindableProperty MouseHelperProperty = 
        BindableProperty.CreateAttached(nameof(MouseHelper), typeof(MouseHelper), typeof(DesktopUtils), null);

    public static MouseCursorType MouseOverCursorType { get; set; }

    public static MouseCursorType GetMouseOverCursorType(BindableObject bindable)
    {
        return (MouseCursorType)bindable.GetValue(MouseOverCursorTypeProperty);
    }

    public static void SetMouseOverCursorType(BindableObject bindable, MouseCursorType value)
    {
        bindable.SetValue(MouseOverCursorTypeProperty, value);
    }

    private static MouseHelper GetMouseHelper(BindableObject bindable)
    {
        return (MouseHelper)bindable.GetValue(MouseHelperProperty);
    }

    private static void SetMouseHelper(BindableObject bindable, MouseHelper value)
    {
        bindable.SetValue(MouseHelperProperty, value);
    }

    private static MouseHelper GetOrCreateMouseHelper(View view)
    {
        MouseHelper mouseHelper = GetMouseHelper(view);

        if (mouseHelper == null)
        {
            mouseHelper = new MouseHelper(view);
            SetMouseHelper(view, mouseHelper);
        }

        return mouseHelper;
    }

    private static void OnMouseCursorTypeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is View view)
        {
#if MACCATALYST || WINDOWS
            var pointerRecognizer = new PointerGestureRecognizer();
            var mouseHelper = GetOrCreateMouseHelper(view);
            pointerRecognizer.PointerEntered += (s, e) => mouseHelper.MouseCursorType = (MouseCursorType)newValue;
            pointerRecognizer.PointerExited += (s, e) => mouseHelper.MouseCursorType = MouseCursorType.Arrow;
            view.GestureRecognizers.Add(pointerRecognizer);
#endif
        }
    }
}

public enum MouseCursorType
{
    Arrow,
    Hand,
    IBeam,
}
