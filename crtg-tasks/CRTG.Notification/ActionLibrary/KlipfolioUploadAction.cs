using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRTG.Common.Data;
using CRTG.Common.Interfaces;
using CRTG.Common;
using CRTG.Common.Attributes;

namespace CRTG.Actions.ActionLibrary
{
    public class KlipfolioUploadAction : BaseAction
    {
        [AutoUI]
        public ViewTimeframe UploadDataWindow { get; set; }

        [AutoUI]
        public string KlipfolioId { get; set; }

        [AutoUI(Skip=true)]
        public DateTime LastUploadTime { get; set; }


        public override void Execute(IDataStore datastore, ISensor sensor, ICondition condition, SensorCollectEventArgs args)
        {
            if (!String.IsNullOrEmpty(KlipfolioId)) {
                try {
                    TimeSpan ts = DateTime.UtcNow - LastUploadTime;
                    if (args.Data != null) {

                        // Filter to just the amount we care about
                        DateTime end = DateTime.UtcNow;
                        DateTime start = DateTime.MinValue;
                        if (UploadDataWindow != ViewTimeframe.AllTime) {
                            start = end.AddMinutes(-(int)UploadDataWindow);
                        }
                        var list = datastore.RetrieveData(sensor, start, end, false);

                        // Determine the correct URL
                        string UploadUrl = String.Format("https://app.klipfolio.com/api/1/datasources/{0}/data", KlipfolioId);
                        if (SensorProject.Current.Notifications.UploadReport<SensorData>(list.Data, false, UploadUrl, HttpVerb.PUT,
                            SensorProject.Current.KlipfolioUsername, SensorProject.Current.KlipfolioPassword)) {
                            LastUploadTime = DateTime.UtcNow;
                        }
                    }

                // Catch problems in uploading
                } catch (Exception ex) {
                    string headline = String.Format("Error uploading {0} ({1}) to Klipfolio", this.Name, this.Identity);
                    SensorProject.LogException(headline, ex);
                }
            }
        }
    }
}
