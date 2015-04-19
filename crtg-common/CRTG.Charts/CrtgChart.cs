/*
 * 2012 - 2015 Ted Spence, http://tedspence.com
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
using CRTG.Sensors.Data;

namespace CRTG.Charts
{
    public class CrtgChart
    {
        private SensorData[] _raw_data;
        private int _width, _height;
        private DateTime _min_date, _max_date;
        private decimal _min_value, _max_value;
        private double _range_in_seconds, _range_in_value;
        private Rectangle _chart_rect;

        public CrtgChart(BaseSensor sensor, ViewTimeframe viewtime, int width, int height)
        {
            _raw_data = new SensorData[] { };
            if (sensor != null && sensor.SensorDataFile != null) {
                _raw_data = sensor.SensorDataFile.Data;
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
                _min_date = (from r in _raw_data select r.Time).Min();
            } else {
                _min_date = _max_date.AddMinutes(-(int)viewtime);
            }
            
            // Filter data by time
            _raw_data = (from r in _raw_data where r.Time > _min_date && r.Time < _max_date orderby r.Time ascending select r).ToArray();
            if (_raw_data.Length > 0) {

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

        public string DrawToTempFile()
        {
            Image i = DrawToImage();
            string tempfn = Path.ChangeExtension(Path.GetTempFileName(), "png");
            i.Save(tempfn, System.Drawing.Imaging.ImageFormat.Png);
            i.Dispose();
            return tempfn;
        }

        public Image DrawToImage()
        {
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
            return bmp;
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
                string time;
                //if (SensorProject.Current.TimeZonePreference == DateTimePreference.LocalTime) {
                //    dtpos = dtpos.ToLocalTime();
                //    time = dtpos.ToString("HH:mm:ss") + " Local Time";
                //} else {
                    time = dtpos.ToString("HH:mm:ss") + " UTC";
                //}
                string date = dtpos.ToString("yyyy-MM-dd");
                var r = g.MeasureString(time, graph_label);
                g.DrawString(time, graph_label, tb, xpos, _chart_rect.Bottom - r.Height);
                if (current_date != date) {
                    g.DrawString(date, graph_label, tb, xpos, _chart_rect.Bottom - r.Height - r.Height);
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

        private int ChooseTimeScale(out string date_time_format)
        {
            date_time_format = null;

            // Ideally, we'd like a vertical line every 200 pixels
            int vertical_lines = (int)(_chart_rect.Width / 200.0d);
            double base_scale = _range_in_seconds / vertical_lines;

            // What magnitude makes sense?  First, let's try a few seconds-based times.
            date_time_format = "HH:mm:ss";
            if (base_scale <= 1) {
                return 1;
            } else if (base_scale <= 10) {
                return 10;
            } else if (base_scale <= 30) {
                return 30;
            } else if (base_scale <= 60) {
                return 60;
            } else if (base_scale <= 120) { // 2 minutes
                return 120;
            } else if (base_scale <= 300) { // 5 minutes
                return 300;
            } else if (base_scale <= 600) { // 10 minutes
                return 600;
            } else if (base_scale <= 900) { // 15 minutes
                return 900;
            } else if (base_scale <= 1200) { // 20 minutes
                return 1200;
            } else if (base_scale <= 1800) { // 30 minutes
                return 1800;
            } else if (base_scale <= 3600) { // 1 hour
                return 3600;
            } else if (base_scale <= 7200) { // 2 hour
                return 7200;
            } else if (base_scale <= 14400) { // 4 hour
                return 14400;
            }

            // From here on out, formatting is best done in days
            date_time_format = "yyyy-MM-dd";
            if (base_scale <= 43200) { // 12 hour
                return 43200;
            } else if (base_scale <= 86400) { // 1 day
                return 86400;
            } else if (base_scale <= 172800) { // 2 days
                return 172800;
            } else { // 1 week
                return 604800;
            } 
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
            if (_raw_data.Length > 0) {
                Pen blue = new Pen(Color.Blue, 2.5f);
                blue.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
                Point last_point = PlotPoint(_raw_data[0]);
                for (int i = 1; i < _raw_data.Length; i++) {
                    Point p = PlotPoint(_raw_data[i]);
                    g.DrawLine(blue, last_point, p);
                    last_point = p;
                }
            }
        }
    }
}
