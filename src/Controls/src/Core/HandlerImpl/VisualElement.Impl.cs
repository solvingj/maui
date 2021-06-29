﻿using System;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;

namespace Microsoft.Maui.Controls
{
	public partial class VisualElement : IFrameworkElement
	{
		Semantics _semantics;
		IViewHandler _handler;

		public Rectangle Frame
		{
			get => Bounds; // TODO ezhart 2021-06-27 This will allocate a new rectangle every time, which isn't great for performance
			set
			{
				// These next four lines are the same as setting Bounds, but without the layout side-effects
				X = value.X;
				Y = value.Y;
				Width = value.Width;
				Height = value.Height;

				Handler?.UpdateValue(nameof(IFrameworkElement.Frame));
			}
		}

		public IViewHandler Handler
		{
			get => _handler;
			set
			{
				SetHandler(value);
			}
		}

		Paint IFrameworkElement.Background
		{
			get
			{
				if (!Brush.IsNullOrEmpty(Background))
					return Background;
				if (BackgroundColor.IsNotDefault())
					return new SolidColorBrush(BackgroundColor);
				return null;
			}
		}

		IShape IFrameworkElement.Clip => Clip;

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);
			Handler?.UpdateValue(propertyName);
		}

		IFrameworkElement IFrameworkElement.Parent => Parent as IView;

		public Size DesiredSize { get; protected set; }

		// TODO ezhart 2021-06-24 Does anything actually _use_ this Arrange method? Because if not, we could remove it
		// and if we can remove it, we might be able to drop the explicit arrange implementation below, and ArrangeOverride
		// which would make all of this much easier to grok
		public void Arrange(Rectangle bounds)
		{
			Layout(bounds);
		}

		Size IFrameworkElement.Arrange(Rectangle bounds)
		{
			return ArrangeOverride(bounds);
		}

		// ArrangeOverride provides a way to allow subclasses (e.g., Layout) to override Arrange even though
		// the interface has to be explicitly implemented to avoid conflict with the old Arrange method
		protected virtual Size ArrangeOverride(Rectangle bounds)
		{
			Frame = bounds;
			return Frame.Size;
		}

		public void Layout(Rectangle bounds)
		{
			Bounds = bounds;
		}

		void IFrameworkElement.InvalidateMeasure()
		{
			InvalidateMeasureOverride();
		}

		// InvalidateMeasureOverride provides a way to allow subclasses (e.g., Layout) to override InvalidateMeasure even though
		// the interface has to be explicitly implemented to avoid conflict with the VisualElement.InvalidateMeasure method
		protected virtual void InvalidateMeasureOverride() => Handler?.UpdateValue(nameof(IFrameworkElement.InvalidateMeasure));

		void IFrameworkElement.InvalidateArrange()
		{
		}

		Size IFrameworkElement.Measure(double widthConstraint, double heightConstraint)
		{
			return MeasureOverride(widthConstraint, heightConstraint);
		}

		// MeasureOverride provides a way to allow subclasses (e.g., Layout) to override Measure even though
		// the interface has to be explicitly implemented to avoid conflict with the old Measure method
		protected virtual Size MeasureOverride(double widthConstraint, double heightConstraint)
		{
			DesiredSize = this.ComputeDesiredSize(widthConstraint, heightConstraint);
			return DesiredSize;
		}

		Maui.FlowDirection IFrameworkElement.FlowDirection => FlowDirection.ToPlatformFlowDirection();
		Primitives.LayoutAlignment IFrameworkElement.HorizontalLayoutAlignment => default;
		Primitives.LayoutAlignment IFrameworkElement.VerticalLayoutAlignment => default;

		Visibility IFrameworkElement.Visibility => IsVisible.ToVisibility();

		Semantics IFrameworkElement.Semantics
		{
			get => _semantics;
		}

		// We don't want to initialize Semantics until someone explicitly 
		// wants to modify some aspect of the semantics class
		internal Semantics SetupSemantics() =>
			_semantics ??= new Semantics();

		double IFrameworkElement.Width => WidthRequest;
		double IFrameworkElement.Height => HeightRequest;

		public event EventHandler AttachingHandler;

		EventHandler _attachedHandler;
		public event EventHandler AttachedHandler
		{
			add
			{
				_attachedHandler += value;
				if (Handler != null)
					value?.Invoke(this, EventArgs.Empty);
			}
			remove
			{
				_attachedHandler -= value;
			}
		}
		public event EventHandler DetachingHandler;
		public event EventHandler DetachedHandler;

		void SetHandler(IViewHandler newHandler)
		{
			if (newHandler == _handler)
				return;

			var previousHandler = _handler;

			if (_handler != null)
			{
				DetachingHandler?.Invoke(this, EventArgs.Empty);
				OnDetachingHandler();
			}

			if (newHandler != null)
			{
				AttachingHandler?.Invoke(this, EventArgs.Empty);
				OnAttachingHandler();
			}

			_handler = newHandler;

			if (_handler?.VirtualView != this)
				_handler?.SetVirtualView((IView)this);

			IsPlatformEnabled = _handler != null;

			if (_handler != null)
			{
				_attachedHandler?.Invoke(this, EventArgs.Empty);
				OnAttachedHandler();
			}

			if (previousHandler != null)
			{
				DetachedHandler?.Invoke(this, EventArgs.Empty);
				OnDetachedHandler();
			}
		}

		public virtual void OnAttachingHandler() { }
		public virtual void OnAttachedHandler() { }
		public virtual void OnDetachingHandler() { }
		public virtual void OnDetachedHandler() { }
	}
}
