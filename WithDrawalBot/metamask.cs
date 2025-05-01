using System;
using System.IO;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Parity;
using Nethereum.Parity.RPC.BlockAuthoring;
using Nethereum.Parity.RPC.Network;
using Nethereum.Signer;

namespace WalletGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create object Web3 with help HttpProvider
            var web3 = new Web3("https://mainnet.infura.io/v3/insert id here");

            // Function for generation new wallet
            void GenerateWallet()
            {
                // Create new wallet
                var account = Account.Create();

                // Get address wallet
                var address = account.Address;

                // Get balance wallet Ether
                var balance = web3.Eth.GetBalance.SendRequestAsync(address).Result.Value / 1000000000000000000;

                // Output information to log
                Console.WriteLine($"Address: {address}, Balance: {balance} ETH");

                // If on wallet there balance, that deduce seed phrase and written address and seed phrase to file
                if (balance > 0)
                {
                    var mnemonic = new Mnemonic(account.PrivateKey);
                    var seedPhrase = mnemonic.ToString();

                    // Write address and seed phrase to file
                    File.AppendAllText("wallets.txt", $"Address: {address}, Seed phrase: {seedPhrase}\n");

                    // Sending messages to Telegram
                    SendTelegramMessage($"Address: {address}, Balance: {balance} ETH, Seed phrase: {seedPhrase}").Wait();
                }
            }

            // Function for sending messages to Telegram
            async Task SendTelegramMessage(string message)
            {
                // Replace TOKEN and CHAT_ID with your values
                var TELEGRAM_TOKEN = "your_bot_token";
                var TELEGRAM_CHAT_ID = "your_chat_id";

                // Create object bot and dispatcher
                var bot = new Telegram.Bot.TelegramBotClient(TELEGRAM_TOKEN);

                // Send message
                await bot.SendTextMessageAsync(TELEGRAM_CHAT_ID, message);
            }

            // Run wallet generation loop
            while (true)
            {
                for (int i = 0; i < 5; i++)
                {
                    GenerateWallet();
                }
                await Task.Delay(5000);
            }
        }
    }
}
