using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.DualScreen;
using Xamarin.Forms.Inking;
using Xamarin.Forms.Inking.Support;
using Xamarin.Forms.Inking.Views;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace XCourierApp
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

			// priority is to pane 1
			twoPaneView.PanePriority = TwoPaneViewPriority.Pane1;

			LeftInkCanvasView.InkPresenter.InputDeviceTypes = XCoreInputDeviceTypes.Pen;
			RightInkCanvasView.InkPresenter.InputDeviceTypes = XCoreInputDeviceTypes.Pen;

			// override
			// var statusbar = DependencyService.Get<IStatusBarPlatformSpecific>();
			// statusbar.SetStatusBarColor(Color.Black);

			// Left Page's InkCanvas
			LeftInkCanvasView.InkPresenter.StrokesCollected += OnLeftInkCanvasViewStrokesCollected;
			LeftInkCanvasView.InkPresenter.StrokesErased += LeftInkCanvasViewPresenter_StrokesErased;

			// Right Page's InkCanvas
			RightInkCanvasView.InkPresenter.StrokesCollected += OnRightInkCanvasViewStrokesCollected;
			RightInkCanvasView.InkPresenter.StrokesErased += RightInkCanvasViewPresenter_StrokesErased;
		}
		/// <summary>
		/// detach the events when disappering
		/// </summary>
		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			// Left Page's InkCanvas
			LeftInkCanvasView.InkPresenter.StrokesCollected -= OnLeftInkCanvasViewStrokesCollected;
			LeftInkCanvasView.InkPresenter.StrokesErased -= LeftInkCanvasViewPresenter_StrokesErased;

			// Right Page's InkCanvas
			LeftInkCanvasView.InkPresenter.StrokesCollected -= OnRightInkCanvasViewStrokesCollected;
			LeftInkCanvasView.InkPresenter.StrokesErased -= RightInkCanvasViewPresenter_StrokesErased;
		}


		/// <summary>
		/// Attach events and initialize the sketch data when appearing
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();

			// Left Page's InkCanvas
			LeftInkCanvasView.InkPresenter.StrokesCollected += OnLeftInkCanvasViewStrokesCollected;
			LeftInkCanvasView.InkPresenter.StrokesErased += LeftInkCanvasViewPresenter_StrokesErased;

			// Right Page's InkCanvas
			RightInkCanvasView.InkPresenter.StrokesCollected += OnRightInkCanvasViewStrokesCollected;
			RightInkCanvasView.InkPresenter.StrokesErased += RightInkCanvasViewPresenter_StrokesErased;
		}

		private void LeftInkCanvasViewPresenter_StrokesErased(XInkPresenter sender, XInkStrokesErasedEventArgs args)
		{
			LeftInkCanvasView.InvalidateCanvas(false, true);
		}

		private void OnLeftInkCanvasViewStrokesCollected(Xamarin.Forms.Inking.Interfaces.IInkPresenter sender, XInkStrokesCollectedEventArgs args)
		{
			LeftInkCanvasView.InvalidateCanvas(false, true);
		}

		private void RightInkCanvasViewPresenter_StrokesErased(XInkPresenter sender, XInkStrokesErasedEventArgs args)
		{
			RightInkCanvasView.InvalidateCanvas(false, true);
		}

		private void OnRightInkCanvasViewStrokesCollected(Xamarin.Forms.Inking.Interfaces.IInkPresenter sender, XInkStrokesCollectedEventArgs args)
		{
			RightInkCanvasView.InvalidateCanvas(false, true);
		}


		protected bool DeviceIsSpanned => DualScreenInfo.Current.SpanMode != TwoPaneViewMode.SinglePane;

		private void LeftInkCanvasView_Painting(object sender, SKCanvas e)
		{
			e.Clear(SKColor.Empty);
		}

		private void RightInkCanvasView_Painting(object sender, SKCanvas e)
		{
			e.Clear(SKColor.Empty);
		}
	} // class 
} // namespace
