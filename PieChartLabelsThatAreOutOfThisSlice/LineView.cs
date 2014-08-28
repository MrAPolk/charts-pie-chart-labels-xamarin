using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;

namespace PieChartLabelsThatAreOutOfThisSlice
{
	public class LineView : UIView
	{
		private Dictionary<NSValue, NSValue[]> _pointPairs;

		public LineView ()
		{
		}

		public override void Draw (System.Drawing.RectangleF rect)
		{
			base.Draw (rect);

			using (CGContext c = UIGraphics.GetCurrentContext ()) {
				c.SetStrokeColor (UIColor.LightGray.CGColor);
				c.SetLineWidth (1);

				IEnumerator enumerator = _pointPairs.Values.GetEnumerator ();

				while (enumerator.MoveNext ()) {
					NSValue[] pair = enumerator.Current as NSValue[];

					PointF first, second;

					first = pair[0].PointFValue;
					second = pair[1].PointFValue;
					c.BeginPath ();

					c.MoveTo (first.X, first.Y);
					c.AddLineToPoint (second.X, second.Y);

					c.StrokePath ();

					c.AddEllipseInRect (new RectangleF (first.X - 2, first.Y - 2, 4, 4));
					c.AddEllipseInRect (new RectangleF (second.X - 2, second.Y - 2, 4, 4));

					c.SetFillColor (UIColor.LightGray.CGColor);
					c.FillPath ();
				}
			}
		}

		public void addPointPair(PointF first, PointF second, UILabel label) {
			if (_pointPairs == null) {
				_pointPairs = new Dictionary<NSValue, NSValue[]>();
			}

			NSValue firstValue, secondValue;

			firstValue = NSValue.FromPointF (first);
			secondValue = NSValue.FromPointF (second);

			//Generate a key from the label.
			NSValue labelKey = NSValue.ValueFromNonretainedObject (label);

			// If we already have a line for this label, update it. Otherwise add a new one to the view.
			NSValue[] existingPair;
			if (_pointPairs.TryGetValue (labelKey, out existingPair)) {
				existingPair [0] = firstValue;
				existingPair [1] = secondValue;
			} else {
				NSValue[] newPair = new NSValue[]{ firstValue, secondValue };
				_pointPairs.Add (labelKey, newPair);
			}

			SetNeedsDisplay ();
		}

		public void reset() {
			if (_pointPairs != null) {
				_pointPairs.Clear ();
			}
		}
	}
}

