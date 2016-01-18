using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common
{
    public interface ISensor
    {
        int Identity { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string MeasurementUnit { get; set; }
        Interval Frequency { get; set; }
        bool PauseOnError { get; set; }
        bool Enabled { get; set; }
        SensorDataCollection SensorData { get; set; }
        Decimal LatestData { get; set; }
        DateTime NextCollectTime { get; set; }
        DateTime LastCollectTime { get; set; }
        bool InFlight { get; set; }
        IDevice Device { get; set; }
        bool InError { get; set; }
        string LastException { get; set; }
        decimal? HighError { get; set; }
        string ErrorMessage { get; set; }
        decimal? LowError { get; set; }
        decimal? HighWarning { get; set; }
        string WarningMessage { get; set; }
        decimal? LowWarning { get; set; }
        bool NotifyOnChange { get; set; }
        NotificationMethod Method { get; set; }
        string Recipients { get; set; }
        string KlipfolioId { get; set; }
        ViewTimeframe UploadAmount { get; set; }
        DateTime LastUploadTime { get; set; }
    }
}
