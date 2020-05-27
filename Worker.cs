using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static CallAPIService.Program;


namespace CallAPIService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private FileSystemWatcher _folderWatcher;
        private readonly string _Folder2;
        private readonly string _Folder3;
        private readonly string _FileLogLocation;
        private readonly string _AccessToken;
        private readonly string _BuildID;
        private readonly string _Folder1;
        private readonly string _VSTSOrgName;
        private readonly string _VSTSprojectName;
        private readonly int _ApiCallTime;

        public Worker(ILogger<Worker> logger, IOptions<AppSettings> settings) 
        {

            //Getting all the Settings fron the AppSettings.json
            _logger = logger;
            _Folder1 = settings.Value.Folder1;
            _Folder2 = settings.Value.Folder2;
            _Folder3 = settings.Value.Folder3;
            _FileLogLocation = settings.Value.Filesloglocation;
            _AccessToken = settings.Value.AccessToken;
            _BuildID = settings.Value.BuildID;
            _ApiCallTime = settings.Value.ApiCallTime;
            _VSTSOrgName = settings.Value.OrgName;
            _VSTSprojectName = settings.Value.ProjectName;
        }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
            
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service Starting");
            //check If monitor direcotry's exists and if not prints it to the system event

            if (!Directory.Exists(_Folder1))
            {
                _logger.LogWarning($"Please make sure the FolderToMonitor [{_Folder1}] exists, then restart the service.");
                return Task.CompletedTask;
            }

            //if (!Directory.Exists(_Folder2))
            //{
            //    _logger.LogWarning($"Please make sure the FolderToMonitor [{_Folder2}] exists, then restart the service.");
            //    return Task.CompletedTask;
            //}

            //if (!Directory.Exists(_Folder3))
            //{
            //    _logger.LogWarning($"Please make sure the FolderToMonitor [{_Folder3}] exists, then restart the service.");
            //    return Task.CompletedTask;
            //}

            
            //Watching on Folder 1
            //_logger.LogInformation($"Binding Events from Input Folder: {_Folder1}");
            var NewFolder1Watcher = new FileSystemWatcher
            {
                Path = _Folder1,
                Filters = { "*.zip", "*.txt" },
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName |
                               NotifyFilters.DirectoryName,
                EnableRaisingEvents = true,
            };

            _folderWatcher = NewFolder1Watcher;
            _folderWatcher.Created += Input_OnChanged;
            _folderWatcher.EnableRaisingEvents = true;


            //Watching on Folder 2
            //_logger.LogInformation($"Binding Events from Input Folder: {_Folder2}");
            //var NewFolder2Watcher = new FileSystemWatcher
            //{
            //    Path = _Folder2,
            //    //Filters = { "*.zip", "*.rar" },
            //    Filter = "*.docx",
            //    NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName |
            //                   NotifyFilters.DirectoryName,
            //    EnableRaisingEvents = true,
            //};

            //_folderWatcher = NewFolder2Watcher;
            //_folderWatcher.Created += Input_OnChanged;
            //_folderWatcher.EnableRaisingEvents = true;

            //Watching on Folder 3
            //_logger.LogInformation($"Binding Events from Input Folder: {_Folder3}");
            //var NewFolder3Watcher = new FileSystemWatcher
            //{
            //    Path = _Folder3,
            //    //Filters = { "*.zip", "*.rar" },
            //    Filter = "*.txt",
            //    NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName |
            //                   NotifyFilters.DirectoryName,
            //    EnableRaisingEvents = true,
            //};

            //_folderWatcher = NewFolder3Watcher;
            //_folderWatcher.Created += Input_OnChanged;
            //_folderWatcher.EnableRaisingEvents = true;

            return base.StartAsync(cancellationToken);
        }

        //Logging the files creatd in the folder 
        protected void Input_OnChanged(object source, FileSystemEventArgs e)
        {
            ManualResetEvent syncEvent = new ManualResetEvent(false);
            Thread TypeToLogFile = new Thread(
        () =>
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                _logger.LogInformation($"InBound Change Event Triggered by [{e.FullPath}]");
                string data = ("TriggerNewCIVersion_" + System.IO.Path.GetFileNameWithoutExtension(e.Name) + "_" + DateTime.Now + Environment.NewLine);
                string path = ((_FileLogLocation + "CI_Trigger_Log.txt"));
                if (! File.Exists(_FileLogLocation))
                {
                    System.IO.Directory.CreateDirectory(_FileLogLocation);
                }
                File.AppendAllText(path, data);
                _logger.LogInformation("Done with Inbound Change Event");
            }
        }
    );
            TypeToLogFile.Start();
            TypeToLogFile.Join();

            Thread.Sleep(_ApiCallTime);
            TriggerBuild();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        //Trigger the API call that Triggers the build
        protected async void TriggerBuild()
        {
            try
            {
                
                var personalaccesstoken = _AccessToken;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", personalaccesstoken))));
                    //var CallAPi = new ByteArrayContent(System.Text.ASCIIEncoding.ASCII.GetBytes("{ \"definition\": {\"id\": 113}}"));
                    var CallAPi = new ByteArrayContent(System.Text.ASCIIEncoding.ASCII.GetBytes(_BuildID));
                    CallAPi.Headers.Add("Content-Type", "application/json");

                    //Shows URL
                    Console.WriteLine("https://dev.azure.com/"+_VSTSOrgName+"/"+_VSTSprojectName+"/_apis/build/builds?api-version=5.0");
                    using (HttpResponseMessage response = await client.PostAsync(
                        "https://dev.azure.com/" + _VSTSOrgName + "/" + _VSTSprojectName + "/_apis/build/builds?api-version=5.0", CallAPi))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Service");
            _folderWatcher.EnableRaisingEvents = false;
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing Service");
            _folderWatcher.Dispose();
            base.Dispose();
        }
    }

}

