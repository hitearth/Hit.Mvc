﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using log4net;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hit.Mvc
{
    /// <summary>
    /// Config
    /// </summary>
    public class Config
    {
        internal static Config Cfg = new Config();
        internal static Func<HttpContext, Exception, bool> DefaultExceptionHandle = (httpContext, ex) =>
        {
            if (new HttpException(null, ex).GetHttpCode() != 500)
                return false;

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.ContentType = "text/html; charset=utf-8";
            httpContext.Response.Write(ex.ToString());
            return true;
        };

        /// <summary>
        /// 是否是Debug模式
        /// </summary>
        public bool Debug = true;
        /// <summary>
        /// 获取错误页面
        /// </summary>
        public Func<string, string> GetErrorPage = _ => "/c/error";
        /// <summary>
        /// 验证用户
        /// </summary>
        public Func<AuthorizationContext, bool> Auth = _ => true;
        /// <summary>
        /// 获取登陆页面 参数 area
        /// </summary>
        public Func<string, string> GetLoginPage = _ => "/login";
        /// <summary>
        /// 全局异常处理
        /// </summary>
        public Func<HttpContext, Exception, bool> ExceptionHandle = DefaultExceptionHandle;
        internal Dictionary<string, Func<object>> pathFactory;
        internal Func<string, IEnumerable<Filter>> GetAuthFilterThunk;
        /// <summary>
        /// 登陆判断 Filters
        /// </summary>
        public IEnumerable<Filter> RequireLoginFilters = new List<Filter> { new Filter(new RequireLoginAttribute(), FilterScope.Global, null) };
        /// <summary>
        /// 移动端访问判断 Filters
        /// </summary>
        public IEnumerable<Filter> RequireMobileFilters = new List<Filter> { new Filter(DependencyResolver.Current.GetService<RequireMobileAttribute>(), FilterScope.First, null) };
        /// <summary>
        /// 空 Filters
        /// </summary>
        public IEnumerable<Filter> EmptyFilters = new List<Filter>(0);

        internal JToken json;
        /// <summary>
        /// 配置项
        /// </summary>
        /// <param name="i">key</param>
        /// <returns>配置项</returns>
        public JToken this[string i] { get { return json[i]; } }

        internal Dictionary<string, object> AutofacDict;

        /// <summary>
        /// 配置开始生效，不执行这个方法的话配置信息无法生效
        /// </summary>
        public void Start()
        {

            #region Mvc 优化
            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            ValueProviderFactories.Factories.Insert(2, new JsonNetValueProviderFactory());
            ViewEngines.Engines.Remove(ViewEngines.Engines.OfType<WebFormViewEngine>().FirstOrDefault());
            var rg = ViewEngines.Engines.OfType<RazorViewEngine>().First();
            rg.FileExtensions = new[] { "cshtml" };
            rg.AreaViewLocationFormats = new[] { "~/Areas/{2}/Views/{1}/{0}.cshtml", "~/Areas/{2}/Views/Shared/{0}.cshtml", };
            rg.AreaPartialViewLocationFormats = new[] { "~/Areas/{2}/Views/{1}/{0}.cshtml", "~/Areas/{2}/Views/Shared/{0}.cshtml", };
            rg.ViewLocationFormats = new[] { "~/Views/{1}/{0}.cshtml", "~/Views/Shared/{0}.cshtml", };
            rg.PartialViewLocationFormats = new[] { "~/Views/{1}/{0}.cshtml", "~/Views/Shared/{0}.cshtml", };
            MvcHandler.DisableMvcResponseHeader = true;
            #endregion

            #region CompileView
            if (pathFactory != null)
                System.Web.WebPages.VirtualPathFactoryManager.RegisterVirtualPathFactory(new CompileViewVirtualPathFactory(pathFactory));
            #endregion

            #region 配置
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            #endregion

            #region AutofacDependency
            if (AutofacDict != null)
            {
                ContainerBuilder builder = new ContainerBuilder();
                foreach (var cfgitem in AutofacDict)
                {
                    switch (cfgitem.Key)
                    {
                        case "Controllers":
                            var curAssembly = (Assembly)cfgitem.Value;
                            builder.RegisterControllers(curAssembly);
                            break;
                        case "StartsWith":
                            var assemblies = System.Web.Compilation.BuildManager.GetReferencedAssemblies();
                            foreach (Assembly item in assemblies)
                            {
                                if (item.FullName.StartsWith((string)cfgitem.Value))
                                    builder.RegisterAssemblyTypes(item);
                            }
                            break;
                        case "func":
                            Action<ContainerBuilder> func = (Action<ContainerBuilder>)cfgitem.Value;
                            func(builder);
                            break;
                        default: throw new Exception("不支持" + cfgitem.Key);
                    }
                }
                IContainer container = builder.Build();
                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            }
            #endregion

            #region JSON.NET
            JsonConvert.DefaultSettings = () =>
            {
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.Converters.Add(new WriteDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm" });
                return serializerSettings;
            };
            #endregion

            GlobalFilters.Filters.Add(new AuthorizationAttribute());


            if (GetAuthFilterThunk != null)
            {
                var fp = new AreaFilterCollection(GetAuthFilterThunk);
                FilterProviders.Providers.Add(fp);
            }
        }

    }
    /// <summary>
    /// 生成Config
    /// </summary>
    public static class ConfigBuild
    {
        /// <summary>
        /// 当前Config
        /// </summary>
        public static Config Config
        {
            get
            {
                return Config.Cfg;
            }
        }
        /// <summary>
        /// 只读一次的配置文件的配置
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Config AddJsonFile(this Config cfg, string path)
        {
            if (Config.Cfg.json == null)
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    Config.Cfg.json = JObject.Load(new JsonTextReader(sr)) as JToken;
                }
            }
            else throw new NotSupportedException();
            return cfg;
        }
        /// <summary>
        /// 可监测配置文件的配置
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <param name="ilog"></param>
        /// <returns></returns>
        public static Config AddWatch(this Config cfg, string path, string key, Action<JToken> callback, Action<string> ilog)
        {
            new Watcher(path, key, callback, ilog).Load();
            return cfg;
        }
        /// <summary>
        /// 默认 Debug = true
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="Debug">默认是true</param>
        /// <returns></returns>
        public static Config SetDebug(this Config cfg, bool Debug)
        {
            cfg.Debug = Debug;
            if (Debug)
                cfg.ExceptionHandle = Config.DefaultExceptionHandle;
            else
                cfg.ExceptionHandle = (httpContext, ex) =>
                {
                    if (new HttpException(null, ex).GetHttpCode() != 500)
                    {
                        return false;
                    }

                    httpContext.ClearError();
                    httpContext.Response.Clear();

                    log4net.ILog log = LogManager.GetLogger("AppError");
                    log.Error(httpContext.Request.Url, ex);
                    string errorPage = Config.Cfg.GetErrorPage(httpContext.Request.Url.ToString());
                    if (httpContext.Request.Url.AbsolutePath != errorPage)
                        httpContext.Response.Redirect(errorPage);
                    return true;
                };
            return cfg;
        }
        /// <summary>
        /// 依赖注入配置
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="AutofacDict"></param>
        /// <returns></returns>
        public static Config UseAutofac(this Config cfg, Dictionary<string, object> AutofacDict)
        {
            cfg.AutofacDict = AutofacDict;
            return cfg;

        }
        /// <summary>
        /// 权限配置
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="getLoginPage"></param>
        /// <param name="auth"></param>
        /// <param name="requierLogin"></param>
        /// <returns></returns>
        public static Config UseAuth(this Config cfg, Func<string, string> getLoginPage, Func<AuthorizationContext, bool> auth, bool requierLogin = false)
        {
            Func<string, IEnumerable<Filter>> d;
            if (requierLogin) d = _ => cfg.RequireLoginFilters; else d = _ => cfg.EmptyFilters;
            return cfg.UseAuth(getLoginPage, auth, d);
        }
        /// <summary>
        /// 权限配置
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="getLoginPage"></param>
        /// <param name="auth"></param>
        /// <param name="GetAuthFilter"></param>
        /// <returns></returns>
        public static Config UseAuth(this Config cfg, Func<string, string> getLoginPage, Func<AuthorizationContext, bool> auth, Func<string, IEnumerable<Filter>> GetAuthFilter)
        {
            if (getLoginPage != null) cfg.GetLoginPage = getLoginPage;
            if (auth != null) cfg.Auth = auth;

            cfg.GetAuthFilterThunk = GetAuthFilter;
            return cfg;
        }

        /// <summary>
        /// 使用预编译视图
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="pathFactory"></param>
        /// <returns></returns>
        public static Config UseCompileView(this Config cfg, Dictionary<string, Func<object>> pathFactory)
        {
            cfg.pathFactory = pathFactory;
            return cfg;
        }

    }





}
