using Raylib_cs;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Nodica;

public class PersianText : Node2D
{
    private Texture2D texture;
    private string textToRender = "سلام";
    private string fontFamily = "Arial"; // Consider using a font that fully supports Persian like Tahoma, B Nazanin, etc.
    private int fontSize = 20;
    private System.Drawing.Color textColor = System.Drawing.Color.White;

    public PersianText()
    {
        LoadTextTexture();
    }

    private void LoadTextTexture()
    {
        // Dispose of the old texture if it exists
        //if (texture.Id != 0)
        //{
        //    Raylib.UnloadTexture(texture);
        //}
        //
        //// Measure the text to determine bitmap size
        //using (var tempBitmap = new Bitmap(1, 1))
        //using (var graphics = Graphics.FromImage(tempBitmap))
        //using (var font = new Font(fontFamily, fontSize))
        //{
        //    var stringFormat = new StringFormat
        //    {
        //        FormatFlags = StringFormatFlags.DirectionRightToLeft,
        //        Alignment = StringAlignment.Near,
        //        LineAlignment = StringAlignment.Near
        //    };
        //    SizeF textSize = graphics.MeasureString(textToRender, font, new PointF(0, 0), stringFormat);
        //
        //    // Create the bitmap with appropriate size
        //    using (var bitmap = new Bitmap((int)Math.Ceiling(textSize.Width), (int)Math.Ceiling(textSize.Height)))
        //    using (var g = Graphics.FromImage(bitmap))
        //    {
        //        g.Clear(System.Drawing.Color.Transparent);
        //        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias; // Optional: for smoother text
        //
        //        using (var fontToUse = new Font(fontFamily, fontSize))
        //        {
        //            var drawPoint = new PointF(bitmap.Width, 0); // Start drawing from the right
        //            g.DrawString(textToRender, fontToUse, new SolidBrush(textColor), drawPoint, stringFormat);
        //        }
        //
        //        // Save the Bitmap to a MemoryStream in PNG format
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            bitmap.Save(ms, ImageFormat.Png);
        //            byte[] imageData = ms.ToArray();
        //
        //            // Load the image from memory
        //            Image image = Raylib.LoadImageFromMemory(".png", imageData);
        //
        //            // Check if the image is valid
        //            if (image.Width == 0 || image.Height == 0)
        //            {
        //                Raylib.TraceLog(TraceLogLevel.Error, "Failed to load image from memory.");
        //                texture = new Texture2D(); // Return an invalid texture
        //            }
        //            else
        //            {
        //                // Load the texture from the image
        //                texture = Raylib.LoadTextureFromImage(image);
        //                Raylib.UnloadImage(image); // Unload the CPU image once the texture is on the GPU
        //            }
        //        }
        //    }
        //}
    }

    protected override void Draw()
    {
        base.Draw();

        //if (texture.Id != 0)
        //{
        //    // Draw the texture on screen
        //    Raylib.DrawTexture(texture, GlobalPosition.X, GlobalPosition.Y, Color.White);
        //}
        //else
        //{
        //    // Draw an error message if texture failed to load
        //    Raylib.DrawText("Failed to load texture", (int)GlobalPosition.X, (int)GlobalPosition.Y, 20, Color.Red);
        //}
    }
}