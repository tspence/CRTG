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

namespace CRTG
{
    /// <summary>
    /// Converts to an integer number of seconds
    /// </summary>
    public enum Interval 
    { 
        Never = 0,
        ThirtySeconds = 30, 
        Minute = 60, 
        FiveMinutes = 5 * 60, 
        TenMinutes = 10 * 60, 
        FifteenMinutes = 15 * 60, 
        ThirtyMinutes = 30 * 60, 
        Hour = 60 * 60, 
        TwoHours = 2 * 60 * 60, 
        FourHours = 4 * 60 * 60, 
        SixHours = 6 * 60 * 60,
        TwelveHours = 12 * 60 * 60,
        Day = 24 * 60 * 60,
        Week = 7 * 24 * 60 * 60,
        Fortnight = 14 * 24 * 60 * 60,
        Month = 30 * 24 * 60 * 60,
        Quarter = 90 * 24 * 60 * 60,
    };

    public enum Severity
    {
        Information = 0,
        Warning = 1,
        Error = 2,
        Critical = 3,
    };

    public enum Operations
    {
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    };

    public enum DeviceType
    {
        NotSelected = 0,
        Windows = 1,
        Linux = 2,
        SNMP = 3,
    }

    public enum NotificationMethod
    {
        None = 0,
        Email = 1,
    }

    public enum NotificationState
    {
        Normal = 0,
        ErrorHigh = 1,
        ErrorLow = 2,
        WarningHigh = 3,
        WarningLow = 4,
        ValueChanged = 5,
    }

    public enum SqlCollectionType
    {
        ScalarValue = 0,
        RecordCount = 1,
    }

    public enum ViewTimeframe
    {
        FifteenMinutes = 15,
        Hour = 60,
        TwoHours = 2 * 60,
        FourHours = 4 * 60,
        TwelveHours = 12 * 60,
        Day = 24 * 60,
        TwoDays = 2 * 24 * 60,
        Week = 7 * 24 * 60,
        TwoWeeks = 2 * 7 * 24 * 60,
        Month = 30 * 24 * 60,
        Quarter = 3 * 30 * 24 * 60,
        Year = 365 * 24 * 60,
        TwoYears = 2 * 365 * 24 * 60,
        AllTime = 0,
    }

    public enum BasicWmiQuery
    {
        CPU = 1,
        Memory = 2,
        Network = 3,
        Disk = 4,
    }

    public enum RequestMethod
    {
        GET = 0,
        HEAD = 1,
        POST = 2,
    }

    public enum DateTimePreference
    {
        UTC = 0,
        LocalTime = 1,
    }

    public enum ReportFileFormat
    {
        CSV = 0,
        OpenXML = 1,
        InlineHtml = 2,
        ExcelWithTables = 3,
    }

    public enum DiskMeasurement
    {
        MegabytesFree = 0,
        MegabytesUsed = 1,
        PercentFree = 2,
        PercentUsed = 3,
        BytesFree = 4,
        BytesUsed = 5,
    }

    public enum MemoryMeasurement
    {
        MegabytesFree = 0,
        MegabytesUsed = 1,
    }

    public enum FileMeasurement
    {
        FileSizeBytes = 0,
        MinutesSinceChanged = 1,
        MinutesSinceCreated = 2,
    }

    public enum FolderMeasurement
    {
        FileCount = 0,
        TotalFileSizeBytes = 1,
        MinutesSinceLastFileChanged = 2,
        MinutesSinceLastFileCreated = 3,
    }

    public enum XmlObjectMeasurement
    {
        DateDiffFromUTC = 0,
        DateDiffFromLocal = 1,
        Decimal = 2,
    }
}
