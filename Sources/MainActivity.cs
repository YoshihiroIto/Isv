using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Isv
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/app_gltriangle",
#if __ANDROID_11__
		HardwareAccelerated=false,
#endif
		ConfigurationChanges = ConfigChanges.KeyboardHidden, LaunchMode = LaunchMode.SingleTask)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			RequestWindowFeature (WindowFeatures.NoTitle);

			// Inflate our UI from its XML layout description
			// - should match filename res/layout/main.xml ?
			SetContentView (Resource.Layout.main);

			// Load the view
			FindViewById (Resource.Id.paintingview);
		}
	}
}