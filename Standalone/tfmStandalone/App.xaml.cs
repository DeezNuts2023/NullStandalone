using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using Microsoft.Practices.Unity;
using tfmClientHook;

namespace tfmStandalone
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			IUnityContainer container = new UnityContainer();
			UnityContainerExtensions.RegisterType<ClientHook>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<MessageInterceptor>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<GameInfo>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<GameSettings>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<CustomCommandInterface>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<LogHandlerService>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<CasierHandlerService>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<WindowService>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<CommandService>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<UpdateService>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<MainWindowViewModel>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<ChatWindowViewModel>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<MainWindow>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<ChatWindow>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<ChatSettingsWindow>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.RegisterType<ModopwetWindow>(container, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
			UnityContainerExtensions.Resolve<GameSettings>(container, new ResolverOverride[0]);
			UnityContainerExtensions.Resolve<CommandService>(container, new ResolverOverride[0]);
			UnityContainerExtensions.Resolve<LogHandlerService>(container, new ResolverOverride[0]);
			UnityContainerExtensions.Resolve<CasierHandlerService>(container, new ResolverOverride[0]);
			MainWindow mainWindow = UnityContainerExtensions.Resolve<MainWindow>(container, new ResolverOverride[0]);
			mainWindow.Show();
			ChatWindow chatWindow = UnityContainerExtensions.Resolve<ChatWindow>(container, new ResolverOverride[0]);
			chatWindow.Show();
			ModopwetWindow modopwetWindow = UnityContainerExtensions.Resolve<ModopwetWindow>(container, new ResolverOverride[0]);
			mainWindow.Closed += delegate(object sender, EventArgs args)
			{
				UnityContainerExtensions.Resolve<ClientHook>(container, new ResolverOverride[0]).Stop();
				modopwetWindow.IsApplicationClosing = true;
				modopwetWindow.Close();
				chatWindow.IsApplicationClosing = true;
				chatWindow.Close();
			};
		}

		public void SetTheme(Theme theme)
		{
			string uriString;
			if (theme != Theme.Light)
			{
				if (theme != Theme.Transformice)
				{
					uriString = "/Resources/Themes/Dark.xaml";
				}
				else
				{
					uriString = "/Resources/Themes/Transformice.xaml";
				}
			}
			else
			{
				uriString = "/Resources/Themes/Light.xaml";
			}
			ResourceDictionary item = Application.LoadComponent(new Uri(uriString, UriKind.Relative)) as ResourceDictionary;
			Application.Current.Resources.MergedDictionaries.Clear();
			Application.Current.Resources.MergedDictionaries.Add(item);
		}
	}
}
