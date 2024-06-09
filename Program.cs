using System.Diagnostics;
using DiscordRPC.Logging;
using AsakaDiscordRPC.Services;
using DiscordRPC;
using System.Text.RegularExpressions;

#pragma warning disable SYSLIB1045, SYSLIB1054, CA1862

namespace AsakaDiscordRPC {
    public class Initialize {
        private static readonly dynamic config = Config.GetConfig();
        private static readonly DiscordRpcClient client = new((string)config.app_id) {
            Logger = new ConsoleLogger() { Level = LogLevel.Warning }
        };
        private static readonly string[] priority_list = [
            // music
            "fl64",
            // games
            "client-win64-shipping", "r5apex", "content warning", "dayz_x64", "osu!",
            // other / coding
            "code", "firefox"
        ];
        public static Task Main() {

            client.OnReady += (sender, e) => {
                System.Console.WriteLine("RPC has been started!");
            };

            client.Initialize();

            // Rich presence handler
            RichPresence RPC_DATA = new();
            Timestamps? timestamp = null;
            string previous_app = string.Empty;

            // General info
            Assets assets = new() {
                LargeImageKey = "asaka_avatar",
                LargeImageText = "Created by asaka"
            };

            RPC_DATA.Buttons = [
                new Button() {
                        Label = "Soundcloud",
                        Url = "https://soundcloud.com/yourasaka"
                    },
                    new Button() {
                        Label = "Telegram",
                        Url = "https://t.me/yourasaka"
                    }
            ];

            // Main loop
            while (true) {
                // Choosing process
                string[]? chosen_process_array = ChooseProcess();

                if (chosen_process_array == null) {
                    client.Logger.Error("chosen process in null");
                    Environment.Exit(100);
                }

                string? chosen_process = chosen_process_array[0];
                string chosen_process_window_title = chosen_process_array[1];

                // Timestamp
                if (previous_app == string.Empty || chosen_process != previous_app)
                    timestamp = Timestamps.Now;

                previous_app = chosen_process!;

                // Chosen process handling
                switch (chosen_process) {
                    // music
                    case "fl64":
                        string fl_project_name = Regex.Match(chosen_process_window_title, ".*.flp").Groups.Values.FirstOrDefault()!.ToString();
                        if (fl_project_name == string.Empty)
                            fl_project_name = "empty project";

                        RPC_DATA.WithDetails("making music!");
                        RPC_DATA.WithState($"working on: {fl_project_name}");

                        assets.SmallImageKey = "fl_logo";
                        assets.SmallImageText = "FL Studio 21";
                        assets.LargeImageKey = "asaka_avatar";
                        break;

                    // games
                    // TODO: add small icons
                    case "client-win64-shipping":
                        RPC_DATA.WithDetails("waiting for Yinlin <3");
                        RPC_DATA.WithState("in: Wuthering Waves");

                        assets.SmallImageKey = "wuwa";
                        assets.SmallImageText = "Wuthering Waves (Europe)";
                        assets.LargeImageKey = "https://media1.tenor.com/m/0-5Rn0PTx_gAAAAC/emolty-yinlin.gif";
                        break;
                    case "r5apex":
                        RPC_DATA.WithDetails("dying to 3rd party");
                        RPC_DATA.WithState("in: Apex Legends");

                        assets.SmallImageKey = "apex_logo";
                        assets.SmallImageText = "Apex Legends";
                        assets.LargeImageKey = "asaka_avatar";
                        break;
                    case "content warning":
                        RPC_DATA.WithDetails("recording home video with homies");
                        RPC_DATA.WithState("in: Content Warning");

                        assets.SmallImageKey = "cw_logo";
                        assets.SmallImageText = "Content Warning";
                        assets.LargeImageKey = "asaka_avatar";
                        break;
                    case "dayz_x64":
                        RPC_DATA.WithDetails("trying to find food");
                        RPC_DATA.WithState("in: DayZ");

                        assets.SmallImageKey = "dayz_logo";
                        assets.SmallImageText = "DayZ";
                        assets.LargeImageKey = "asaka_avatar";
                        break;
                    case "osu!":
                        RPC_DATA.WithDetails("boosting my friends account");
                        RPC_DATA.WithState("in: osu!");

                        assets.SmallImageKey = "osu_logo";
                        assets.SmallImageText = "osu!";
                        assets.LargeImageKey = "asaka_avatar";
                        break;

                    // other / coding
                    case "code":
                        var vscode_split = chosen_process_window_title.Split('-', StringSplitOptions.TrimEntries);

                        RPC_DATA.WithDetails($"coding: {vscode_split[1]}");
                        RPC_DATA.WithState($"working on: {vscode_split[0]}");

                        assets.SmallImageKey = "vscode";
                        assets.SmallImageText = "Visual Studio Code";
                        assets.LargeImageKey = "asaka_avatar";
                        break;
                    case "firefox":
                        // TODO: refactor later
                        string firefox_tab = "empty tab";
                        string firefox_action = "on";
                        string firefox_action_source = "browsing firefox";
                        string logo = "firefox_logo";
                        string small_text = "Mozilla Firefox";

                        if (Regex.Match(chosen_process_window_title, "(?<=).+?(?= — Mozilla Firefox)").Success)
                            firefox_tab = Regex.Match(chosen_process_window_title, "(?<=).+?(?= — Mozilla Firefox)").Groups.Values.FirstOrDefault()!.ToString();
                        if (firefox_tab.ToLower().Contains("discover the top streamed music and songs online on soundcloud"))
                            firefox_tab = "Home page";

                        if (Regex.Match(chosen_process_window_title, ".*Twitch").Success) {
                            firefox_action = "watching";
                            firefox_action_source = "procrastinating on twitch";

                            small_text = "Twitch - Mozilla Firefox";
                            logo = "twitch_logo";
                        }
                        if (Regex.Match(chosen_process_window_title, ".*YouTube").Success) {
                            firefox_action = "watching";
                            firefox_action_source = "procrastinating on youtube";

                            small_text = "YouTube - Mozilla Firefox";
                            logo = "youtube_logo";
                        }
                        if (Regex.Match(chosen_process_window_title.ToLower(), "(?<=).+?(?=soundcloud)").Success) {
                            firefox_action = "listening to";
                            firefox_action_source = "browsing soundcloud";

                            small_text = "SoundCloud - Mozilla Firefox";
                            logo = "soundcloud_logo";
                        }
                        if (firefox_tab.Length + firefox_action.Length + 2 >= 128)
                            firefox_tab = string.Concat(firefox_tab.AsSpan(0, 128 - firefox_action.Length - 5), "...");

                        RPC_DATA.WithDetails(firefox_action_source);
                        RPC_DATA.WithState($"{firefox_action}: {firefox_tab}");

                        assets.SmallImageKey = logo;
                        assets.SmallImageText = small_text;
                        assets.LargeImageKey = "asaka_avatar";
                        break;

                    // default
                    default:
                        RPC_DATA.Details = "chilling";
                        RPC_DATA.State = "...or procrastinating";
                        assets.LargeImageKey = "asaka_avatar";
                        assets.SmallImageKey = "";
                        break;
                }

                RPC_DATA.WithAssets(assets);
                RPC_DATA.WithTimestamps(timestamp);

                if ((string)config.custom_status != "") {
                    RPC_DATA.Details = (string)config.custom_status;
                    chosen_process = null;
                }

                // Updating presense
                try {
                    client.SetPresence(RPC_DATA);
                } catch (Exception ex) {
                    client.Logger.Error(ex.Message);
                    Environment.Exit(0);
                }

                // Bedge
                Thread.Sleep(3000);
            }
        }
        private static string[]? ChooseProcess() {
            var process_list_raw = Process.GetProcesses();

            Dictionary<string, string> process_list = [];
            while (process_list.Count <= 0) {
                foreach (var process in process_list_raw) {
                    if (!string.IsNullOrEmpty(process.MainWindowTitle)) {
                        process_list.Add(process.ProcessName.ToLower(), process.MainWindowTitle.ToString().Replace("#", ""));
                    }
                }
            }

            string? chosen_process = null;
            string? chosen_process_window_title = null;
            for (int i = 0; i < priority_list.Length; i++) {
                if (process_list.Keys.ToArray().Contains(priority_list[i])) {
                    chosen_process = priority_list[i];
                    chosen_process_window_title = process_list[chosen_process];
                    break;
                }
            }

            return [chosen_process!, chosen_process_window_title!];
        }
    }
}