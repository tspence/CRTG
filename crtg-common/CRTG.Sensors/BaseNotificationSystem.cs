/*
 * 2012 - 2015 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using CRTG.Sensors.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Sensors
{
    public class BaseNotificationSystem
    {
        public virtual void Notify(BaseSensor baseSensor, NotificationState ns, DateTime timestamp, decimal value, string s)
        {
            throw new NotImplementedException();
        }

        public virtual void SendReport(string[] recipients, string subject, string message, DataTable report_data, string attachment_filename)
        {
            throw new NotImplementedException();
        }
    }
}
