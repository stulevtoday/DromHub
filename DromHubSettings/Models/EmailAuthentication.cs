using System;

namespace DromHubSettings.Models
{
    public class EmailAuthentication
    {
        public Guid Id { get; set; }
        public string DownloadEmail { get; set; }
        public string DownloadPassword { get; set; }
        public string UploadEmail { get; set; }
        public string UploadPassword { get; set; }
    }
}
