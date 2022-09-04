using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
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
            GetCurrentDownloadAndUploadUsage.CancelAsync();
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
