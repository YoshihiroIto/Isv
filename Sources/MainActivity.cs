using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Media;

namespace Isv
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/app_gltriangle",
#if __ANDROID_11__
		HardwareAccelerated=false,
#endif
		ScreenOrientation = ScreenOrientation.Landscape,
		ConfigurationChanges = ConfigChanges.KeyboardHidden, LaunchMode = LaunchMode.SingleTask)]
	public class MainActivity : Activity
	{
		private MediaRouter _mediaRouter;
		private MediaRouterCallback _mediaRouterCallback;
		private ExternalVideoPresentation _presentation;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			RequestWindowFeature (WindowFeatures.NoTitle);
			Window.AddFlags (WindowManagerFlags.Fullscreen);

			#if true

			_mediaRouterCallback = new MediaRouterCallback (this);

			// Get the MediaRouter service
			_mediaRouter = (MediaRouter) GetSystemService (Android.Content.Context.MediaRouterService);

			#else

			// Inflate our UI from its XML layout description
			// - should match filename res/layout/main.xml ?
			SetContentView (Resource.Layout.main);

			#endif
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			if (_presentation != null)
			{
				_presentation.OnTouchEvent (e);
			}

			return base.OnTouchEvent (e);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			if (_mediaRouter != null) {

				// Register a callback for all events related to live video devices
				_mediaRouter.AddCallback (MediaRouteType.LiveVideo, _mediaRouterCallback);

				// Update the displays based on the currently active routes
				UpdatePresentation ();
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			if (_mediaRouter != null) {
				// Stop listening for changes to media routes.
				_mediaRouter.RemoveCallback (_mediaRouterCallback);
			}
		}

		protected override void OnStop ()
		{
			base.OnStop ();

			// Dismiss the presentation when the activity is not visible.
			if (_presentation != null) {
				_presentation.Dismiss ();
				_presentation = null;
			}
		}

		public void UpdatePresentation ()
		{
			// Get the selected route for live video
			var selectedRoute = _mediaRouter.GetSelectedRoute (MediaRouteType.LiveVideo);

			// Get its Display if a valid route has been selected
			Display selectedDisplay = null;
			if (selectedRoute != null)
				selectedDisplay = selectedRoute.PresentationDisplay;

			/*
			 * Dismiss the current presentation if the display has changed or no new
			 * route has been selected
			 */
			if (_presentation != null && _presentation.Display != selectedDisplay) {
				_presentation.Dismiss ();
				_presentation = null;
			}

			/*
			 * Show a new presentation if the previous one has been dismissed and a
			 * route has been selected.
			 */
			if (_presentation == null && selectedDisplay != null) {

				// Initialise a new Presentation for the Display
				_presentation = new ExternalVideoPresentation (this, selectedDisplay);
				_presentation.DismissEvent += delegate (object sender, EventArgs e) {
					if (sender == _presentation) {
						_presentation = null;
					}
				};

				// Try to show the presentation, this might fail if the display has
				// gone away in the mean time
				try {
					_presentation.Show ();
				} catch (WindowManagerInvalidDisplayException ex) {
					// Couldn't show presentation - display was already removed
					_presentation = null;
					Console.WriteLine (ex);
				}
			}
		}
		class MediaRouterCallback : MediaRouter.SimpleCallback
		{
			MainActivity main;

			public MediaRouterCallback (MainActivity m)
			{
				main = m;
			}

			public override void OnRouteSelected (MediaRouter router, MediaRouteType type, MediaRouter.RouteInfo info)
			{
				main.UpdatePresentation ();
			}

			public override void OnRouteUnselected (MediaRouter router, MediaRouteType type, MediaRouter.RouteInfo info)
			{
				main.UpdatePresentation ();
			}

			public override void OnRoutePresentationDisplayChanged (MediaRouter router, MediaRouter.RouteInfo info)
			{
				main.UpdatePresentation ();
			}
		}
	}
}