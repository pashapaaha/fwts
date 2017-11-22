using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using System.IO;

namespace fwts
{
    public struct fileWatchInfo
    {
        public string act;
        public string path;
        public fileWatchInfo(string a, string b)
        {
            act = a;
            path = b;
        }
    }
    public partial class Service3 : ServiceBase
    {

        Logger logger;

        public Service3()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            logger = new Logger();
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }



        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(1000);
        }
    }

    class Logger
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;

        public Logger()
        {
            watcher = new FileSystemWatcher("D:\\Temp");
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
            initTimer();
        }

        System.Timers.Timer timer;
        List<fileWatchInfo> fileInfoList = new List<fileWatchInfo>();

        public void initTimer()
        {
            timer = new System.Timers.Timer(15000);
            timer.Elapsed += HandleTimer;
            timer.Start();
        }

        private void HandleTimer(Object source, ElapsedEventArgs e)
        {
            if(fileInfoList.Count < 1)
            {
                RecordEntry();
                return;
            }
            foreach(fileWatchInfo fi in fileInfoList)
            {
                RecordEntry(fi.act, fi.act);
                //writer.WriteLine(String.Format("{0} файл {1} был {2}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), fi.path, fi.act));
                //writer.Flush();
            }
            fileInfoList.Clear();
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }

        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "переименован в " + e.FullPath;
            string filePath = e.OldFullPath;
            fileInfoList.Add(new fileWatchInfo(fileEvent, filePath));
            //RecordEntry(fileEvent, filePath);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "изменен";
            string filePath = e.FullPath;
            fileInfoList.Add(new fileWatchInfo(fileEvent, filePath));
            //RecordEntry(fileEvent, filePath);
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "создан";
            string filePath = e.FullPath;
            fileInfoList.Add(new fileWatchInfo(fileEvent, filePath));
            //RecordEntry(fileEvent, filePath);
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "Удален";
            string filePath = e.FullPath;
            fileInfoList.Add(new fileWatchInfo(fileEvent, filePath));
            //RecordEntry(fileEvent, filePath);
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter("D:\\templog.txt", true))
                {
                    writer.WriteLine(String.Format("{0} файл {1} был {2}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }
        private void RecordEntry()
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter("D:\\templog.txt", true))
                {
                    writer.WriteLine(String.Format("{0} Без изменений", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")));
                    writer.Flush();
                }
            }
        }
    }
}
