using System.Net.Sockets;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using Image = System.Drawing.Image;

UdpClient server = new UdpClient(4678);
var remoteEP = new IPEndPoint(IPAddress.Any, 0);

while (true)
{
    var result = await server.ReceiveAsync();
    new Task(async () =>
    {
        remoteEP = result.RemoteEndPoint;
        while (true)
        {
            var img = TakeScreenshot();
            var imgByte = ImageToByte(img);
            var chunk = imgByte.Chunk(ushort.MaxValue - 29);
            foreach (var c in chunk)
            {
                await server.SendAsync(c, c.Length, remoteEP);
            }
        }

    }).Start();
}



Image TakeScreenshot()
{
    Bitmap bitmap = new Bitmap(1920, 1080);

    Graphics graphics = Graphics.FromImage(bitmap);
    graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

    return bitmap;
}

byte[] ImageToByte(Image image)
{
    using (var stream = new MemoryStream())
    {
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

        return stream.ToArray();
    }
}