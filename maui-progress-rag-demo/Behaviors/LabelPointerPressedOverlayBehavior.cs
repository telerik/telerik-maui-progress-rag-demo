namespace MauiProgressRagDemo.Behaviors;

public class LabelPointerPressedOverlayBehavior : Behavior<Label>
{
    private PointerGestureRecognizer? gestureRecognizer;

    public required View OverlayElement { get; set; }

    protected override void OnAttachedTo(Label bindable)
    {
        base.OnAttachedTo(bindable);

        this.gestureRecognizer = new PointerGestureRecognizer();
        this.gestureRecognizer.PointerPressed += this.OnPointerPressed;
        this.gestureRecognizer.PointerReleased += this.OnPointerReleased;
        this.gestureRecognizer.PointerExited += this.OnPointerExited;

        bindable.GestureRecognizers.Add(this.gestureRecognizer);
    }

    protected override void OnDetachingFrom(Label bindable)
    {
        base.OnDetachingFrom(bindable);

        if (this.gestureRecognizer != null)
        {
            this.gestureRecognizer.PointerPressed -= this.OnPointerPressed;
            this.gestureRecognizer.PointerReleased -= this.OnPointerReleased;
            this.gestureRecognizer.PointerExited -= this.OnPointerExited;
            bindable.GestureRecognizers.Remove(this.gestureRecognizer);
            this.gestureRecognizer = null;
        }
    }

    private void OnPointerPressed(object? sender, PointerEventArgs e)
    {
        this.ToggleOverlay(sender as Element, true);
    }

    private void OnPointerReleased(object? sender, PointerEventArgs e)
    {
        this.ToggleOverlay(sender as Element, false);
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        this.ToggleOverlay(sender as Element, false);
    }

    private void ToggleOverlay(Element? element, bool isVisible)
    {
        if (element == null || this.OverlayElement == null)
        {
            return;
        }   

        this.OverlayElement.IsVisible = isVisible;
    }
}
