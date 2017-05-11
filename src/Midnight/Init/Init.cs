using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Midnight.Init {
    public class Init {
        public Init() {
            loadStory();
        }

        public async void loadStory() {
            string[] fileName;
            fileName = new string[15] { "X1", "X2", "X3", "X4", "X5", "X10", "X11", "X20", "X21", "X40", "X41", "X110", "X111", "X400", "X401" };
            //读文件
            //创建Uri，注意这里我们把后缀名改成了txt但实际上是db文件
            foreach (var fileN in fileName) {
                Uri uri = new Uri("ms-appx:///Assets/Story/" + fileN + ".txt");
                //用Uri创建StorageFile
                StorageFile originFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                //获取进缓冲区
                var buffer = await FileIO.ReadBufferAsync(originFile);

                //写文件
                //打开localstate文件夹
                StorageFolder storageFolder = ApplicationData.Current.LocalCacheFolder;
                //创建新文件
                Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync(fileN + ".db", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                //打开文件
                StorageFile targetFile = await storageFolder.GetFileAsync(fileN + ".db");
                //获得文件的流
                var stream = await targetFile.OpenAsync(FileAccessMode.ReadWrite);
                //用刚才的buffer写进去
                await stream.WriteAsync(buffer);
            }
        }
    }
}
