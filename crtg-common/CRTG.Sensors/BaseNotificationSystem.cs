using System;
using System.Collections.Generic;
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

        public virtual void SendReport(string[] recipients, string subject, string message, string[] attachments)
        {
            throw new NotImplementedException();
        }
    }
}
