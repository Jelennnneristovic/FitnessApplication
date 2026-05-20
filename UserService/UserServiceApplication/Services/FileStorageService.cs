using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace UserServiceApplication.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadFolder = "uploads/users";

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task<string> SaveUserImage(Guid userId, IFormFile file)
        {
            // 1. Putanja do foldera: wwwroot/uploads/users
            var rootPath = Path.Combine(_env.WebRootPath, _uploadFolder);

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            // 2. Pronađi i obriši stare slike tog korisnika (bilo koji format)
            var existingFiles = Directory.GetFiles(rootPath, $"{userId}.*");
            foreach (var oldFile in existingFiles)
            {
                File.Delete(oldFile);
            }

            // 3. Pripremi novu putanju
            var extension = Path.GetExtension(file.FileName).ToLower();
            var fileName = $"{userId}{extension}";
            var fullPath = Path.Combine(rootPath, fileName);

            // 4. Sačuvaj fajl
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Vraćamo relativnu putanju za bazu (npr. /uploads/users/1.jpg)
            return Task.FromResult($"/{_uploadFolder}/{fileName}");
        }
    }
}
