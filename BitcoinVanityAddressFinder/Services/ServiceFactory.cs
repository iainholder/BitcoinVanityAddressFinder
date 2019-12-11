namespace BitcoinVanityAddressFinder.Services
{
    public interface IServiceFactory
    {
        VanityAddressService GetVanityAddressService();
    }

    public class ServiceFactory : IServiceFactory
    {
        public VanityAddressService GetVanityAddressService()
        {
            return new VanityAddressService();
        }
    }
}