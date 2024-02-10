using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace Moiniorell.Infrastructure.Utilities.Extensions
{
    public static class FileValidator
    {
        public static bool CheckFileType(this IFormFile file, string type)
        {
            if (file.ContentType.Contains(type))
            {
                return true;
            }
            return false;
        }
        public static bool CheckFileSize(this IFormFile file, int Mb)
        {
            if (file.Length > Mb * 1024 * 1024)
            {
                return true;
            }
            return false;
        }
        public static async Task<string> CreateFileAsync(this IFormFile file, string root, bool cropImage = false, params string[] folders)
        {
            string path = root;
            string filename = Guid.NewGuid().ToString() + file.FileName;
            foreach (var item in folders)
            {
                path = Path.Combine(path, item);
            }
            path = Path.Combine(path, filename);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }
            if (cropImage)
            {
                CropImage(path, 1440, 1080);
            }

            return filename;
        }
        private static void CropImage(string imagePath, int targetWidth, int targetHeight)
        {
            using (Image image = Image.Load(imagePath))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new SixLabors.ImageSharp.Size(targetWidth, targetHeight),
                    Mode = ResizeMode.Crop
                }));

                image.Save(imagePath);
            }
        }
        public static void DeleteFile(this string filename, string root, params string[] folders)
        {
            string path = root;

            foreach (var item in folders)
            {
                path = Path.Combine(path, item);
            }
            path = Path.Combine(path, filename);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

        }

    }
}
