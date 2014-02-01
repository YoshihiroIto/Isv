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

            #if true

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
        private ToggleButton btnBlendD;
        private ToggleButton btnBlendE;
        private ToggleButton btnBlendF;

        private Button btnScale10;
        private Button btnScale20;
        private Button btnScale30;
        private Button btnScale05;

        private Button btnRotate0;
        private Button btnRotate45p;
        private Button btnRotate45m;

        private void InitializeControl()
        {
            lvwChA = FindViewById<ListView>(Resource.Id.lvwChA);
            lvwChB = FindViewById<ListView>(Resource.Id.lvwChB);

            btnSeekA = FindViewById<Button>(Resource.Id.btnSeekA);
            btnSeekB = FindViewById<Button>(Resource.Id.btnSeekB);

            btnBlendA = FindViewById<ToggleButton>(Resource.Id.btnBlendA);
            btnBlendB = FindViewById<ToggleButton>(Resource.Id.btnBlendB);
            btnBlendC = FindViewById<ToggleButton>(Resource.Id.btnBlendC);
            btnBlendD = FindViewById<ToggleButton>(Resource.Id.btnBlendD);
            btnBlendE = FindViewById<ToggleButton>(Resource.Id.btnBlendE);
            btnBlendF = FindViewById<ToggleButton>(Resource.Id.btnBlendF);

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
            btnBlendD.Tag = (int)Channel.D;
            btnBlendE.Tag = (int)Channel.E;
            btnBlendF.Tag = (int)Channel.F;
            btnBlendA.CheckedChange += OnBlendChanged; 
            btnBlendB.CheckedChange += OnBlendChanged; 
            btnBlendC.CheckedChange += OnBlendChanged; 
            btnBlendD.CheckedChange += OnBlendChanged; 
            btnBlendE.CheckedChange += OnBlendChanged; 
            btnBlendF.CheckedChange += OnBlendChanged; 

            //
            btnScale10 = FindViewById<Button>(Resource.Id.btnScale10);
            btnScale20 = FindViewById<Button>(Resource.Id.btnScale20);
            btnScale30 = FindViewById<Button>(Resource.Id.btnScale30);
            btnScale05 = FindViewById<Button>(Resource.Id.btnScale05);
            btnScale10.Tag = 1.0f;
            btnScale20.Tag = 2.0f;
            btnScale30.Tag = 3.0f;
            btnScale05.Tag = 0.5f;
            btnScale10.Click += OnScale;
            btnScale20.Click += OnScale;
            btnScale30.Click += OnScale;
            btnScale05.Click += OnScale;

            //
            btnRotate0   = FindViewById<Button>(Resource.Id.btnRotate0  );
            btnRotate45p = FindViewById<Button>(Resource.Id.btnRotate45p);
            btnRotate45m = FindViewById<Button>(Resource.Id.btnRotate45m);
            btnRotate0.Tag   = 0.0f;
            btnRotate45p.Tag = +45.0f;
            btnRotate45m.Tag = -45.0f;
            btnRotate0.Click   += OnRotate;
            btnRotate45p.Click += OnRotate;
            btnRotate45m.Click += OnRotate;
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

        private void OnScale(object sender, EventArgs e)
        {
            if (_presentation == null)
                return;

            var button = sender as Button;

			_presentation.PaintingView.Scale = (float)button.Tag;
        }

        private void OnRotate(object sender, EventArgs e)
        {
            if (_presentation == null)
                return;

            var button = sender as Button;

			_presentation.PaintingView.Rotate = (float)button.Tag;
        }

        private static string MovieDir = "/sdcard/Movies/";
        private static string[] _movies = null;
        private static string[] Movies
        {
            get {
                if (_movies == null)
                    _movies = Directory.EnumerateFiles (MovieDir).Select (x => Path.GetFileName (x)).OrderBy(x => x).ToArray();

                return _movies;
            }
        }
    }
}
