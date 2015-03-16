using System;
using System.Collections;
using System.ComponentModel;
using System.Net;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace IgniterPart.SDK.SystemMock.BindingList
{

    public interface IBindingList : IList, ICollection, IEnumerable
    {

        /*
        object AddNew();
        void AddIndex(PropertyDescriptor property);
        void ApplySort(PropertyDescriptor property, ListSortDirection direction);
        int Find(PropertyDescriptor property, object key);
        void RemoveIndex(PropertyDescriptor property);
        void RemoveSort();
        bool AllowNew { get; }
        bool AllowEdit { get; }
        bool AllowRemove { get; }
        bool SupportsChangeNotification { get; }
        bool SupportsSearching { get; }
        bool SupportsSorting { get; }
        bool IsSorted { get; }
        PropertyDescriptor SortProperty { get; }
        ListSortDirection SortDirection { get; }
         */

        event ListChangedEventHandler ListChanged;
    }

    public delegate void ListChangedEventHandler(object sender, ListChangedEventArgs e);
}
