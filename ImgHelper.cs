using System;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace library.img {
    /// <summary>
    /// Descripción breve de ImgHelper.
    /// </summary>
    public class ImgHelper {

        public System.Drawing.Bitmap objBMP;
        public System.Drawing.Graphics objGraphics;
        public System.Drawing.Font objFont;
        public System.Drawing.Brush objBrush;

        /// <summary>
        /// Creates the basic objBMP.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="size"></param>
        public ImgHelper(int width, int size) {
            this.objBMP=new Bitmap(width, size);
            //' Create a graphics object to work with from the BMP
            this.objGraphics=System.Drawing.Graphics.FromImage(this.objBMP);
        }


        public System.Drawing.Bitmap GetImg() {
            return this.objBMP;
        }

        public void Font(string family, int size) {
            this.objFont=new Font(family, size);
        }

        public void Brush(Brush theBrushes) {
            this.objBrush=theBrushes;
        }


        public void RotateText(string text, int x, int y, float angloDegrees) {

            //this.objGraphics.DrawString (text, this.objFont, this.objBrush , x, y);

            float width=this.objGraphics.MeasureString(text, this.objFont).Width;
            float height=this.objGraphics.MeasureString(text, this.objFont).Height;

            double angle=(angloDegrees/180)*Math.PI;
            this.objGraphics.TranslateTransform(
            (this.objBMP.Width+(float)(height*Math.Sin(angle))-(float)(width*Math.Cos(angle)))/2,
            (this.objBMP.Height-(float)(height*Math.Cos(angle))-(float)(width*Math.Sin(angle)))/2);


            this.objGraphics.RotateTransform((int)angloDegrees);
            this.objGraphics.DrawString(text, this.objFont, this.objBrush, x, y);
            this.objGraphics.ResetTransform();

        }


        public void DrawString(string text, int x, int y) {
            this.objGraphics.DrawString(text, this.objFont, this.objBrush, x, y);
        }

        public void T() {

            string texto="hola mundo cruel";
            //float height = 	this.objGraphics.MeasureString(texto,this.objFont).Width;
            //80,200
            this.objGraphics.TranslateTransform(0, 190);
            this.objGraphics.RotateTransform(-45);
            this.objGraphics.DrawString(texto, this.objFont, this.objBrush, 0, 0);

            this.objGraphics.DrawString("sdfasdf", this.objFont, this.objBrush, 12, 12);

            this.objGraphics.DrawString("aer aer aaer aer", this.objFont, this.objBrush, 24, 24);

            this.objGraphics.DrawString("aer aer aaer aer", this.objFont, this.objBrush, 36, 36);

            this.objGraphics.ResetTransform();

        }


    }
}


//
//public ImgHelper()		
//{
//this.objBMP = new Bitmap(100, 100);
//
//			//' Create a graphics object to work with from the BMP
//this.objGraphics = System.Drawing.Graphics.FromImage(this.objBMP);
//			
//			
//this.objGraphics.RotateTransform (45);
//	
//StringFormat format = new StringFormat   (StringFormatFlags.NoClip);
//format.Alignment =  StringAlignment.Center;
//format.LineAlignment =  StringAlignment.Center;
//this.objFont = new Font("Arial", 16, FontStyle.Bold);
//
//this.objGraphics.DrawString ("A simple TextString ", this.objFont, Brushes.White, 0, 0, format);
//
//}
//
//
//public ImgHelper(int width, int height)		
//{
//this.objBMP = new Bitmap(width, height);
//
//			//' Create a graphics object to work with from the BMP
//this.objGraphics = System.Drawing.Graphics.FromImage(this.objBMP);
//
//}
//
//public System.Drawing.Bitmap GetObjBMP()
//{
//return this.objBMP;
//}
//
//public System.Drawing.Bitmap InsertText(string text, int angle)
//{
//this.objGraphics.RotateTransform (45);
//	
//StringFormat format = new StringFormat   (StringFormatFlags.NoClip);
//format.Alignment =  StringAlignment.Center;
//format.LineAlignment =  StringAlignment.Center;
//this.objFont = new Font("Arial", 16, FontStyle.Bold);
//
//this.objGraphics.DrawString ("A simple TextString ", this.objFont, Brushes.White, 0, 0, format);
//
//}
//



//public ImgHelper(string T)		
//{
//			//Create new image - bitmap
//this.objBMP = new Bitmap(100, 100);
//
//			//' Create a graphics object to work with from the BMP
//this.objGraphics = System.Drawing.Graphics.FromImage(this.objBMP);
//
//			//' Fill the image with background color
//this.objGraphics.Clear(Color.Green);
//
//			//' Set anti-aliasing for text to make it better looking
//this.objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
//
//			//' Configure font to use for text
//this.objFont = new Font("Arial", 16, FontStyle.Bold);
//
//			//' Write out the text
//this.objGraphics.DrawString("ASP 101", this.objFont, Brushes.White,  3, 3);
//
//		
//}