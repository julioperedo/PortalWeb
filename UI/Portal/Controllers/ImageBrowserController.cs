using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace Portal.Controllers
{
    [Obsolete]
    public class ImageBrowserController : EditorImageBrowserController
    {
        private const string contentFolderRoot = "images";
        private const string folderName = "";
        private static readonly string[] foldersToCopy = new[] {
            contentFolderRoot, Path.Combine(contentFolderRoot, "events"), Path.Combine(contentFolderRoot, "lines"), Path.Combine(contentFolderRoot, "products"),
            Path.Combine(contentFolderRoot, "userdata"), Path.Combine(contentFolderRoot, "productdocs")
        };
        private readonly JsonSerializerSettings serializeOptions = new() { ContractResolver = new DefaultContractResolver() };

        public override string ContentPath => CreateUserFolder();

        public ImageBrowserController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }

        public override string GetFileName(IFormFile file)
        {
            var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            string name = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
            return name;
        }

        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(contentFolderRoot, folderName);
            var path = HostingEnvironment.WebRootFileProvider.GetFileInfo(virtualPath).PhysicalPath;

            if (path != null && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                foreach (var sourceFolder in foldersToCopy)
                {
                    CopyFolder(HostingEnvironment.WebRootFileProvider.GetFileInfo(sourceFolder).PhysicalPath, path);
                }
            }
            return virtualPath;
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }
            foreach (var file in Directory.EnumerateFiles(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }
            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }

        public override JsonResult Read(string path)
        {
            var data = base.Read(path);
            var list = ((FileBrowserEntry[])data.Value).Select(s => new { s.Name, s.Size, s.EntryType }).ToList();
            return Json(list, serializeOptions);
        }

        [AcceptVerbs("POST")]
        public override ActionResult Upload(string path, IFormFile file)
        {
            var fullPath = NormalizePath(path);
            if (AuthorizeUpload(fullPath, file))
            {
                SaveFile(file, fullPath);
                var result = new FileBrowserEntry { Size = file.Length, Name = GetFileName(file) };
                return Json(result, serializeOptions);
            }
            throw new Exception("Forbidden");
        }

    }

}