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
using System.Threading.Tasks;

namespace CRTG.Common.Attributes
{
    public class AutoUI : Attribute
    {
        /// <summary>
        /// The conceptual group that categorizes this element.  Related elements are grouped together in the UI.
        /// </summary>
        public string Group;

        /// <summary>
        /// The prompt given to the user to explain this element.
        /// </summary>
        public string Label;

        /// <summary>
        /// If the user hovers the mouse over this element, show this help string.
        /// </summary>
        public string Help;

        /// <summary>
        /// If this string represents a file that can be "browsed" to
        /// </summary>
        public bool BrowseFile = false;

        /// <summary>
        /// If this string represents a folder that can be "browsed" to
        /// </summary>
        public bool BrowseFolder = false;

        /// <summary>
        /// To generate a multiline text box, set this value to something greater than 1
        /// </summary>
        public int MultiLine = 1;

        /// <summary>
        /// Set this to "read only" if the value should not be edited in the UI
        /// </summary>
        public bool ReadOnly = false;

        /// <summary>
        /// Set this value to true if you wish this item to be skipped from the UI
        /// </summary>
        public bool Skip = false;

        /// <summary>
        /// If true, hides the keystrokes in this field and turns it into a password edit box
        /// </summary>
        public bool PasswordField = false;

        /// <summary>
        /// If you want syntax highlighting for the editor, specify a language here
        /// </summary>
        public string EditorLanguage = null;
    }
}
