/*
 * 2012 - 2016 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using CRTG.Common;
using CRTG.Common.Interfaces;

namespace CRTG.Charts
{
    public class CrtgChart : IDisposable
    {
        private List<SensorData> _raw_data;
        private int _width, _height;
        private DateTime _min_date, _max_date;
        private decimal _min_value, _max_value;
        private double _range_in_seconds, _range_in_value;
        private Rectangle _chart_rect;
        private Image _chart_image;

        public CrtgChart(ISensor sensor, SensorDataCollection data, ViewTimeframe viewtime, int width, int height)
        {
            _raw_data = new List<SensorData>();
            if (data != null) {
                _raw_data = data.Data.ToList();
            }
            _width = width;
            _height = height;
            _max_date = DateTime.UtcNow;
            _min_date = _max_date;
            _chart_rect = new Rectangle() { X = 5, Y = 5, Width = width - 10, Height = height - 10 };
            _min_value = decimal.MaxValue;
            _max_value = decimal.MinValue;

            // If there's an artificial limitation on the time window, use that instead
            if (viewtime == ViewTimeframe.AllTime) {
                if (_raw_data.Count > 0) {
                    _min_date = (from r in _raw_data select r.Time).Min();
                }
            } else {
                _min_date = _max_date.AddMinutes(-(int)viewtime);
            }
            
            // Filter data by time
            _raw_data = (from r in _raw_data where r.Time > _min_date && r.Time < _max_date orderby r.Time ascending select r).ToList();
            if (_raw_data.Any()) {

                // Range checks and clamping
                _min_value = (from r in _raw_data select r.Value).Min();
                _max_value = (from r in _raw_data select r.Value).Max();
                if (_min_value > 0) _min_value = 0;
                if (_max_value < 0) _max_value = 0;

                // Increase the maximum slightly so we can see the peak
                if (_max_value > 0m) {
                    _max_value = _max_value * 1.03m;
                }

                // Keep track of ranges
                _range_in_seconds = (double)(_max_date - _min_date).TotalSeconds;
                _range_in_value = (double)(_max_value - _min_value);
            }
        }

        #region Rendering Chart
        private Point PlotPoint(SensorData sensorData)
        {
            return new Point(GetTimePosition(sensorData.Time), GetValuePosition((double)sensorData.Value));
        }

        private int GetValuePosition(double d)
        {
            double ypos = d / _range_in_value;
            return (int)(_chart_rect.Y + _chart_rect.Height - (ypos * _chart_rect.Height));
        }

        private int GetTimePosition(DateTime time)
        {
            TimeSpan ts = time - _min_date;
            double xpos = ts.TotalSeconds / _range_in_seconds;
            return (int)(_chart_rect.X + (xpos * _chart_rect.Width));
        }

        //public string DrawToTempFile()
        //{
        //    Image i = DrawToImage();
        //    string tempfn = Path.ChangeExtension(Path.GetTempFileName(), "png");
        //    i.Save(tempfn, System.Drawing.Imaging.ImageFormat.Png);
        //    i.Dispose();
        //    return tempfn;
        //}

        private void DrawToImage()
        {
            if (_width <= 0 && _height <= 0) return;

            // New version of drawing code does it all ourselves
            Bitmap bmp = new Bitmap(_width, _height);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // We have a 5px border around each edge, and remember the positions
            Pen black = new Pen(Color.Black);
            g.DrawRectangle(black, _chart_rect);

            // Only draw something if values make sense
            if (_range_in_value > 0 && _range_in_seconds > 0) {
                DrawGraphLines(g);
                DrawSensorData(g);
            }

            // Save this to a temporary filename
            g.Dispose();
            _chart_image = bmp;
        }

        private void DrawGraphLines(Graphics g)
        {
            Pen graphline = new Pen(Color.DarkSlateGray);
            graphline.DashPattern = new float[] { 4, 4 };
            Brush tb = new SolidBrush(Color.Black);
            Font graph_label = new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Regular);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Calculate number of grid lines to draw horizontally
            int gridline_scale = ChooseScale();
            int pos = gridline_scale;
            while (pos < _range_in_value) {

                // Draw a dotted line at this position
                int ypos = GetValuePosition(pos);
                g.DrawLine(graphline, _chart_rect.Left, ypos, _chart_rect.Right, ypos);

                // Label the line
                g.DrawString(pos.ToString("#,##0"), graph_label, tb, _chart_rect.Left, ypos);

                // Move on to next line
                pos += gridline_scale;
            }

            // Now let's do some vertical gridlines
            string date_time_format = null;
            int time_scale_in_seconds = ChooseTimeScale(out date_time_format);
            DateTime dtpos = _min_date.AddSeconds(time_scale_in_seconds);
            dtpos = RoundDateTime(dtpos, time_scale_in_seconds);
            string current_date = null;
            while (dtpos < _max_date) {

                // Draw a dotted line at this position
                int xpos = GetTimePosition(dtpos);
                g.DrawLine(graphline, xpos, _chart_rect.Top, xpos, _chart_rect.Bottom);

                // Label the line, based on whether the user wants to see local time or UTC
                string time_utc = dtpos.ToString("HH:mm:ss") + " UTC";
                string time_local = dtpos.ToLocalTime().ToString("HH:mm:ss") + " Local";
                string date = dtpos.ToString("yyyy-MM-dd");
                var r = g.MeasureString(time_utc, graph_label);
                g.DrawString(time_local, graph_label, tb, xpos, _chart_rect.Bottom - r.Height);
                g.DrawString(time_utc, graph_label, tb, xpos, _chart_rect.Bottom - r.Height - r.Height);
                if (current_date != date) {
                    g.DrawString(date, graph_label, tb, xpos, _chart_rect.Bottom - r.Height - r.Height - r.Height);
                    current_date = date;
                }

                // Move on to next line
                dtpos = dtpos.AddSeconds(time_scale_in_seconds);
            }
        }

        private DateTime RoundDateTime(DateTime dtpos, int time_scale_in_seconds)
        {
            TimeSpan span = new TimeSpan(0, 0, time_scale_in_seconds);
            long ticks = dtpos.Ticks / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }

        /// <summary>
        /// This list describes base scales in terms of seconds
        /// </summary>
        private static int[] TIME_OF_DAY_SCALE_VALUES = new int[] { 
            10, 30, 60, // seconds
            60 * 2, 60 * 5, 60 * 10, 60 * 15, 60 * 20, 60 * 30, // 2 minutes - 30 minutes
            3600, 3600 * 2, 3600 * 4, 3600 * 12, // 1 hour - 12 hours
        };

        /// <summary>
        /// This list describes base scales in terms of days
        /// </summary>
        private static int[] DAY_OF_YEAR_SCALE_VALUES = new int[] { 
            86400, 86400 * 4, // 1 day  - 4 days
            604800 * 2, // 2 weeks
            2592000, 2592000 * 6, // 1 month - 6 months
        };

        private int ChooseTimeScale(out string date_time_format)
        {
            date_time_format = null;

            // Ideally, we'd like a vertical line every 200 pixels
            int vertical_lines = (int)(_chart_rect.Width / 200.0d);
            double base_scale = _range_in_seconds / vertical_lines;

            // What magnitude makes sense?  First, let's try a few seconds-based times.
            date_time_format = "HH:mm:ss";
            foreach (var scale in TIME_OF_DAY_SCALE_VALUES) {
                if (base_scale <= scale) {
                    return scale;
                }
            }

            // From here on out, formatting is best done in days
            date_time_format = "yyyy-MM-dd";
            foreach (var scale in DAY_OF_YEAR_SCALE_VALUES) {
                if (base_scale <= scale) {
                    return scale;
                }
            }

            // Return maximum 1 year scale (365.25 days in seconds)
            return 31557600;
        }

        private int ChooseScale()
        {
            // How many lines should we have?  At least one line every 40 pixels...
            int horizontal_lines = (int)(_chart_rect.Height / 40.0d);

            // Now that we know how many lines we want to have, what scale corresponds to that?
            double val = _range_in_value / (horizontal_lines + 1);
            
            // Now, let's make the scale human friendly...
            int magnitude = 1;
            while (true) {
                int scale = (int)val / magnitude;
                if (scale <= 1) {
                    return 1 * magnitude;
                } else if (scale <= 2) {
                    return 2 * magnitude;
                } else if (scale <= 5) {
                    return 5 * magnitude;
                } else if (scale <= 10) {
                    return 10 * magnitude;
                } else if (scale <= 20) {
                    return 20 * magnitude;
                } else if (scale <= 50) {
                    return 50 * magnitude;
                } else if (scale <= 100) {
                    return 100 * magnitude;
                } else if (scale <= 200) {
                    return 200 * magnitude;
                } else if (scale <= 500) {
                    return 500 * magnitude;
                }

                // Scale up
                magnitude *= 1000;
            }
        }

        /// <summary>
        /// Draw all data from the sensors
        /// </summary>
        /// <param name="g"></param>
        private void DrawSensorData(Graphics g)
        {
            if (_raw_data.Count > 0) {
                Pen blue = new Pen(Color.Blue, 2.5f);
                blue.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
                Point last_point = PlotPoint(_raw_data[0]);
                for (int i = 1; i < _raw_data.Count; i++) {
                    Point p = PlotPoint(_raw_data[i]);
                    g.DrawLine(blue, last_point, p);
                    last_point = p;
                }
            }
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Keep track of the image
        /// </summary>
        public Image ChartImage
        {
            get
            {
                // Do we need to produce the chart image?
                if (_chart_image == null) {
                    DrawToImage();
                }
                return _chart_image;
            }
        }

        /// <summary>
        /// Save the chart to a file
        /// </summary>
        /// <param name="png_filename"></param>
        public void SaveToFile(string png_filename)
        {
            ChartImage.Save(png_filename, System.Drawing.Imaging.ImageFormat.Png);
        }

        /// <summary>
        /// Data used to build the chart
        /// </summary>
        public List<SensorData> RawData
        {
            get
            {
                return _raw_data;
            }
        }

        /// <summary>
        /// Determine the most useful information to show
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public string TooltipFromPoint(float horizontal_distance)
        {
            if (_raw_data != null && _raw_data.Count > 0) {

                // Figure out what time point this distance represents
                double num_seconds_from_chart_left_side = (_range_in_seconds * horizontal_distance);
                DateTime best_available_time = _min_date.AddSeconds((int)num_seconds_from_chart_left_side);

                // Find the best available data point
                SensorData sd = null;
                foreach (var r in _raw_data) {
                    if (r.Time > best_available_time) break;
                    sd = r;
                }
                if (sd != null) {
                    return String.Format("{0} UTC\r\n{1} Local\r\nMeasurement: {2}", sd.Time, sd.Time.ToLocalTime(), sd.Value);
                }
            }
            return "";
        }
        #endregion

        #region Cleanup
        /// <summary>
        /// Ensure safe cleanup
        /// </summary>
        public void Dispose()
        {
            if (_chart_image != null) _chart_image.Dispose();
        }
        #endregion
    }
}
