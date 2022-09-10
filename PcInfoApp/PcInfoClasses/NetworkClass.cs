using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using PcInfoApp.Util;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace PcInfoApp.PcInfoClasses
{
    public class NetworkClass : INotifyPropertyChanged
    {
        public string CurrentUploadUsage { get; set; }
        public string CurrentDownloadUsage { get; set; }
        public string UploadUsageWhileAppRunning { get; set; }
        public string DownloadUsageWhileAppRunning { get; set; }
        public ObservableDictionary<string, AppUsage> AppsUsage { get; set; }
        private decimal DownloadWhileRunnig;
        private decimal UploadWhileRunnig;
        private string DownloadWhileRunnigType;
        private string UploadWhileRunnigType;
        private BackgroundWorker GetCurrentDownloadAndUploadUsage;
        public event PropertyChangedEventHandler PropertyChanged;

        public NetworkClass()
        {
            this.DownloadWhileRunnig = 0;
            this.UploadWhileRunnig = 0;
            this.DownloadWhileRunnigType = "KB";
            this.UploadWhileRunnigType = "KB";
            this.CurrentDownloadUsage = "0KB/s";
            this.CurrentUploadUsage = "0KB/s";
            this.AppsUsage = new ObservableDictionary<string, AppUsage>();
            GetCurrentDownloadAndUploadUsage = new BackgroundWorker();
            GetCurrentDownloadAndUploadUsage.WorkerSupportsCancellation = true;
            GetCurrentDownloadAndUploadUsage.DoWork += GetCurrentDownloadAndUploadUsage_DoWork;
            GetCurrentDownloadAndUploadUsage.RunWorkerAsync();
        }
        private void PrepareData(decimal UploadUsage, decimal DownloadUsage)
        {
            decimal UploadWhileRunnigDecimal = 0;
            decimal DownloadWhileRunnigDecimal = 0;
            string UploadUsageType, DownloadUsageType;
            UploadUsage /= 1024;
            UploadWhileRunnigDecimal = UploadUsage;
            UploadUsageType = "KB/s";
            if (UploadUsage > 1024)
            {
                UploadUsage /= 1024;
                UploadUsageType = "MB/s";
                if (UploadUsage > 1024)
                {
                    UploadUsage /= 1024;
                    UploadUsageType = "GB/s";
                }
            }
            if (UploadUsage.ToString().Length > 4)
                UploadUsage = decimal.Parse(UploadUsage.ToString().Substring(0, 4));
            this.CurrentUploadUsage = UploadUsage + UploadUsageType;
            OnPropertyChanged("CurrentUploadUsage");
            DownloadUsage /= 1024;
            DownloadWhileRunnigDecimal = DownloadUsage;
            DownloadUsageType = "KB/s";
            if (DownloadUsage > 1024)
            {
                DownloadUsage /= 1024;
                DownloadUsageType = "MB/s";
                if (DownloadUsage > 1024)
                {
                    DownloadUsage /= 1024;
                    DownloadUsageType = "GB/s";
                }
            }
            if (DownloadUsage.ToString().Length > 4)
                DownloadUsage = decimal.Parse(DownloadUsage.ToString().Substring(0, 4));
            this.CurrentDownloadUsage = DownloadUsage + DownloadUsageType;
            OnPropertyChanged("CurrentDownloadUsage");
            if (this.UploadWhileRunnigType == "KB")
            {
                this.UploadWhileRunnig += UploadWhileRunnigDecimal;
                if (this.UploadWhileRunnig > 1024)
                {
                    this.UploadWhileRunnig /= 1024;
                    this.UploadWhileRunnigType = "MB";
                }
            }

            else if (this.UploadWhileRunnigType == "MB")
            {
                this.UploadWhileRunnig += (UploadWhileRunnigDecimal / 1024);
                if (this.UploadWhileRunnig > 1024)
                {
                    this.UploadWhileRunnig /= 1024;
                    this.UploadWhileRunnigType = "GB";
                }
            }

            else if (this.UploadWhileRunnigType == "GB")
            {
                this.UploadWhileRunnig += (UploadWhileRunnigDecimal / 1024 / 1024);
                if (this.UploadWhileRunnig > 1024)
                {
                    this.UploadWhileRunnig /= 1024;
                    this.UploadWhileRunnigType = "TB";
                }
            }
            else if (this.UploadWhileRunnigType == "TB")
                this.UploadWhileRunnig += (UploadWhileRunnigDecimal / 1024 / 1024 / 1024);
            if (this.UploadWhileRunnig.ToString().Contains("."))
                this.UploadUsageWhileAppRunning = this.UploadWhileRunnig.ToString().Substring(0, UploadWhileRunnig.ToString().IndexOf(".") + 2) + this.UploadWhileRunnigType;
            else
                this.UploadUsageWhileAppRunning = this.UploadWhileRunnig + this.UploadWhileRunnigType;
            OnPropertyChanged("UploadUsageWhileAppRunning");
            if (this.DownloadWhileRunnigType == "KB")
            {
                this.DownloadWhileRunnig += DownloadWhileRunnigDecimal;
                if (this.DownloadWhileRunnig > 1024)
                {
                    this.DownloadWhileRunnig /= 1024;
                    this.DownloadWhileRunnigType = "MB";
                }
            }

            else if (this.DownloadWhileRunnigType == "MB")
            {
                this.DownloadWhileRunnig += (DownloadWhileRunnigDecimal / 1024);
                if (this.DownloadWhileRunnig > 1024)
                {
                    this.DownloadWhileRunnig /= 1024;
                    this.DownloadWhileRunnigType = "GB";
                }
            }

            else if (this.DownloadWhileRunnigType == "GB")
            {
                this.DownloadWhileRunnig += (DownloadWhileRunnigDecimal / 1024 / 1024);
                if (this.DownloadWhileRunnig > 1024)
                {
                    this.DownloadWhileRunnig /= 1024;
                    this.DownloadWhileRunnigType = "TB";
                }
            }
            else if (this.DownloadWhileRunnigType == "TB")
                this.DownloadWhileRunnig += (DownloadWhileRunnigDecimal / 1024 / 1024 / 1024);
            if (DownloadWhileRunnig.ToString().Contains("."))
                this.DownloadUsageWhileAppRunning = this.DownloadWhileRunnig.ToString().Substring(0, DownloadWhileRunnig.ToString().IndexOf(".") + 2) + this.DownloadWhileRunnigType;
            else
                this.DownloadUsageWhileAppRunning = this.DownloadWhileRunnig + this.DownloadWhileRunnigType;
            OnPropertyChanged("DownloadUsageWhileAppRunning");
            if (AppsUsage.Count != 0)
            {
                foreach (AppUsage app in AppsUsage.Values)
                {
                    if (app.CurrentUploadInBytes == 0)
                        app.CurrentUpload = "0KB";
                    else if ((app.CurrentUploadInBytes / 1024).ToString().Contains("."))
                    {
                        app.CurrentUpload = (app.CurrentUploadInBytes / 1024).ToString().Substring(0, (app.CurrentUploadInBytes / 1024).ToString().IndexOf(".") + 2) + "KB";
                    }
                    else
                        app.CurrentUpload = (app.CurrentUploadInBytes / 1024) + "KB";
                    if (app.CurrentDownloadInBytes == 0)
                        app.CurrentDownload = "0KB";
                    else if ((app.CurrentDownloadInBytes / 1024).ToString().Contains("."))
                    {
                        app.CurrentDownload = (app.CurrentDownloadInBytes / 1024).ToString().Substring(0, (app.CurrentDownloadInBytes / 1024).ToString().IndexOf(".") + 2) + "KB";
                    }
                    else
                        app.CurrentDownload = (app.CurrentDownloadInBytes / 1024) + "KB";
                    if ((app.TotalDownloadInBytes / 1024 / 1024).ToString().Length > 6)
                    {
                        app.TotalDownload = (app.TotalDownloadInBytes / 1024 / 1024).ToString().Substring(0, 6) + "MB";
                    }
                    else
                        app.TotalDownload = (app.TotalDownloadInBytes / 1024 / 1024) + "MB";
                    if ((app.TotalUploadInBytes / 1024 / 1024).ToString().Length > 6)
                    {
                        app.TotalUpload = (app.TotalUploadInBytes / 1024 / 1024).ToString().Substring(0, 6) + "MB";
                    }
                    else
                        app.TotalUpload = (app.TotalUploadInBytes / 1024 / 1024) + "MB";
                    app.OnPropertyChanged("CurrentUpload");
                    app.OnPropertyChanged("CurrentDownload");
                    app.OnPropertyChanged("TotalDownload");
                    app.OnPropertyChanged("TotalUpload");
                }
                if (AppsUsage.Count != 0)
                {
                    foreach (AppUsage app in AppsUsage.Values)
                    {
                        app.CurrentUploadInBytes = 0;
                        app.CurrentDownloadInBytes = 0;
                    }
                }
            }
        }
        private void GetCurrentDownloadAndUploadUsage_DoWork(object? sender, DoWorkEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            TraceEventSession m_EtwSession;
            decimal UploadUsage = 0;
            decimal DownloadUsage = 0;
            System.Net.IPAddress NetworkIp = null;
            bool IsConnectionToInternet = false;
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                try
                {
                    if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        IsConnectionToInternet = System.Net.IPAddress.TryParse(networkInterface.GetIPProperties().GatewayAddresses[0].Address.ToString(), out NetworkIp);
                    else
                        continue;
                    if (IsConnectionToInternet)
                    {
                        foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                NetworkIp = ip.Address;
                            }
                        }
                    }
                }
                catch (System.ArgumentOutOfRangeException ex)
                {
                    continue;
                }
            }
            if (NetworkIp == null)
                return;
            System.Net.IPAddress PcIp = System.Net.IPAddress.Parse("127.0.0.1");
            try
            {
                using (m_EtwSession = new TraceEventSession("MyKernelAndClrEventsSession"))
                {
                    m_EtwSession.EnableKernelProvider(KernelTraceEventParser.Keywords.NetworkTCPIP);
                    m_EtwSession.Source.Kernel.TcpIpRecv += data =>
                    {
                        if (data.saddr.Equals(NetworkIp))
                        {
                            DownloadUsage += data.size;
                            if (AppsUsage.ContainsKey(data.ProcessName))
                            {
                                AppsUsage[data.ProcessName].CurrentDownloadInBytes += data.size;
                                AppsUsage[data.ProcessName].TotalDownloadInBytes += data.size;
                            }
                            else
                                AppsUsage.Add(data.ProcessName, new AppUsage(data.ProcessName, data.size, 0));
                            if (stopwatch.ElapsedMilliseconds > 1000)
                            {
                                PrepareData(UploadUsage, DownloadUsage);
                                DownloadUsage = 0;
                                UploadUsage = 0;
                                stopwatch.Restart();
                            }
                        }
                    };
                    m_EtwSession.Source.Kernel.UdpIpRecv += data =>
                    {
                        if (data.saddr.Equals(NetworkIp))
                        {
                            DownloadUsage += data.size;
                            if (AppsUsage.ContainsKey(data.ProcessName))
                            {
                                AppsUsage[data.ProcessName].CurrentDownloadInBytes += data.size;
                                AppsUsage[data.ProcessName].TotalDownloadInBytes += data.size;
                            }
                            else
                                AppsUsage.Add(data.ProcessName, new AppUsage(data.ProcessName, data.size, 0));
                            if (stopwatch.ElapsedMilliseconds > 1000)
                            {
                                PrepareData(UploadUsage, DownloadUsage);
                                DownloadUsage = 0;
                                UploadUsage = 0;
                                stopwatch.Restart();
                            }
                        }
                    };
                    m_EtwSession.Source.Kernel.UdpIpSend += data =>
                    {
                        if (data.saddr.Equals(NetworkIp))
                        {
                            UploadUsage += data.size;
                            if (AppsUsage.ContainsKey(data.ProcessName))
                            {
                                AppsUsage[data.ProcessName].CurrentUploadInBytes += data.size;
                                AppsUsage[data.ProcessName].TotalUploadInBytes += data.size;
                            }
                            else
                                AppsUsage.Add(data.ProcessName, new AppUsage(data.ProcessName, 0, data.size));
                            if (stopwatch.ElapsedMilliseconds > 1000)
                            {
                                PrepareData(UploadUsage, DownloadUsage);
                                DownloadUsage = 0;
                                UploadUsage = 0;
                                stopwatch.Restart();
                            }
                        }
                    };
                    m_EtwSession.Source.Kernel.TcpIpSend += data =>
                    {
                        if (data.saddr.Equals(NetworkIp))
                        {
                            UploadUsage += data.size;
                            if (AppsUsage.ContainsKey(data.ProcessName))
                            {
                                AppsUsage[data.ProcessName].CurrentUploadInBytes += data.size;
                                AppsUsage[data.ProcessName].TotalUploadInBytes += data.size;
                            }
                            else
                                AppsUsage.Add(data.ProcessName, new AppUsage(data.ProcessName, 0, data.size));
                            if (stopwatch.ElapsedMilliseconds > 1000)
                            {
                                PrepareData(UploadUsage, DownloadUsage);
                                DownloadUsage = 0;
                                UploadUsage = 0;
                                stopwatch.Restart();
                            }
                        }
                    };
                    m_EtwSession.Source.Process();
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class AppUsage : INotifyPropertyChanged
    {
        public string AppName { get; set; }
        public string CurrentUpload { get; set; }
        public string CurrentDownload { get; set; }
        public string TotalUpload { get; set; }
        public string TotalDownload { get; set; }
        public decimal CurrentUploadInBytes { get; set; }
        public decimal CurrentDownloadInBytes { get; set; }
        public decimal TotalUploadInBytes { get; set; }
        public decimal TotalDownloadInBytes { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public AppUsage(string AppName, decimal DownloadInBytes, decimal UploadInBytes)
        {
            if (AppName == "")
                this.AppName = "UnkownName";
            else
                this.AppName = AppName;
            this.CurrentUpload = "0";
            this.CurrentDownload = "0";
            this.CurrentUploadInBytes = 0;
            this.CurrentDownloadInBytes = 0;
            this.TotalDownload = "0";
            this.TotalUpload = "0";
            this.TotalUploadInBytes = 0;
            this.TotalDownloadInBytes = 0;
            GetCurrentAndTotalString(DownloadInBytes, UploadInBytes);
        }
        private void GetCurrentAndTotalString(decimal DownloadInBytes, decimal UploadInBytes)
        {
            this.CurrentUploadInBytes += UploadInBytes;
            this.CurrentDownloadInBytes += DownloadInBytes;
            this.TotalDownloadInBytes += TotalUploadInBytes;
            this.TotalDownloadInBytes += DownloadInBytes;
        }
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
