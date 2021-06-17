using System.IO;
using XCourierApp;
using XCourierApp.Storage;
using XCourierApp.Storage.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(StorageFileAccess))]
namespace XCourierApp.Storage.Droid
{
    public class StorageFileAccess : IStorageFileAccess
    {

        public string GetStorageFileLocation()
        {
            //var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Constants.OFFLINE_DATABASE_NAME);

            // trying this way
            // var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Constants.OFFLINE_DATABASE_NAME);

            var path = Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath, Constants.OFFLINE_DATABASE_NAME);

            // trying this way
            // var path = Path.Combine(, Constants.OFFLINE_DATABASE_NAME);


            // var path = Path.Combine()

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            return path;
        }
	} // class
} // namespace