﻿using Crepido.ElmahOfflineViewer.Core.Domain;
using Crepido.ElmahOfflineViewer.Core.Infrastructure.Cache;
using Crepido.ElmahOfflineViewer.Core.Infrastructure.FileSystem;
using Crepido.ElmahOfflineViewer.Core.Infrastructure.Logging;
using Crepido.ElmahOfflineViewer.Core.Infrastructure.Settings;
using Crepido.ElmahOfflineViewer.Core.Infrastructure.Web;
using Crepido.ElmahOfflineViewer.Core.Integrations.HttpUserAgentSearch;
using Crepido.ElmahOfflineViewer.Core.Presentation;
using Ninject.Activation;
using Ninject.Modules;
using NLog;

namespace Crepido.ElmahOfflineViewer.Core.Infrastructure.Dependencies
{
	public class NinjectCoreModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IErrorLogRepository>().To<ErrorLogRepository>().InSingletonScope();
			Bind<ISettingsManager>().To<SettingsManager>();
			Bind<IErrorLogSource>().To<FileErrorLogSource>();
			Bind<IErrorLogFileParser>().To<ErrorLogFileParser>();
			Bind<IReportGenerator>().To<ReportGenerator>();
			Bind<SearchPresenter>().To<SearchPresenter>();
			Bind<ReportPresenter>().To<ReportPresenter>();
			Bind<SettingsPresenter>().To<SettingsPresenter>();
			Bind<SelectServerPresenter>().To<SelectServerPresenter>();
			Bind<ICacheHelper>().To<CacheHelper>();
			Bind<IWebRequestHelper>().To<WebRequestHelper>();
			Bind<IClientInformationResolver>().To<ClientInformationResolver>();
			Bind<IProcessStarter>().To<ProcessStarter>();
			Bind<IUrlPing>().To<UrlPing>();
			Bind<IErrorLogDownloader>().To<ErrorLogDownloader>();
			Bind<IHttpUserAgentSearchLauncher>().To<UserAgentStringSearchLauncher>();
			Bind<IFileSystemHelper>().To<FileSystemHelper>();
			Bind<ILog>().ToMethod(GetLogger);
		}

		private static ILog GetLogger(IContext context)
		{
			var loggerName = "Default";

			if (context.Request.Target != null)
			{
				loggerName = context.Request.Target.Member.DeclaringType.FullName;
			}

			var logger = LogManager.GetLogger(loggerName);

			return new Log(logger);
		}
	}
}
