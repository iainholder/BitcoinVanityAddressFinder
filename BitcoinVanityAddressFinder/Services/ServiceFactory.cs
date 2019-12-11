using System.Collections.Generic;

namespace BitcoinVanityAddressFinder.Services
{
    public interface IServiceFactory
    {
        VanityAddressService GetVanityAddressService();
        InputStringVerifierService GetInputStringVerifierService(string vanityText, bool isCaseSensitive, bool isStartsWith, bool isEndsWith);
        DictionaryWordVerifierService GetDictionaryWordVerifierService(HashSet<string> words, bool isCaseSensitive, bool isStartsWith, bool isEndsWith);
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class ServiceFactory : IServiceFactory
    {
        public VanityAddressService GetVanityAddressService()
        {
            return new VanityAddressService(this);
        }

        public InputStringVerifierService GetInputStringVerifierService(string vanityText, bool isCaseSensitive, bool isStartsWith, bool isEndsWith)
        {
            return new InputStringVerifierService(vanityText, isCaseSensitive, isStartsWith, isEndsWith);
        }

        public DictionaryWordVerifierService GetDictionaryWordVerifierService(HashSet<string> words, bool isCaseSensitive, bool isStartsWith, bool isEndsWith)
        {
            return new DictionaryWordVerifierService(words, isCaseSensitive, isStartsWith, isEndsWith);
        }
    }
}