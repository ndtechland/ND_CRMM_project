using CRM.Models.Crm;
using Microsoft.AspNetCore.Hosting;

namespace CRM.IUtilities
{
    public  class FileOperation
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileOperation(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public string SaveBase64Image(string folderName, IFormFile file, string[] allowedExtensions)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    string extension = Path.GetExtension(file.FileName);
                    if (!allowedExtensions.Contains(extension.ToLower()))
                    {
                        return null;
                    }

                    string fileName = $"{DateTime.Now.Ticks}{extension}";
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    return fileName;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
                return null;
            }
        }
    }
}
