using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using PcInfoApp.Util;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

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
            DispatcherTimer UsageTimer = new DispatcherTimer();
            UsageTimer.Interval = TimeSpan.FromMilliseconds(1000);
            UsageTimer.Tick += UsageTimer_Tick;
            UsageTimer.Start();
        }

        private void UsageTimer_Tick(object? sender, EventArgs e)
        {
            GetCurrentDownloadAndUploadUsage.CancelAsync();
            GetCurrentDownloadAndUploadUsage = new BackgroundWorker();
            GetCurrentDownloadAndUploadUsage.WorkerSupportsCancellation = true;
            GetCurrentDownloadAndUploadUsage.DoWork += GetCurrentDownloadAndUploadUsage_DoWork;
            GetCurrentDownloadAndUploadUsage.RunWorkerAsync();
        }
        private void GetCurrentDownloadAndUploadUsage_DoWork(object? sender, DoWorkEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            TraceEventSession m_EtwSession;
            decimal UploadUsage = 0;
            decimal DownloadUsage = 0;
            decimal UploadWhileRunnigDecimal = 0;
            decimal DownloadWhileRunnigDecimal = 0;
            int time = 0;
            string UploadUsageType, DownloadUsageType;
            if (AppsUsage.Count != 0)
            {
                foreach (AppUsage app in AppsUsage.Values)
                {
                    app.CurrentUploadInBytes = 0;
                    app.CurrentDownloadInBytes = 0;
                }
            }
            System.Net.IPAddress PcIp = System.Net.IPAddress.Parse("127.0.0.1");
            try
            {
                using (m_EtwSession = new TraceEventSession("MyKernelAndClrEventsSession"))
                {
                    m_EtwSession.EnableKernelProvider(KernelTraceEventParser.Keywords.NetworkTCPIP);
                    m_EtwSession.Source.Kernel.TcpIpRecv += data =>
                    {
                        if (data.daddr.Address != PcIp.Address)
                        {
                            DownloadUsage += data.size;
                            if (AppsUsage.ContainsKey(data.ProcessName))
                            {
                                AppsUsage[data.ProcessName].CurrentDownloadInBytes += data.size;
                                AppsUsage[data.ProcessName].TotalDownloadInBytes += data.size;
                            }
                            else
                                AppsUsage.Add(data.ProcessName, new AppUsage(data.ProcessName, data.size, 0));
                            if (stopwatch.ElapsedMilliseconds > 2000)
                            {
                                time = (int)stopwatch.ElapsedMilliseconds;
                                stopwatch.Stop();
                                m_EtwSession.Stop();
                            }
                        }
                    };
                    m_EtwSession.Source.Kernel.UdpIpRecv += data =>
                    {
                        if (data.daddr.Address != PcIp.Address)
                        {
                            DownloadUsage += data.size;
                            if (AppsUsage.ContainsKey(data.ProcessName))
                            {
                                AppsUsage[data.ProcessName].CurrentDownloadInBytes += data.size;
                                AppsUsage[data.ProcessName].TotalDownloadInBytes += data.size;
                            }
                            else
                                AppsUsage.Add(data.ProcessName, new AppUsage(data.ProcessName, data.size, 0));
                            if (stopwatch.ElapsedMilliseconds > 2000)
                            {
                                time = (int)stopwatch.ElapsedMilliseconds;
                                stopwatch.Stop();
                                m_EtwSession.Stop();
                            }
                        }
                    };
                    m_EtwSession.Source.Kernel.UdpIpSend += data =>
                    {
                        if (data.daddr.Address != PcIp.Address)
                        {
                            UploadUsage += data.size;
                            if (AppsUsage.ContainsKey(data.ProcessName))
                            {
                                AppsUsage[data.ProcessName].CurrentUploadInBytes += data.size;
                                AppsUsage[data.ProcessName].TotalUploadInBytes += data.size;
                            }
                            else
                                AppsUsage.Add(data.ProcessName, new AppUsage(data.ProcessName, 0, data.size));
                            if (stopwatch.ElapsedMilliseconds > 2000)
                            {
                                time = (int)stopwatch.ElapsedMilliseconds;
                                stopwatch.Stop();
                                m_EtwSession.Stop();
                            }
                        }
                    };
                    m_EtwSession.Source.Kernel.TcpIpSend += data =>
                    {
                        if (data.daddr.Address != PcIp.Address)
                        {
                            UploadUsage += data.size;
                            if (AppsUsage.ContainsKey(data.ProcessName))
                            {
                                AppsUsage[data.ProcessName].CurrentUploadInBytes += data.size;
                                AppsUsage[data.ProcessName].TotalUploadInBytes += data.size;
                            }
                            else
                                AppsUsage.Add(data.ProcessName, new AppUsage(data.ProcessName, 0, data.size));
                            if (stopwatch.ElapsedMilliseconds > 2000)
                            {
                                time = (int)stopwatch.ElapsedMilliseconds;
                                stopwatch.Stop();
                                m_EtwSession.Stop();
                            }
                        }
                    };
                    m_EtwSession.Source.Process();
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                return;
            }
            time /= 1000;
            if (time != 0)
                UploadUsage /= time;
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
            if (time != 0)
                DownloadUsage /= time;
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
            if (this.UploadWhileRunnig.ToString().Length > 4 && this.UploadWhileRunnigType != "TB")
                this.UploadWhileRunnig = decimal.Parse(this.UploadWhileRunnig.ToString().Substring(0, 4));
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
            if (this.DownloadWhileRunnig.ToString().Length > 4 && this.DownloadWhileRunnigType != "TB")
                this.DownloadWhileRunnig = decimal.Parse(this.DownloadWhileRunnig.ToString().Substring(0, 4));
            this.DownloadUsageWhileAppRunning = this.DownloadWhileRunnig + this.DownloadWhileRunnigType;
            OnPropertyChanged("DownloadUsageWhileAppRunning");
            if (AppsUsage.Count != 0)
            {
                foreach (AppUsage app in AppsUsage.Values)
                {
                    if (app.CurrentUploadInBytes == 0)
                        app.CurrentUpload = "0KB";
                    else if ((app.CurrentUploadInBytes / 1024).ToString().Length > 4)
                    {
                        app.CurrentUpload = (app.CurrentUploadInBytes / 1024).ToString().Substring(0, 4) + "KB";
                    }
                    else
                        app.CurrentUpload = (app.CurrentUploadInBytes / 1024) + "KB";
                    if (app.CurrentDownloadInBytes == 0)
                        app.CurrentDownload = "0KB";
                    else if ((app.CurrentDownloadInBytes / 1024).ToString().Length > 4)
                    {
                        app.CurrentDownload = (app.CurrentDownloadInBytes / 1024).ToString().Substring(0, 4) + "KB";
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
            }
            GetCurrentDownloadAndUploadUsage.CancelAsync();
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
