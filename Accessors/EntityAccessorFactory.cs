﻿using System;
using NewLife.Model;
using XCode.Model;

namespace XCode.Accessors
{
    /// <summary>实体访问器工厂</summary>
    public static class EntityAccessorFactory
    {
        internal static void Reg(IObjectContainer container)
        {
            // 注册内置访问器
            container
                .Register<IEntityAccessor, HttpEntityAccessor>(EntityAccessorTypes.Http.ToString())
                .Register<IEntityAccessor, WebFormEntityAccessor>(EntityAccessorTypes.WebForm.ToString())
                .Register<IEntityAccessor, WinFormEntityAccessor>(EntityAccessorTypes.WinForm.ToString())
                .Register<IEntityAccessor, BinaryEntityAccessor>(EntityAccessorTypes.Binary.ToString())
                .Register<IEntityAccessor, XmlEntityAccessor>(EntityAccessorTypes.Xml.ToString())
                .Register<IEntityAccessor, JsonEntityAccessor>(EntityAccessorTypes.Json.ToString());
        }

        /// <summary>
        /// 创建指定类型的实体访问器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEntityAccessor Create(String name)
        {
            return XCodeService.Resolve<IEntityAccessor>(name);
        }

        /// <summary>
        /// 创建指定类型的实体访问器
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static IEntityAccessor Create(EntityAccessorTypes kind)
        {
            return Create(kind.ToString());
        }

        internal static Boolean EqualIgnoreCase(this String str, EntityAccessorOptions option)
        {
            //if (String.IsNullOrEmpty(str)) return false;

            return String.Equals(str, option.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}