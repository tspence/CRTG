using CRTG.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common
{
    /// <summary>
    /// Shortcut implementation to avoid having to implement this multiple times
    /// </summary>
    public class BaseSensorTreeModel : ISensorTreeModel, INotifyPropertyChanged
    {
        /// <summary>
        /// The name of this item.  Renameable.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                Notify("Name");
            }
        }

        /// <summary>
        /// List of children
        /// </summary>
        public ObservableCollection<ISensorTreeModel> Children { get; private set; }

        /// <summary>
        /// Remembers the parent of this item.  Do not serialize this object!
        /// </summary>
        [JsonIgnore]
        public ISensorTreeModel Parent { get; set; }

        /// <summary>
        /// Remembers whether this item is expanded
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }

            set
            {
                _isExpanded = value;
                if (Parent != null) Parent.IsExpanded = value;
                Notify("IsExpanded");
            }
        }

        /// <summary>
        /// Remembers whether this item is selected
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
                if (value == true && Parent != null) Parent.IsExpanded = true;
                Notify("IsSelected");
            }
        }
        /// <summary>
        /// By default, everyone gets a project folder icon
        /// </summary>
        public virtual string IconPath
        {
            get
            {
                return "Resources/project.png";
            }
        }


        #region Private values
        private string _name;
        private bool _isExpanded;
        private bool _isSelected;
        #endregion

        #region Property Change Notifications
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify changes
        /// </summary>
        /// <param name="property"></param>
        public void Notify(string property)
        {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Interface for adding a child
        /// </summary>
        /// <param name="child"></param>
        public virtual void AddChild(ISensorTreeModel child)
        {
            Children.Add(child);
            Notify("Children");
        }

        /// <summary>
        /// Interface for removing a child
        /// </summary>
        /// <param name="child"></param>
        public virtual void RemoveChild(ISensorTreeModel child)
        {
            Children.Remove(child);
            Notify("Children");
        }
        #endregion

        #region Constructor
        public BaseSensorTreeModel()
        {
            this.Children = new ObservableCollection<ISensorTreeModel>();
            this.Name = "New " + this.GetType().Name;
            IsExpanded = true;
            IsSelected = false;
        }
        #endregion
    }
}
