using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;




namespace Snake_v1
{
    public static class Images
    {

        public readonly static ImageSource Empty = LoadIamge("Empty.png");
        public readonly static ImageSource Body = LoadIamge("Body.png");
        public readonly static ImageSource Head = LoadIamge("Head.png");
        public readonly static ImageSource Food = LoadIamge("Food.png");
        public readonly static ImageSource DeadBody = LoadIamge("DeadBody.png");
        public readonly static ImageSource DeadHead = LoadIamge("DeadHead.png");

        private static ImageSource LoadIamge(String fileName)
        {
            return new BitmapImage(new Uri($"Assets/{fileName}",UriKind.Relative));
        }


    }
}
