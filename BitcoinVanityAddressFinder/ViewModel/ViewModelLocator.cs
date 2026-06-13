using BitcoinVanityAddressFinder.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace BitcoinVanityAddressFinder.ViewModel
{
    /// <summary>
    /// Wires up dependency injection and exposes the view models as an entry point for XAML bindings.
    /// Registered as a static resource ("Locator") in App.xaml.
    /// </summary>
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddSingleton<IServiceFactory, ServiceFactory>()
                    .AddSingleton<VanityAddressViewModel>()
                    .BuildServiceProvider()
            );
        }

        public VanityAddressViewModel VanityAddressViewModel => Ioc.Default.GetRequiredService<VanityAddressViewModel>();
    }
}