using Telerik.Maui.Controls;
using Microsoft.Maui.Controls;

namespace MauiProgressRagDemo.Behaviors
{
    public class BorderFocusStateBehavior : Behavior<RadBorder>
    {
        private RadBorder border;
        private PointerGestureRecognizer pointerGesture;

        public VisualElement FocusSource { get; set; }

        protected override void OnAttachedTo(RadBorder bindable)
        {
            base.OnAttachedTo(bindable);

            this.border = bindable;

            if (this.FocusSource != null)
            {
                this.FocusSource.Focused += this.OnFocused;
                this.FocusSource.Unfocused += this.OnUnfocused;
            }

            this.pointerGesture = new PointerGestureRecognizer();
            this.pointerGesture.PointerEntered += this.OnPointerEntered;

            this.border.GestureRecognizers.Add(this.pointerGesture);
        }

        protected override void OnDetachingFrom(RadBorder bindable)
        {
            if (this.FocusSource != null)
            {
                this.FocusSource.Focused -= this.OnFocused;
                this.FocusSource.Unfocused -= this.OnUnfocused;
            }

            if (this.pointerGesture != null)
            {
                this.pointerGesture.PointerEntered -= this.OnPointerEntered;
                this.border.GestureRecognizers.Remove(this.pointerGesture);
            }

            this.border = null;
            base.OnDetachingFrom(bindable);
        }

        private void OnFocused(object sender, FocusEventArgs e)
        {
            VisualStateManager.GoToState(this.border, "Focused");
        }

        private void OnUnfocused(object sender, FocusEventArgs e)
        {
            VisualStateManager.GoToState(this.border, "Normal");
        }

        private void OnPointerEntered(object sender, PointerEventArgs e)
        {
            if (this.FocusSource?.IsFocused == true)
            {
                this.border.Dispatcher.Dispatch(() =>
                    VisualStateManager.GoToState(this.border, "Focused"));
            }
        }
    }
}