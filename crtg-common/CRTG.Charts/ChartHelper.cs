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
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Drawing;

namespace CRTG.Charts
{
    public static class ChartHelper
    {
        public static string GetChartImage(BaseSensor sensor, ViewTimeframe viewtime, int width, int height)
        {
            CrtgChart c = new CrtgChart(sensor, viewtime, width, height);
            return c.DrawToTempFile();
        }

        public static Image GetChartImageAsBitmap(BaseSensor sensor, ViewTimeframe viewtime, int width, int height)
        {
            CrtgChart c = new CrtgChart(sensor, viewtime, width, height);
            return c.DrawToImage();
        }

        //public static void SetupChart(BaseSensor sensor, Chart chart, ViewTimeframe viewtime)
        //{
        //    // Figure out what time range we're using
        //    DateTime range_start = DateTime.Now.AddMinutes(-15);
        //    string axis_time_format = "HH:mm:ss";
        //    if (viewtime == ViewTimeframe.Hour) {
        //        range_start = DateTime.Now.AddHours(-1);
        //        axis_time_format = "HH:mm:ss";
        //    } else if (viewtime == ViewTimeframe.Day) {
        //        range_start = DateTime.Now.AddHours(-24);
        //        axis_time_format = "HH:mm:ss";
        //    } else if (viewtime == ViewTimeframe.Week) {
        //        range_start = DateTime.Now.AddDays(-7);
        //        axis_time_format = "yyyy-MM-dd";
        //    } else if (viewtime == ViewTimeframe.Month) {
        //        range_start = DateTime.Now.AddDays(-30);
        //        axis_time_format = "yyyy-MM-dd";
        //    } else if (viewtime == ViewTimeframe.AllTime) {
        //        range_start = DateTime.MinValue;
        //        axis_time_format = "yyyy-MM-dd";
        //    }

        //    // Now update the chart
        //    chart.SuspendLayout();
        //    DataPointCollection dpc = chart.Series[0].Points;
        //    dpc.Clear();
        //    chart.Series[0].Name = sensor.Name;
        //    foreach (SensorData sd in from sd in sensor.SensorDataFile.Data orderby sd.Time ascending select sd) {
        //        if (sd.Time >= range_start) {
        //            DataPoint dp = new DataPoint();
        //            dp.YValues = new double[] { (double)sd.Value };
        //            dp.AxisLabel = sd.Time.ToString(axis_time_format);
        //            dpc.Add(dp);
        //        }
        //    }
        //    chart.ResumeLayout();
        //}
    }
}
