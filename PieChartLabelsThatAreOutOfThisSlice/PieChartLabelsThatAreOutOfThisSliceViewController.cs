using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using ShinobiCharts;

namespace PieChartLabelsThatAreOutOfThisSlice
{
	public partial class PieChartLabelsThatAreOutOfThisSliceViewController : UIViewController
	{

		private ShinobiChart _chart;
		private LineView _lineView;

		#region View lifecycle

		public PieChartLabelsThatAreOutOfThisSliceViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.White;

			_chart = new ShinobiChart (new RectangleF (150, 300, 500, 500)) {
				AutoresizingMask = ~UIViewAutoresizing.None
			};

			// Enter license key here
//			_chart.LicenseKey = "";
			
			_lineView = new LineView ();
			_lineView.UserInteractionEnabled = false;
			_lineView.BackgroundColor = UIColor.Clear;
			_chart.AddSubview (_lineView);


			_chart.Delegate = new CustomSliceDelegate (_lineView);
			_chart.DataSource = new MyDataSource ();
			_chart.Canvas.ClipsToBounds = false;
			_chart.ClipsToBounds = false;
			_chart.CanvasAreaBackgroundColor = UIColor.DarkGray;

			View.AddSubview(_chart);
		}

		#endregion
		
	}

	public class CustomSliceDelegate : SChartDelegate {

		const float EXTRUSION = 200;

		private LineView _lineView;

		public CustomSliceDelegate (LineView lineView) {
			_lineView = lineView;
		}
	
		protected override void OnAddingRadialLabel (ShinobiChart chart, UILabel label, SChartRadialDataPoint datapoint, int index, SChartRadialSeries series)
		{

			SChartPieSeries pieSeries = (SChartPieSeries)series;

			// Get our radial point from our datasource method.

			// Three points:
			PointF pieCenter; // Chart center for trig calculations.
			PointF oldLabelCenter; // Original label center.
			PointF labelCenter; // New label center.
			PointF endOfLine;

			pieCenter = pieSeries.DonutCenter;
			oldLabelCenter = labelCenter = pieSeries.GetSliceCenter(index);

			// Find the angle of the slice, and add on a little to the label's center.

			float xChange, yChange;

			xChange = pieCenter.X - labelCenter.X;
			yChange = pieCenter.Y - labelCenter.Y;

			float angle = (float)Math.Atan2 (xChange, yChange) + (float)(Math.PI / 2);

			// We do the Math.Pi / 2 adjustment because of how the pie chart is drawn internally.

			labelCenter.X = oldLabelCenter.X + EXTRUSION * (float)Math.Cos(angle);
			labelCenter.Y = oldLabelCenter.Y - EXTRUSION * (float)Math.Sin(angle);

			endOfLine.X = oldLabelCenter.X + (EXTRUSION - 30) * (float)Math.Cos (angle);
			endOfLine.Y = oldLabelCenter.Y - (EXTRUSION - 30) * (float)Math.Sin (angle);

			label.Text = (NSString)datapoint.XValue;
			label.SizeToFit ();
			label.Center = labelCenter;
			label.Hidden = false;
			label.TextColor = UIColor.DarkGray;

			_lineView.addPointPair(oldLabelCenter, endOfLine, label);
		}

		protected override void OnRenderStarted (ShinobiChart chart, bool fullRedraw)
		{
			// Position our view over the topf ot the GL canvas.
			RectangleF glFrame = chart.Canvas.Frame;
			glFrame.Y = chart.Canvas.Frame.Y;
			_lineView.Frame = glFrame;
			// Remove the old point pairs from the line view.
			_lineView.reset ();
		}

	}

	public class MyDataSource : SChartDataSource {

		string[] _categories;
		int[] _data;

		public MyDataSource () {
			_categories = new string[3] {"Slice 1", "Slice 2", "Slice 3"};
			_data = new int[3] {3, 9, 6};
		}

		public override int GetNumberOfSeries (ShinobiChart chart)
		{
			return 1;
		}

		public override int GetNumberOfDataPoints (ShinobiChart chart, int dataSeriesIndex)
		{
			return 3;
		}

		public override SChartSeries GetSeries (ShinobiChart chart, int dataSeriesIndex)
		{
			return new SChartPieSeries ();
		}

		public override SChartData GetDataPoint (ShinobiChart chart, int dataIndex, int dataSeriesIndex)
		{
			SChartDataPoint point = new SChartDataPoint ();

			point.XValue = new NSString (_categories [dataIndex]);
			point.YValue = new NSNumber (_data [dataIndex]);

			return point;
		}
	}
}

