using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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

			#if !true

			_mediaRouterCallback = new MediaRouterCallback (this);

			_mediaRouter = (MediaRouter) GetSystemService (Android.Content.Context.MediaRouterService);

			SetContentView (Resource.Layout.ControlPanel);
			InitializeControl();

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

		private ListView lvwChA;
		private ListView lvwChB;
		private Button btnSeekA;
		private Button btnSeekB;
		private ToggleButton btnBlendA;
		private ToggleButton btnBlendB;
		private ToggleButton btnBlendC;

		private void InitializeControl()
		{
			lvwChA = FindViewById<ListView>(Resource.Id.lvwChA);
			lvwChB = FindViewById<ListView>(Resource.Id.lvwChB);

			btnSeekA = FindViewById<Button>(Resource.Id.btnSeekA);
            btnSeekB = FindViewById<Button>(Resource.Id.btnSeekB);

            btnBlendA = FindViewById<ToggleButton>(Resource.Id.btnBlendA);
            btnBlendB = FindViewById<ToggleButton>(Resource.Id.btnBlendB);
            btnBlendC = FindViewById<ToggleButton>(Resource.Id.btnBlendC);

            lvwChA.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, Movies);
            lvwChB.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, Movies);

			//
			btnSeekA.Tag = (int)Channel.A;
			btnSeekB.Tag = (int)Channel.B;		
			btnSeekA.Click += OnSeekClick;
			btnSeekB.Click += OnSeekClick;

			//		
			lvwChA.Tag = (int)Channel.A;
			lvwChB.Tag = (int)Channel.B;
			lvwChA.ItemClick += OnMovieItemChanged;
			lvwChB.ItemClick += OnMovieItemChanged;

            //
            btnBlendA.Tag = (int)Channel.A;
            btnBlendB.Tag = (int)Channel.B;
            btnBlendC.Tag = (int)Channel.C;
			btnBlendA.CheckedChange += OnBlendChanged; 
			btnBlendB.CheckedChange += OnBlendChanged; 
			btnBlendC.CheckedChange += OnBlendChanged; 
		}

		private void OnSeekClick(object sender, EventArgs e)
		{
			if (_presentation == null)
				return;

			var button = sender as Button;

			_presentation.PaintingView.SeekBegin ((Channel)(int)button.Tag);
		}

		private void OnMovieItemChanged(object sender, AdapterView.ItemClickEventArgs e)
		{
			if (_presentation == null)
				return;

			var listView = sender as ListView;
			var filePath = Path.Combine (MovieDir, Movies [e.Id]);
		                                                                                                                                                                              
			_presentation.PaintingView.Play ((Channel)(int)listView.Tag, filePath);
		}

		private void OnBlendChanged(object sender, EventArgs e)
        {
			if (_presentation == null)
				return;

			var button = sender as ToggleButton;

			_presentation.PaintingView.StartBlend ((Channel)(int)button.Tag, button.Checked);
        }

		private static string MovieDir = "/sdcard/Movies/";

		private static string[] _movies = null;
		private static string[] Movies
		{
			get {
				if (_movies == null)
					_movies = Directory.EnumerateFiles (MovieDir).Select (x => Path.GetFileName (x)).ToArray();

				return _movies;
			}
		}
	}
}
