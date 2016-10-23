using CRTG.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRTG.Common.Data;
using CRTG.Common;

namespace CRTG.Actions.ConditionLibrary
{
    public class ThresholdCondition : BaseCondition
    {
        [AutoUI(Group ="Error Threshold")]
        public decimal? HighError { get; set; }
        [AutoUI(Group = "Error Threshold")]
        public string ErrorMessage { get; set; }
        [AutoUI(Group = "Error Threshold")]
        public decimal? LowError { get; set; }

        [AutoUI(Group = "Warning Threshold")]
        public decimal? HighWarning { get; set; }
        [AutoUI(Group = "Warning Threshold")]
        public string WarningMessage { get; set; }
        [AutoUI(Group = "Warning Threshold")]
        public decimal? LowWarning { get; set; }

        private ErrorState CurrentState { get; set; }

        public override void TestCondition(SensorCollectEventArgs args)
        {
            // Determine the state we are in
            ErrorState newState = ErrorState.Normal;
            if (args.Data == null) {
                newState = ErrorState.Exception;
            } else {
                if (args.Data.Value < LowError) {
                    newState = ErrorState.ErrorLow;
                } else if (args.Data.Value > HighError) {
                    newState = ErrorState.ErrorHigh;
                } else if (args.Data.Value < LowWarning) {
                    newState = ErrorState.WarningLow;
                } else if (args.Data.Value > HighWarning) {
                    newState = ErrorState.WarningHigh;
                } else {
                    newState = ErrorState.Normal;
                }
            }

            // If the state has changed, notify
            if (newState != CurrentState) {
                args.ConditionMessage = String.Format("{0}: {1} is now {2}", newState, Sensor.Name, args.Data.Value);
                TriggerActions(args);
            }
            CurrentState = newState;
        }
    }
}
