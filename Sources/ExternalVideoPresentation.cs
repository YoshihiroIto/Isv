using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Isv
{
	internal class ExternalVideoPresentation : Presentation
	{
		public PaintingView PaintingView{ get; private set; }

		public ExternalVideoPresentation (Context outerContext, Display display) : base (outerContext, display)
		{
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.main);

			PaintingView = FindViewById (Resource.Id.paintingview) as PaintingView;
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			PaintingView.OnTouchEvent (e);

			return base.OnTouchEvent(e);
		}
	}
}

