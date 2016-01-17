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
using CRTG.Common;

namespace CRTG.Charts
{
    public static class ChartHelper
    {
        /// <summary>
        /// Retrieve a full display package for this selected sensor and timeframe and chart size
        /// </summary>
        /// <param name="SelectedSensor"></param>
        /// <param name="vt"></param>
        /// <param name="chart_width"></param>
        /// <param name="chart_height"></param>
        /// <returns></returns>
        public static CrtgChart GetDisplayPackage(ISensor sensor, ViewTimeframe viewtime, int width, int height)
        {
            return new CrtgChart(sensor, viewtime, width, height);
        }
    }
}
