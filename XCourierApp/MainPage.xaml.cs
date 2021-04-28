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

			//twoPaneView.TallModeConfiguration = Xamarin.Forms.DualScreen.TwoPaneViewTallModeConfiguration.SinglePane;

			// priority is to pane 1
			twoPaneView.PanePriority = TwoPaneViewPriority.Pane1;
			

			//twoPaneView.MinWideModeWidth = 1400;
			//twoPaneView.MinTallModeHeight = 1800;
			//twoPaneView.Pane

			// by default, Android app opens on one screen not two
			//twoPaneView.Mode = TwoPaneViewMode.SinglePane;
		}


		protected bool DeviceIsSpanned => DualScreenInfo.Current.SpanMode != TwoPaneViewMode.SinglePane;

		private void Button_Clicked(object sender, EventArgs e)
		{
			var di = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
		}
	} // class 
} // namespace
