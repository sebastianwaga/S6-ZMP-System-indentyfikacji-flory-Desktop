using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VirtualHerbarium.AdminPanel.Services.Offline
{
    public static class PhotoCacheService
    {
        private static readonly string CacheDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VirtualHerbariumAdmin", "cache", "photos");

        static PhotoCacheService()
        {
            if (!Directory.Exists(CacheDir))
                Directory.CreateDirectory(CacheDir);
        }

        private static string GetPhotoPath(string photoId)
            => Path.Combine(CacheDir, $"{photoId}.jpg");

        public static async Task<BitmapImage?> LoadPhotoAsync(string photoId, string url)
        {
            CleanupOldPhotos();

            var path = GetPhotoPath(photoId);

            if (File.Exists(path))
                return LoadFromDisk(path);

            if (!InternetService.Instance.IsOnline)
                return null;

            var bitmap = await DownloadFromApi(url);

            if (bitmap != null)
                SaveToDisk(bitmap, path);

            return bitmap;
        }


        private static BitmapImage? LoadFromDisk(string path)
        {
            try
            {
                var img = new BitmapImage();
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);

                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = fs;
                img.EndInit();
                img.Freeze();

                return img;
            }
            catch
            {
                return null;
            }
        }

        private static async Task<BitmapImage?> DownloadFromApi(string url)
        {
            try
            {
                var clean = url.TrimStart('/');
                var fullUrl = $"https://ezielnik-production.up.railway.app/{clean}";

                using var http = new HttpClient();
                var bytes = await http.GetByteArrayAsync(fullUrl);

                if (bytes.Length == 0)
                    return null;

                using var ms = new MemoryStream(bytes);
                var img = new BitmapImage();

                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = ms;
                img.EndInit();
                img.Freeze();

                return img;
            }
            catch
            {
                return null;
            }
        }

        private static void SaveToDisk(BitmapImage img, string path)
        {
            try
            {
                using var fileStream = new FileStream(path, FileMode.Create);

                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                encoder.Save(fileStream);
            }
            catch
            {

            }
        }
        public static void CleanupOldPhotos()
        {
            try
            {
                var dir = new DirectoryInfo(CacheDir);
                if (!dir.Exists)
                    return;

                foreach (var file in dir.GetFiles("*.jpg"))
                {
                    if (file.CreationTimeUtc < DateTime.UtcNow.AddDays(-7))
                    {
                        file.Delete();
                    }
                }
            }
            catch
            {

            }
        }

    }
}
