/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:BitcoinVanityAddressFinder"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/
using Microsoft.Extensions.DependencyInjection;

// using GalaSoft.MvvmLight;

using BitcoinVanityAddressFinder.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommonServiceLocator;

namespace BitcoinVanityAddressFinder.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            // ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default); // Not needed with CommunityToolkit.Mvvm.DependencyInjection.Ioc

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            Ioc.Default.ConfigureServices(
                new Microsoft.Extensions.DependencyInjection.ServiceCollection()
                    .AddSingleton<IServiceFactory, ServiceFactory>()
                    .AddSingleton<VanityAddressViewModel>()
                    .BuildServiceProvider()
            );
        }

        public VanityAddressViewModel VanityAddressViewModel => Ioc.Default.GetRequiredService<VanityAddressViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}