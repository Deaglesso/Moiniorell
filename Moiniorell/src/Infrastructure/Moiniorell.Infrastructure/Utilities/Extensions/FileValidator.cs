using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static async Task<string> CreateFileAsync(this IFormFile file, string root, params string[] folders)
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
            return filename;
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
