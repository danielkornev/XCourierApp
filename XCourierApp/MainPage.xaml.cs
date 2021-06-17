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
using XCourierApp.Storage.Journals;
using XCourierApp.Droid.Assistance;

namespace XCourierApp
{
	public partial class MainPage : ContentPage
	{
		public Storage.StorageContext StorageContext
		{
			get; internal set;
		}

		public JournalEntity CurrentJournal
		{
			get { return (JournalEntity)GetValue(CurrentJournalProperty); }
			set { SetValue(CurrentJournalProperty, value); }
		}

		//// Using a DependencyProperty as the backing store for CurrentJournal.  This enables animation, styling, binding, etc...
		//public static readonly DependencyProperty CurrentJournalProperty =
		//	DependencyProperty.Register("CurrentJournal", typeof(JournalEntity), typeof(MainPage), new PropertyMetadata(null));

		public static readonly BindableProperty CurrentJournalProperty = BindableProperty.Create("CurrentJournalProperty", typeof(JournalEntity), typeof(MainPage), null);

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


			#region Storage
			this.StorageContext = new Storage.StorageContext();

			// this.LeftInkCanvasView.InkPresenter.StrokeContainer.G


			// this should always return the last journal
			var lastJournal = this.StorageContext.Journals
				.Where(j => j.IsSoftDeleted == false)
				.OrderByDescending(j => j.DateUpdated)
				.FirstOrDefault();

			// loading pages
			lastJournal.Pages.Load();

			// ...

			// loading journal
			LoadJournal(lastJournal);

			#endregion
		}

		#region Appearing & Disappearing
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

			// TEMPORARY SAVING
			// binding
			var pages = this.CurrentJournal.Pages;
			var leftPage = pages[1];
			var rightPage = pages[2];

			leftPage.InkLayer = this.LeftInkCanvasView.InkPresenter.StrokeContainer.GetStrokes().ToArray();
			rightPage.InkLayer = this.RightInkCanvasView.InkPresenter.StrokeContainer.GetStrokes().ToArray();

			this.StorageContext.SavePage(leftPage);
			this.StorageContext.SavePage(rightPage);

			this.StorageContext.SaveJournal(this.CurrentJournal);

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

		#endregion

		#region Canvas Invalidation
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

		#endregion


		private async void LoadJournal(JournalEntity journalEntity)
		{
			this.CurrentJournal = journalEntity;

			// binding
			var pages = journalEntity.Pages;
			var leftPage = pages[1];
			var rightPage = pages[2];

			if (leftPage.InkLayer != null)
			{
				LoadInkStrokesFromInkLayer(leftPage.InkLayer, this.LeftInkCanvasView);
			}

			if (rightPage.InkLayer != null)
				LoadInkStrokesFromInkLayer(rightPage.InkLayer, this.RightInkCanvasView);

			//this.currentJournalBook.ItemsSource = journalEntity.Pages;
			//this.currentJournalBook.CurrentSheetIndex = journalEntity.LastOpenPage;

			// update current journal's metadata
			// UpdateCurrentJournalMetadata(journalEntity);

			//var page1 = this.CurrentJournal.Pages[0];


			this.CurrentJournal.PropertyChanged += CurrentJournal_PropertyChanged;

			// this.book.ItemsSource = this.CurrentJournal.Pages;

			// this.LeftPageNumber = this.book.CurrentPage;
			// this.RightPageNumber = this.LeftPageNumber + 1;
		}

		private void LoadInkStrokesFromInkLayer(XInkStroke[] inkLayer, InkCanvasView inkCanvasView)
		{
			foreach (var stroke in inkLayer)
			{
				if (stroke.DrawingAttributes.Color.A == 0)
				{
					stroke.DrawingAttributes.Color = Xamarin.Forms.Color.FromRgba(stroke.DrawingAttributes.Color.R, stroke.DrawingAttributes.Color.G, stroke.DrawingAttributes.Color.B, 255);
				}
				stroke.UpdateBounds();
			}

			Dispatcher.BeginInvokeOnMainThread(delegate
			{
				//inkCanvasView.BackgroundColor = sketchData.BackgroundColor;
				//InkCanvas.CanvasSize = new Size(sketchData.Width, sketchData.Height);
				inkCanvasView.InkPresenter.StrokeContainer.Clear();
				inkCanvasView.InkPresenter.StrokeContainer.Add(inkLayer);

				inkCanvasView.InvalidateCanvas(false, true);
			});
		}

		private void CurrentJournal_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// TO DO: 
		}

		private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
		{
			this.LeftStartMenuGrid.IsVisible = true;
		}

		private void LeftStartMenuSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
		{
			this.LeftStartMenuGrid.IsVisible = false;
		}

		private void InvokeDigitalAssistantButton_Clicked(object sender, EventArgs e)
		{
			DependencyService.Get<IDigitalAssistantActivity>().OpenDigitalAssistant();
		}
	} // class 
} // namespace
