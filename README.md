# BitcoinVanityAddressFinder
Windows (WPF) application to find vanity bitcoin addresses using NBitcoin.

'Vanity addresses are valid bitcoin addresses that contain human-readable messages. For example, 
1LoveBPzzD72PUXLzCkYAtGFYmK5vYNR33 is a valid address that contains the letters forming the word "Love" 
as the first four Base-58 letters. Vanity addresses require generating and testing billions of candidate 
private keys, until a bitcoin address with the desired pattern is found. Although there are some optimizations
in the vanity generation algorithm, the process essentially involves picking a private key at random, deriving
the public key, deriving the bitcoin address, and checking to see if it matches the desired vanity pattern, 
repeating billions of times until a match is found.' 

-- Andreas M. Antonopolous, Mastering Bitcoin, O'Reilly

[Open source version: https://github.com/bitcoinbook/bitcoinbook/blob/develop/ch04.asciidoc]

It supports:
- Search for addresses containing strings of up to 8 characters. Each letter makes the search exponentially harder.
- Case sensitivity
- Addresses starting with a given string. Note that first character of a bitcoin address has a meaning so after character.
- Address ending with a given string.
- Multithreaded search up to number of cores your machine has. Defaults to N-1.
- Multiple networks, currently Main, TestNet and RegTest

Other projects used:
- NBitcoin by Nicolas Dorier (https://github.com/MetacoSA/NBitcoin)
- MVVMLight by Laurent Bugnion (http://www.mvvmlight.net)
- NUnit (https://nunit.org/)

** Use at your own risk. Main address are valid and you're playing with real money.**
