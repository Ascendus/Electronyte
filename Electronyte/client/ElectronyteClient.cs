﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Electronyte.client
{
    public class ElectronyteClient
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<Configuration>(json);

            DiscordConfiguration configOptions = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
            };

            Client = new DiscordClient(configOptions);

            Client.Ready += ReadyEvent;

            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private async Task<Task> ReadyEvent(object sender, ReadyEventArgs e)
        {
            /*
            int[] users = new int[] {};
            int i = 0;

            foreach (DiscordGuild guild in Client.Guilds.Values)
            {
                users[i] = guild.MemberCount;
                i++;
            }

            int sum = 0;
            Array.ForEach(users, delegate (int i) { sum += i; });
            */

            DiscordActivity activity = new DiscordActivity
            {
                ActivityType = ActivityType.Watching,
                Name = "stuff" //$"{sum} users"
            };

            Console.WriteLine("Ready!");
            await Client.UpdateStatusAsync(activity, UserStatus.Online);
            return Task.CompletedTask;
        }
    }
}