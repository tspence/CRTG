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
using System.IO;
using CRTG.Common;
using CRTG.Common.Attributes;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "File System", Tooltip = "Find a specific folder and report on facts about that folder.")]
    public class FolderSensor : BaseSensor
    {
        [AutoUI(Group = "File")]
        public FolderMeasurement Measurement { get; set; }

        [AutoUI(Group = "File", BrowseFolder = true)]
        public string Path { get; set; }


        #region Implementation
        public override decimal Collect()
        {
            //FileInfo fi = null;

            //// Is this a UNC path?
            //if (Path.StartsWith("\\\\")) {
            //    using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials()) {

            //        // Detect problems connecting
            //        if (!unc.NetUseWithCredentials(Path, Device.WinUsername, Device.WinDomain, Device.WinPassword)) {
            //            throw new Exception("Unable to connect to file server with supplied credentials.");
            //        }

            //        // Retrieve information about this file
            //        fi = new FileInfo(Path);
            //    }

            //    // Okay, this is local, just connect to the file directly
            //} else {
            //    fi = new FileInfo(Path);
            //}

            //// Retrieve information for this file
            //TimeSpan ts;
            //switch (Measurement) {
            //    case FileMeasurement.FileSizeBytes:
            //        return (decimal)fi.Length;
            //    case FileMeasurement.MinutesSinceChanged:
            //        ts = DateTime.UtcNow - fi.LastWriteTimeUtc;
            //        return (decimal)ts.TotalMinutes;
            //    case FileMeasurement.MinutesSinceCreated:
            //        ts = DateTime.UtcNow - fi.CreationTimeUtc;
            //        return (decimal)ts.TotalMinutes;
            //}

            // Failed
            return 0;
        }
        #endregion

        #region Icon
        public override string GetNormalIconPath()
        {
            return "Resources/file.png";
        }
        #endregion
    }
}
