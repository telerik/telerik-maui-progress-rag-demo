using Microsoft.Maui.Controls;
using System;

namespace MauiProgressRagDemo.Behaviors
{
    public class ResponsiveGridLayoutBehavior : Behavior<Grid>
    {
        private const double BreakpointWidth = 450;
        private Grid attachedGrid;
        private bool isMobileLayout;

        protected override void OnAttachedTo(Grid bindable)
        {
            base.OnAttachedTo(bindable);

            this.attachedGrid = bindable;
            this.attachedGrid.SizeChanged += this.OnSizeChanged;
        }

        protected override void OnDetachingFrom(Grid bindable)
        {
            base.OnDetachingFrom(bindable);

            if (this.attachedGrid != null)
            {
                this.attachedGrid.SizeChanged -= this.OnSizeChanged;
                this.attachedGrid = null;
            }
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            if (this.attachedGrid == null)
            {
                return;
            }

#if WINDOWS || MACCATALYST
            var width = this.attachedGrid.Width;
            if (width <= BreakpointWidth && !this.isMobileLayout)
            {
                this.attachedGrid.RowDefinitions.Clear();
                this.attachedGrid.ColumnDefinitions.Clear();

                this.attachedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                this.attachedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                this.attachedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                this.attachedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                
                this.attachedGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                
                this.isMobileLayout = true;
                this.UpdateChildPositions(this.isMobileLayout);
            }
            else if (width > BreakpointWidth && this.isMobileLayout)
            {
                this.attachedGrid.RowDefinitions.Clear();
                this.attachedGrid.ColumnDefinitions.Clear();

                this.attachedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                this.attachedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                this.attachedGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                this.attachedGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                this.isMobileLayout = false;
                this.UpdateChildPositions(this.isMobileLayout);
            }
#endif
        }

        private void UpdateChildPositions(bool isMobileLayout)
        {
            if (this.attachedGrid.Children.Count < 4)
            {
                return;
            }

            if (isMobileLayout)
            {
                this.attachedGrid.SetRow(this.attachedGrid.Children[0], 0);
                this.attachedGrid.SetColumn(this.attachedGrid.Children[0], 0);

                this.attachedGrid.SetRow(this.attachedGrid.Children[1], 1);
                this.attachedGrid.SetColumn(this.attachedGrid.Children[1], 0);

                this.attachedGrid.SetRow(this.attachedGrid.Children[2], 2);
                this.attachedGrid.SetColumn(this.attachedGrid.Children[2], 0);

                this.attachedGrid.SetRow(this.attachedGrid.Children[3], 3);
                this.attachedGrid.SetColumn(this.attachedGrid.Children[3], 0);
            }
            else
            {
                this.attachedGrid.SetRow(this.attachedGrid.Children[0], 0);
                this.attachedGrid.SetColumn(this.attachedGrid.Children[0], 0);

                this.attachedGrid.SetRow(this.attachedGrid.Children[1], 0);
                this.attachedGrid.SetColumn(this.attachedGrid.Children[1], 1);

                this.attachedGrid.SetRow(this.attachedGrid.Children[2], 1);
                this.attachedGrid.SetColumn(this.attachedGrid.Children[2], 0);

                this.attachedGrid.SetRow(this.attachedGrid.Children[3], 1);
                this.attachedGrid.SetColumn(this.attachedGrid.Children[3], 1);
            }
        }
    }
}
