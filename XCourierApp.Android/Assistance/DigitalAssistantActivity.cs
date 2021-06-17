using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XCourierApp.Droid.Assistance;

[assembly: Xamarin.Forms.Dependency(typeof(DigitalAssistantActivity))]
namespace XCourierApp.Droid.Assistance
{
	public class DigitalAssistantActivity : IDigitalAssistantActivity
	{
		public void OpenDigitalAssistant()
		{
			// startActivity(new Intent(Intent.ACTION_VOICE_COMMAND).setFlags(Intent.FLAG_ACTIVITY_NEW_TASK));

			var intent = new Intent(Intent.ActionVoiceCommand);
			intent.SetFlags(ActivityFlags.NewTask); 

			// trying it this way
			Android.App.Application.Context.StartActivity(intent);
		}
	} // class
} // namespace