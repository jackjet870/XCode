﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using NewLife.Reflection;
using XCode.Configuration;

namespace XCode
{
    public partial class EntityList<T> : IListSource, ITypedList, IBindingList, IBindingListView, ICancelAddNew
    {
        #region IListSource接口
        bool IListSource.ContainsListCollection
        {
            get { return false; }
        }

        IList IListSource.GetList()
        {
            // 如果是接口，创建新的集合，否则返回自身
            if (!typeof(T).IsInterface) return this;

            if (Count < 1) return null;

            return ToArray(null);
        }
        #endregion

        #region 复制
        IList ToArray(Type type)
        {
            if (Count < 1) return null;

            // 元素类型
            if (type == null) type = this[0].GetType();
            // 泛型
            type = typeof(EntityList<>).MakeGenericType(type);

            // 初始化集合，实际上是创建了一个真正的实体类型
            IList list = TypeX.CreateInstance(type) as IList;
            for (int i = 0; i < Count; i++)
            {
                list.Add(this[i]);
            }

            return list;
        }
        #endregion

        #region ITypedList接口
        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            Type type = EntityType;
            // 调用TypeDescriptor获取属性
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(type);
            if (pdc == null || pdc.Count <= 0) return pdc;

            return EntityBase.Fix(type, pdc);

            //// 准备字段集合
            //Dictionary<String, FieldItem> dic = new Dictionary<string, FieldItem>();
            ////factory.Fields.ForEach(item => dic.Add(item.Name, item));
            //foreach (FieldItem item in Factory.Fields)
            //{
            //    dic.Add(item.Name, item);
            //}

            //List<PropertyDescriptor> list = new List<PropertyDescriptor>();
            //foreach (PropertyDescriptor item in pdc)
            //{
            //    // 显示名与属性名相同，并且没有DisplayName特性
            //    if (item.Name == item.DisplayName && !ContainAttribute(item.Attributes, typeof(DisplayNameAttribute)))
            //    {
            //        // 添加一个特性
            //        FieldItem fi = null;
            //        if (dic.TryGetValue(item.Name, out fi) && !String.IsNullOrEmpty(fi.DisplayName))
            //        {
            //            DisplayNameAttribute dis = new DisplayNameAttribute(fi.DisplayName);
            //            list.Add(TypeDescriptor.CreateProperty(type, item, dis));
            //            continue;
            //        }
            //    }
            //    list.Add(item);
            //}
            //pdc = new PropertyDescriptorCollection(list.ToArray());

            //return pdc;
        }

        //static Boolean ContainAttribute(AttributeCollection attributes, Type type)
        //{
        //    if (attributes == null || attributes.Count < 1 || type == null) return false;

