using Telerik.Maui.Controls;
using Telerik.Maui.Controls.CollectionView;

namespace MauiProgressRagDemo.Behaviors
{
    public class CollectionViewResponsiveLayoutBehavior : Behavior<RadCollectionView>
    {
        public double ThresholdWidth { get; set; } = 768;
        public int SmallSpanCount { get; set; } = 1;
        public int LargeSpanCount { get; set; } = 3;
        private RadCollectionView? collectionView;

        protected override void OnAttachedTo(RadCollectionView bindable)
        {
            base.OnAttachedTo(bindable);
            this.collectionView = bindable;
            this.collectionView.SizeChanged += this.OnSizeChanged;
            this.UpdateSpanCount();
        }

        protected override void OnDetachingFrom(RadCollectionView bindable)
        {
            base.OnDetachingFrom(bindable);
            //this.collectionView?.SizeChanged -= this.OnSizeChanged;
            if (this.collectionView != null)
            {
                this.collectionView.SizeChanged -= this.OnSizeChanged;
            }
            this.collectionView = null;
        }

        private void OnSizeChanged(object? sender, EventArgs e)
        {
            this.UpdateSpanCount();
        }

        private void UpdateSpanCount()
        {
            if (this.collectionView == null)
            {
                return;
            }

            var itemsLayout = this.collectionView.ItemsLayout as CollectionViewGridLayout;
            if (itemsLayout == null)
            {
                return;
            }

            var width = this.collectionView.Width;
            if (width <= 0)
            {
                return;
            }

            var currentSpanCount = itemsLayout.SpanCount;
            if (width <= this.ThresholdWidth)
            {
                if (currentSpanCount != this.SmallSpanCount)
                {
                    itemsLayout.SpanCount = this.SmallSpanCount;
                }
            }
            else
            {
                if (currentSpanCount != this.LargeSpanCount)
                {
                    itemsLayout.SpanCount = this.LargeSpanCount;
                }
            }
        }
    }
}
