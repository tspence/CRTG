using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common.Interfaces
{
    public interface ISensorTreeModel
    {
        /// <summary>
        /// The name of this item
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The ID number of this item
        /// </summary>
        int Identity { get; set; }

        /// <summary>
        /// The icon of this item (controlled by the item's type, non-changeable)
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// Remembers whether this item is expanded in the treeview
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Remembers whether this item is selected in the treeview
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Identifies the parent, so that nested actions can propagate upwards
        /// </summary>
        ISensorTreeModel Parent { get; set; }

        /// <summary>
        /// Identifies the children of this item, if any
        /// </summary>
        ObservableCollection<ISensorTreeModel> Children { get; }

        /// <summary>
        /// Adds one child and handles notifications
        /// </summary>
        /// <param name="child"></param>
        void AddChild(ISensorTreeModel child);

        /// <summary>
        /// Removes one child and handles notifications
        /// </summary>
        /// <param name="child"></param>
        void RemoveChild(ISensorTreeModel child);
    }
}
