using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Security;
using System.Text;
using System.Windows;

namespace Igniter.SystemMock
{
    public sealed class DependencyPropertyDescriptor : PropertyDescriptor
    {
        private static Dictionary<object, DependencyPropertyDescriptor> _cache =
            new Dictionary<object, DependencyPropertyDescriptor>((IEqualityComparer<object>)new ReferenceEqualityComparer());

        private static Dictionary<object, DependencyPropertyDescriptor> _ignorePropertyTypeCache =
            new Dictionary<object, DependencyPropertyDescriptor>((IEqualityComparer<object>)new ReferenceEqualityComparer());

        private PropertyDescriptor _property;
        private Type _componentType;
        private DependencyProperty _dp;
        private bool _isAttached;
        private PropertyMetadata _metadata;

        public DependencyProperty DependencyProperty
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._dp; }
        }

        public bool IsAttached
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._isAttached; }
        }

        public PropertyMetadata Metadata
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._metadata; }
        }

        public override Type ComponentType
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return this._componentType; }
        }

        public override bool IsReadOnly
        {
            get { return this.Property.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return this._dp.PropertyType; }
        }

        public override AttributeCollection Attributes
        {
            get { return this.Property.Attributes; }
        }

        public override string Category
        {
            get { return this.Property.Category; }
        }

        public override string Description
        {
            get { return this.Property.Description; }
        }

        public override bool DesignTimeOnly
        {
            get { return this.Property.DesignTimeOnly; }
        }

        public override string DisplayName
        {
            get { return this.Property.DisplayName; }
        }

        public override TypeConverter Converter
        {
            get
            {
                TypeConverter converter = this.Property.Converter;
                if (converter.GetType().IsPublic)
                    return converter;
                return (TypeConverter)null;
            }
        }

        public override bool IsBrowsable
        {
            get { return this.Property.IsBrowsable; }
        }

        public override bool IsLocalizable
        {
            get { return this.Property.IsLocalizable; }
        }

        public override bool SupportsChangeEvents
        {
            get { return this.Property.SupportsChangeEvents; }
        }

        //public CoerceValueCallback DesignerCoerceValueCallback
        //{
        //    get { return this.DependencyProperty.DesignerCoerceValueCallback; }
        //    set { this.DependencyProperty.DesignerCoerceValueCallback = value; }
        //}

        private PropertyDescriptor Property
        {
            
            get
            {
                if (this._property == null)
                {
                    this._property = TypeDescriptor.GetProperties(this._componentType)[this.Name];
                    if (this._property == null)
                        this._property = TypeDescriptor.CreateProperty(this._componentType, this.Name, this._dp.PropertyType, new Attribute[0]);
                }
                return this._property;
            }
        }

        private DependencyPropertyDescriptor(PropertyDescriptor property, string name, Type componentType, DependencyProperty dp, bool isAttached)
            : base(name, (Attribute[])null)
        {
            this._property = property;
            this._componentType = componentType;
            this._dp = dp;
            this._isAttached = isAttached;
            this._metadata = this._dp.GetMetadata(componentType);
        }

        public static DependencyPropertyDescriptor FromProperty(PropertyDescriptor property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            DependencyPropertyDescriptor propertyDescriptor1;
            bool flag;
            lock (DependencyPropertyDescriptor._cache)
                flag = DependencyPropertyDescriptor._cache.TryGetValue((object)property, out propertyDescriptor1);
            if (flag)
                return propertyDescriptor1;
            DependencyProperty dp = (DependencyProperty)null;
            bool isAttached = false;


            var propertyType = property.GetType();
            if (propertyType.UnderlyingSystemType.ToString() == "MS.Internal.ComponentModel.DependencyObjectPropertyDescriptor")
            {
                dp = (DependencyProperty)GetPrivateValue(property, propertyType, "DependencyProperty");
                isAttached = (bool)GetPrivateValue(property, propertyType, "IsAttached");
            }
            else
            {
                var t = SearchType(property.Attributes);

                dp = (DependencyProperty)GetPrivateValue(property, propertyType, "DependencyProperty");
                isAttached = (bool)GetPrivateValue(property, propertyType, "IsAttached");
            }

            if (dp != null)
            {
                propertyDescriptor1 = new DependencyPropertyDescriptor(property, property.Name, property.ComponentType, dp, isAttached);
                lock (DependencyPropertyDescriptor._cache)
                    DependencyPropertyDescriptor._cache[(object)property] = propertyDescriptor1;
            }
            return propertyDescriptor1;
        }

        private static object GetPrivateValue(object self, Type type, String name)
        {
            var property = type.UnderlyingSystemType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
            
            return property!=null ? property.GetValue(self, null) : null;
        }

        private static object SearchType(AttributeCollection collection)
        {
            for (var i = 0; i < collection.Count; i++)
                if (collection[i].GetType().FullName == "MS.Internal.ComponentModel.DependencyPropertyAttribute")
                    return collection[i];

            return null;
        }

        //internal static DependencyPropertyDescriptor FromProperty(DependencyProperty dependencyProperty, Type ownerType, Type targetType, bool ignorePropertyType)
        //{
        //    if (dependencyProperty == null)
        //        throw new ArgumentNullException("dependencyProperty");
        //    if (targetType == (Type)null)
        //        throw new ArgumentNullException("targetType");
        //    DependencyPropertyDescriptor propertyDescriptor = (DependencyPropertyDescriptor)null;
        //    if (ownerType.GetProperty(dependencyProperty.Name) != (PropertyInfo)null)
        //    {
        //        lock (DependencyPropertyDescriptor._ignorePropertyTypeCache)
        //            DependencyPropertyDescriptor._ignorePropertyTypeCache.TryGetValue((object)dependencyProperty, out propertyDescriptor);
        //        if (propertyDescriptor == null)
        //        {
        //            propertyDescriptor = new DependencyPropertyDescriptor((PropertyDescriptor)null, dependencyProperty.Name, targetType, dependencyProperty, false);
        //            lock (DependencyPropertyDescriptor._ignorePropertyTypeCache)
        //                DependencyPropertyDescriptor._ignorePropertyTypeCache[(object)dependencyProperty] = propertyDescriptor;
        //        }
        //    }
        //    else
        //    {
        //        if (ownerType.GetMethod("Get" + dependencyProperty.Name) == (MethodInfo)null && ownerType.GetMethod("Set" + dependencyProperty.Name) == (MethodInfo)null)
        //            return (DependencyPropertyDescriptor)null;
        //        PropertyDescriptor property = (PropertyDescriptor)DependencyObjectProvider.GetAttachedPropertyDescriptor(dependencyProperty, targetType);
        //        if (property != null)
        //            propertyDescriptor = DependencyPropertyDescriptor.FromProperty(property);
        //    }
        //    return propertyDescriptor;
        //}

        //public static DependencyPropertyDescriptor FromProperty(DependencyProperty dependencyProperty, Type targetType)
        //{
        //    if (dependencyProperty == null)
        //        throw new ArgumentNullException("dependencyProperty");
        //    if (targetType == (Type)null)
        //        throw new ArgumentNullException("targetType");
        //    DependencyPropertyDescriptor propertyDescriptor = (DependencyPropertyDescriptor)null;
        //    DependencyPropertyKind dependencyPropertyKind = DependencyObjectProvider.GetDependencyPropertyKind(dependencyProperty, targetType);
        //    if (dependencyPropertyKind.IsDirect)
        //    {
        //        lock (DependencyPropertyDescriptor._cache)
        //            DependencyPropertyDescriptor._cache.TryGetValue((object)dependencyProperty, out propertyDescriptor);
        //        if (propertyDescriptor == null)
        //        {
        //            propertyDescriptor = new DependencyPropertyDescriptor((PropertyDescriptor)null, dependencyProperty.Name, targetType, dependencyProperty, false);
        //            lock (DependencyPropertyDescriptor._cache)
        //                DependencyPropertyDescriptor._cache[(object)dependencyProperty] = propertyDescriptor;
        //        }
        //    }
        //    else if (!dependencyPropertyKind.IsInternal)
        //    {
        //        PropertyDescriptor property = (PropertyDescriptor)DependencyObjectProvider.GetAttachedPropertyDescriptor(dependencyProperty, targetType);
        //        if (property != null)
        //            propertyDescriptor = DependencyPropertyDescriptor.FromProperty(property);
        //    }
        //    return propertyDescriptor;
        //}

        //public static DependencyPropertyDescriptor FromName(string name, Type ownerType, Type targetType)
        //{
        //    if (name == null)
        //        throw new ArgumentNullException("name");
        //    if (ownerType == (Type)null)
        //        throw new ArgumentNullException("ownerType");
        //    if (targetType == (Type)null)
        //        throw new ArgumentNullException("targetType");
        //    DependencyProperty dependencyProperty = DependencyProperty.FromName(name, ownerType);
        //    if (dependencyProperty != null)
        //        return DependencyPropertyDescriptor.FromProperty(dependencyProperty, targetType);
        //    return (DependencyPropertyDescriptor)null;
        //}

        //public static DependencyPropertyDescriptor FromName(string name, Type ownerType, Type targetType, bool ignorePropertyType)
        //{
        //    if (name == null)
        //        throw new ArgumentNullException("name");
        //    if (ownerType == (Type)null)
        //        throw new ArgumentNullException("ownerType");
        //    if (targetType == (Type)null)
        //        throw new ArgumentNullException("targetType");
        //    DependencyProperty dependencyProperty = DependencyProperty.FromName(name, ownerType);
        //    if (dependencyProperty == null)
        //        return (DependencyPropertyDescriptor)null;
        //    if (!ignorePropertyType)
        //        return DependencyPropertyDescriptor.FromProperty(dependencyProperty, targetType);
        //    try
        //    {
        //        return DependencyPropertyDescriptor.FromProperty(dependencyProperty, ownerType, targetType, ignorePropertyType);
        //    }
        //    catch (AmbiguousMatchException ex)
        //    {
        //        return DependencyPropertyDescriptor.FromProperty(dependencyProperty, targetType);
        //    }
        //}

        public override bool Equals(object obj)
        {
            DependencyPropertyDescriptor propertyDescriptor = obj as DependencyPropertyDescriptor;
            return propertyDescriptor != null && propertyDescriptor._dp == this._dp && propertyDescriptor._componentType == this._componentType;
        }

        public override int GetHashCode()
        {
            return this._dp.GetHashCode() ^ this._componentType.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool CanResetValue(object component)
        {
            return this.Property.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return this.Property.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            this.Property.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            this.Property.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return this.Property.ShouldSerializeValue(component);
        }

        public override void AddValueChanged(object component, EventHandler handler)
        {
            this.Property.AddValueChanged(component, handler);
        }

        public override void RemoveValueChanged(object component, EventHandler handler)
        {
            this.Property.RemoveValueChanged(component, handler);
        }

        public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
        {
            return this.Property.GetChildProperties(instance, filter);
        }

        public override object GetEditor(Type editorBaseType)
        {
            return this.Property.GetEditor(editorBaseType);
        }

        internal static void ClearCache()
        {
            lock (DependencyPropertyDescriptor._cache)
                DependencyPropertyDescriptor._cache.Clear();
        }
    }
}
