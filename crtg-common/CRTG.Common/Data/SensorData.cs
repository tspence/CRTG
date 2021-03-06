﻿/*
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

namespace CRTG.Common
{
    public class SensorData
    {
        public DateTime Time { get; set; }
        public decimal Value { get; set; }
        public int CollectionTimeMs { get; set; }
    }
}
