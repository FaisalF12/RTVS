﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Common.Core;
using Microsoft.Extensions.Logging;

namespace Microsoft.R.Host.Monitor {
    public class BrokerManager {
        private const string RHostBroker = "Microsoft.R.Host.Broker";
        private static string RHostBrokerExe = $"{RHostBroker}.exe";
        private static string RHostBrokerConfig = $"{RHostBroker}.Config.json";
        private static Process _brokerProcess;

        public static bool AutoRestart { get; set; }
        public static int AutoRestartMaxCount {
            get {
                return Properties.Settings.Default.AutoRestartMaxCount;
            }
            set {
                Properties.Settings.Default.AutoRestartMaxCount = value;
            }
        }

        private static int _autoRestartCount = 0;

        static BrokerManager() {
            AutoRestart = true;
        }

        public static Task<int> CreateOrAttachToBrokerInstanceAsync(ILogger logger = null) {
            return Task.Run(async () => {
                await StopBrokerInstanceAsync();
                Process[] processes = Process.GetProcessesByName(RHostBroker);
                if (processes.Length > 0) {
                    _brokerProcess = processes[0];
                    _brokerProcess.EnableRaisingEvents = true;
                    _brokerProcess.Exited += async (object sender, EventArgs e) => {
                        await ProcessExitedAsync(logger);
                    };
                    logger?.LogInformation(Resources.Info_BrokerAlreadyRunning, _brokerProcess.Id);
                } else {
                    string assemblyRoot = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                    string rBrokerExePath = Path.Combine(assemblyRoot, RHostBrokerExe);
                    string configFilePath = Path.Combine(assemblyRoot, RHostBrokerConfig);

                    ProcessStartInfo psi = new ProcessStartInfo(rBrokerExePath);
                    psi.Arguments = $"--config \"{configFilePath}\"";
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = false;
                    psi.WorkingDirectory = assemblyRoot;

                    if (Properties.Settings.Default.UseDifferentBrokerUser) {
                        await CredentialManager.SetCredentialsOnProcessAsync(psi, logger);
                    }

                    _brokerProcess = new Process() { StartInfo = psi };
                    _brokerProcess.EnableRaisingEvents = true;
                    _brokerProcess.Exited += async (object sender, EventArgs e) => {
                        await ProcessExitedAsync(logger);
                    };
                    _brokerProcess.Start();
                    logger?.LogInformation(Resources.Info_NewBrokerInstanceStarted, _brokerProcess.Id);
                }

                AutoRestart = true;
                return _brokerProcess.Id;
            });
        }

        public static void ResetAutoStart() {
            _autoRestartCount = 0;
        }

        private static async Task ProcessExitedAsync(ILogger logger = null) {
            if (AutoRestart && ++_autoRestartCount <= AutoRestartMaxCount) {
                try {
                    await CreateOrAttachToBrokerInstanceAsync();
                } catch (Exception ex) when (!ex.IsCriticalException()) {
                    logger?.LogError(Resources.Error_AutoRestartFailed, ex.Message);
                }
            }
        }

        public static async Task<int> StopBrokerInstanceAsync(ILogger logger = null) {
            int id = _brokerProcess?.Id ?? 0;
            await TaskUtilities.SwitchToBackgroundThread();
            try {
                AutoRestart = false;
                _brokerProcess?.Kill();
                _brokerProcess = null;
            } catch (Exception ex) when (!ex.IsCriticalException()) {
                logger?.LogError(Resources.Error_StopBrokerFailed, ex.Message);
            }

            return id;
        }
    }
}
