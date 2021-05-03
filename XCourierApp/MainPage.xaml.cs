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

			//LeftInkCanvasView.InkPresenter.
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
			//LeftInkCanvasView.InvalidateCanvas(false, true);
		}

		private void OnLeftInkCanvasViewStrokesCollected(Xamarin.Forms.Inking.Interfaces.IInkPresenter sender, XInkStrokesCollectedEventArgs args)
		{
			
		}

		private void RightInkCanvasViewPresenter_StrokesErased(XInkPresenter sender, XInkStrokesErasedEventArgs args)
		{
			RightInkCanvasView.InvalidateCanvas(false, true);
		}

		private void OnRightInkCanvasViewStrokesCollected(Xamarin.Forms.Inking.Interfaces.IInkPresenter sender, XInkStrokesCollectedEventArgs args)
		{
			
		}


		protected bool DeviceIsSpanned => DualScreenInfo.Current.SpanMode != TwoPaneViewMode.SinglePane;

		private void Button_Clicked(object sender, EventArgs e)
		{
			var di = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
		}

		private void ButtonDraw_Clicked(object sender, EventArgs e)
		{
			LeftInkCanvasView.InkPresenter.InputProcessingConfiguration.Mode = XInkInputProcessingMode.Inking;
		}

		private void ButtonErase_Clicked(object sender, EventArgs e)
		{
			LeftInkCanvasView.InkPresenter.InputProcessingConfiguration.Mode = XInkInputProcessingMode.Erasing;
		}
	} // class 
} // namespace
