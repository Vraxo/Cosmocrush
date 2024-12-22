using Raylib_cs;
using System.Drawing;
using System.IO;

namespace Nodica
{
    public class PersianText : Node2D
    {
        private Texture2D texture;

        public PersianText()
        {
            // Create texture from GDI+ (System.Drawing) rendered text
            var bitmap = new Bitmap(200, 50);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(System.Drawing.Color.Transparent); // Ensure transparent background

                using (var font = new System.Drawing.Font("Arial", 20)) // Choose an appropriate font for Persian text
                {
                    var stringFormat = new StringFormat();
                    stringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft; // Ensures RTL text for Persian
                    graphics.DrawString("سلام", font, Brushes.White, new PointF(10, 10), stringFormat);
                }
            }

            // Save the Bitmap to a MemoryStream
            texture = LoadTextureFromBitmap(bitmap);
        }

        protected override void Draw()
        {
            base.Draw();

            if (texture.Id != 0)
            {
                // Draw the texture on screen
                Raylib.DrawTexture(texture, 100, 100, Color.White);
            }
            else
            {
                // Draw an error message if texture failed to load
                Raylib.DrawText("Failed to load texture", 100, 100, 20, Color.Red);
            }
        }

        private Texture2D LoadTextureFromBitmap(Bitmap bitmap)
        {
            // Convert the Bitmap to a MemoryStream in PNG format
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageData = ms.ToArray();

                // Load the texture from memory
                Image image = Raylib.LoadImageFromMemory("png", imageData);

                // Check if the image is valid
                if (image.Width == 0 || image.Height == 0)
                {
                    Raylib.TraceLog(TraceLogLevel.Error, "Failed to load image from memory.");
                    return new Texture2D(); // Return an invalid texture
                }

                // Load the texture from the image
                return Raylib.LoadTextureFromImage(image);
            }
        }
    }
}