        //    foreach (Attribute item in attributes)
        //    {
        //        if (type.IsAssignableFrom(item.GetType())) return true;
        //    }
        //    return false;
        //}

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return null;
        }

        //class MyPropertyDescriptor : PropertyDescriptor
        //{
        //    #region 重载
        //    PropertyDescriptor pd;

        //    public MyPropertyDescriptor(PropertyDescriptor p)
        //        : base(p)
        //    {
        //        pd = p;
        //        Fix();
        //    }

        //    public override bool CanResetValue(object component)
        //    {
        //        return pd.CanResetValue(component);
        //    }

        //    public override Type ComponentType
        //    {
        //        get { return pd.ComponentType; }
        //    }

        //    public override object GetValue(object component)
        //    {
        //        return pd.GetValue(component);
        //    }

        //    public override bool IsReadOnly
        //    {
        //        get { return pd.IsReadOnly; }
        //    }

        //    public override Type PropertyType
        //    {
        //        get { return pd.PropertyType; }
        //    }

        //    public override void ResetValue(object component)
        //    {
        //        pd.ResetValue(component);
        //    }

        //    public override void SetValue(object component, object value)
        //    {
        //        pd.SetValue(component, value);
        //    }

        //    public override bool ShouldSerializeValue(object component)
        //    {
        //        return pd.ShouldSerializeValue(component);
        //    }
        //    #endregion

        //    #region 改写
        //    private String _Category;
        //    /// <summary>类别</summary>
        //    public override String Category
        //    {
        //        get { return _Category ?? base.Category; }
        //        //set { _Category = value; }
        //    }

        //    private String _DisplayName;
        //    /// <summary>显示名</summary>
        //    public override String DisplayName
        //    {
        //        get { return _DisplayName ?? base.DisplayName; }
        //        //set { _DisplayName = value; }
        //    }

        //    static DescriptionAttribute emptyDes = new DescriptionAttribute();
        //    static DisplayNameAttribute emptyDis = new DisplayNameAttribute();
        //    static BindColumnAttribute emptyBind = new BindColumnAttribute();

        //    void Fix()
        //    {
        //        BindColumnAttribute bc = pd.Attributes[typeof(BindColumnAttribute)] as BindColumnAttribute;

        //        // 显示名和属性名相同、没有DisplayName特性、有Description特性
        //        if (pd.DisplayName == pd.Name && !pd.Attributes.Contains(emptyDis))
        //        {
        //            DescriptionAttribute des = pd.Attributes[typeof(DescriptionAttribute)] as DescriptionAttribute;
        //            if (des != null)
        //            {
        //                if (!String.IsNullOrEmpty(bc.Description)) _DisplayName = des.Description;
        //            }
        //            if (pd.DisplayName == pd.Name && bc != null)
        //            {
        //                if (!String.IsNullOrEmpty(bc.Description)) _DisplayName = bc.Description;
        //            }
        //        }
        //    }
        //    #endregion
        //}
        #endregion

        #region IBindingList接口
        #region 属性
        Boolean _AllowEdit = true;
        /// <summary>获取是否可更新列表中的项。</summary>
        bool IBindingList.AllowEdit { get { return _AllowEdit; } }
        bool AllowEdit
        {
            get { return _AllowEdit; }
            set { if (_AllowEdit != value) { _AllowEdit = value; OnListChanged(ResetEventArgs); }; }
        }

        Boolean _AllowNew = true;
        /// <summary>获取是否可以使用 System.ComponentModel.IBindingList.AddNew() 向列表中添加项。</summary>
        bool IBindingList.AllowNew { get { return _AllowNew; } }
        bool AllowNew
        {
            get { return _AllowNew; }
            set { if (_AllowNew != value) { _AllowNew = value; OnListChanged(ResetEventArgs); }; }
        }

        Boolean _AllowRemove = true;
        /// <summary>获取是否可以使用 System.Collections.IList.Remove(System.Object) 或 System.Collections.IList.RemoveAt(System.Int32)从列表中移除项。</summary>
        bool IBindingList.AllowRemove { get { return _AllowRemove; } }
        bool AllowDelete
        {
            get { return _AllowRemove; }
            set { if (_AllowRemove != value) { _AllowRemove = value; OnListChanged(ResetEventArgs); }; }
        }

        Boolean _IsSorted = false;
        /// <summary>获取是否对列表中的项进行排序。</summary>
        bool IBindingList.IsSorted { get { return _IsSorted; } }
        bool IsSorted
        {
            get { return _IsSorted; }
            set { if (_IsSorted != value) { _IsSorted = value; OnListChanged(ResetEventArgs); }; }
        }

        ListSortDirection _SortDirection;
        ListSortDirection IBindingList.SortDirection { get { return _SortDirection; } }
        /// <summary>获取排序的方向。</summary>
        ListSortDirection SortDirection
        {
            get { return _SortDirection; }
            set { if (_SortDirection != value) { _SortDirection = value; OnListChanged(ResetEventArgs); }; }
        }

        PropertyDescriptor _SortProperty;
        PropertyDescriptor IBindingList.SortProperty { get { return _SortProperty; } }
        /// <summary>获取正在用于排序的 System.ComponentModel.PropertyDescriptor。</summary>
        PropertyDescriptor SortProperty
        {
            get { return _SortProperty; }
            set { if (_SortProperty != value) { _SortProperty = value; OnListChanged(ResetEventArgs); }; }
        }

        bool IBindingList.SupportsChangeNotification
        {
            get { return true; }
        }

        bool IBindingList.SupportsSearching
        {
            get { return true; }
        }

        bool IBindingList.SupportsSorting
        {
            get { return true; }
        }
        #endregion

        #region 事件
        event ListChangedEventHandler _ListChanged;
        event ListChangedEventHandler IBindingList.ListChanged
        {
            add { _ListChanged += value; }
            remove { _ListChanged -= value; }
        }

        static ListChangedEventArgs ResetEventArgs = new ListChangedEventArgs(ListChangedType.Reset, -1);

        void OnListChanged(ListChangedEventArgs e)
        {
            //DataColumn dataColumn = null;
            //string propName = null;
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemMoved:
                case ListChangedType.ItemChanged:
                    if (0 <= e.NewIndex)
                    {
                        //DataRow row = this.GetRow(e.NewIndex);
                        //if (row.HasPropertyChanged)
                        //{
                        //    dataColumn = row.LastChangedColumn;
                        //    propName = (dataColumn != null) ? dataColumn.ColumnName : string.Empty;
                        //}
                        //row.ResetLastChangedColumn();
                    }
                    break;
            }
            if (_ListChanged != null)
            {
                //if ((dataColumn != null) && (e.NewIndex == e.OldIndex))
                //{
                //    //ListChangedEventArgs args = new ListChangedEventArgs(e.ListChangedType, e.NewIndex, new DataColumnPropertyDescriptor(dataColumn));
                //    //_ListChanged(this, args);
                //}
                //else
                //{
                //    _ListChanged(this, e);
                //}
            }
            //if (propName != null)
            //{
            //    this[e.NewIndex].RaisePropertyChangedEvent(propName);
            //}
        }
        #endregion

        #region 方法
        void IBindingList.AddIndex(PropertyDescriptor property)
        {
        }

        object IBindingList.AddNew()
        {
            T entity = (T)Factory.Create();
            base.Add(entity);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, base.IndexOf(entity)));
            return entity;
        }

        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            Sort(property.Name, direction == ListSortDirection.Descending);

            IsSorted = true;
            SortProperty = property;
            SortDirection = direction;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            return FindIndex(item => Object.Equals(item[property.Name], key));
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
        }

        void IBindingList.RemoveSort()
        {
            FieldItem fi = Factory.Fields[0];
            Boolean isDesc = false;
            foreach (FieldItem item in Factory.Fields)
            {
                if (item.DataObjectField.IsIdentity)
                {
                    fi = item;
                    isDesc = true;
                    break;
                }
                else if (item.DataObjectField.PrimaryKey)
                {
                    fi = item;
                    isDesc = false;
                    break;
                }
            }
            Sort(Factory.Fields[0].Name, isDesc);

            IsSorted = false;
            SortProperty = null;
            SortDirection = ListSortDirection.Ascending;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        #endregion
        #endregion

        #region IBindingListView接口
        void IBindingListView.ApplySort(ListSortDescriptionCollection sorts)
        {
            if (sorts == null || sorts.Count < 1) return;

            List<String> ns = new List<string>();
            List<Boolean> ds = new List<bool>();
            foreach (ListSortDescription item in sorts)
            {
                ns.Add(item.PropertyDescriptor.Name);
                ds.Add(item.SortDirection == ListSortDirection.Descending);
            }

            Sort(ns.ToArray(), ds.ToArray());

            SortDescriptions = sorts;
        }

        string _Filter;
        string IBindingListView.Filter
        {
            get { return _Filter; }
            set { _Filter = value; }
        }

        void IBindingListView.RemoveFilter()
        {
            _Filter = "";
        }

        ListSortDescriptionCollection _SortDescriptions;
        ListSortDescriptionCollection IBindingListView.SortDescriptions { get { return _SortDescriptions; } }
        /// <summary>获取当前应用于数据源的排序说明的集合。</summary>
        ListSortDescriptionCollection SortDescriptions
        {
            get { return _SortDescriptions; }
            set { if (_SortDescriptions != value) { _SortDescriptions = value; OnListChanged(ResetEventArgs); }; }
        }

        bool IBindingListView.SupportsAdvancedSorting
        {
            get { return true; }
        }

        bool IBindingListView.SupportsFiltering
        {
            get { return false; }
        }
        #endregion

        #region ICancelAddNew 成员
        void ICancelAddNew.CancelNew(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= Count) return;

            RemoveAt(itemIndex);
        }

        void ICancelAddNew.EndNew(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= Count) return;

            this[itemIndex].Insert();
        }
        #endregion
    }
}