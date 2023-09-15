using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Aneka.Examples.ThreadDemo
{
    /// <summary>
    /// <para>
    /// Class <i><b>WarholThread</b></i>. Applies the Warhol effect on a given <see cref="T:System.Drawing.Bitmap">.
    /// </para>
    /// <para>
    /// The Warhol effect is an image filter that when applied to an image produces a simplified version
    /// of the image with a reducet color set. What characterizes this filter is its resemblance to the 
    /// to the paintings of Andy Warhol, who invented this technique. Warhol paintings are characterized 
    /// by a stereographic copy of a given painting. This painting is represented with different color 
    /// sets for each stereographic copy and the color space has been reduced from the original. All these
    /// paintings are then put all together one near the other one in the final painting.
    /// </para>
    /// <para>
    /// This class allows to reduce the color space of <see cref="P:Aneka.Examples.ThreadDemo.WarholFilter.Image" />
    /// and remaps the colors of the image according to the given <see cref="P:Aneka.Examples.ThreadDemo.WarholFilter.Palette" />.
    /// The resulting image is then stored in the <see cref="P:Aneka.Examples.ThreadDemo.Image" /> property.
    /// </para>
    /// </summary>
    [Serializable]
    public class WarholFilter 
    {
        /// <summary>
        /// Common color set 1: Yellow, Blue Navy, Dark Green.
        /// </summary>
        public static readonly Color[] YellowGreenNavy = new Color[3] { Color.Yellow, Color.DarkGreen, Color.Navy };
        /// <summary>
        /// Common color set 2: Fuchsia, Orange, Dark Blue.
        /// </summary>
        public static readonly Color[] FuchsiaOrangeBlue = new Color[3] { Color.Fuchsia, Color.Orange, Color.DarkBlue };
        /// <summary>
        /// Common color set 3: Green, Orange, Gainsboro.
        /// </summary>
        public static readonly Color[] GreenOrangeGainsboro = new Color[3] { Color.Green, Color.Orange, Color.Gainsboro };
        /// <summary>
        /// Common color set 4: Fuchsia, Dark Olive Green, White Smoke.
        /// </summary>
        public static readonly Color[] FuchsiaGreenWhite = new Color[3] { Color.Fuchsia, Color.DarkOliveGreen, Color.WhiteSmoke };

        /// <summary>
        /// <see cref="T:System.Drawing.Image" /> reference that
        /// contains the instance that will be filtered.
        /// </summary>
        protected Bitmap image;
        /// <summary>
        /// Gets or sets the <see cref="T:System.Drawing.Image" /> 
        /// instance that will be filtered. This property is used
        /// either as input or as output of the filtering Process.
        /// </summary>
        public Bitmap Image 
        {
            get { return this.image; }
            set { this.image = value; }
        }
        /// <summary>
        /// Color palette used to remap the color space 
        /// of <see cref="P:Aneka.Examples.ThreadDemo.Warhol.Image" />.
        /// </summary>
        protected Color[] palette;
        /// <summary>
        /// Gets or sets the palette used to remap the color
        /// of <see cref="P:Aneka.Examples.ThreadDemo.Warhol.Image" />.
        /// space of 
        /// </summary>
        public Color[] Palette 
        {
            get { return this.palette; }
            set { this.palette = value; }
        }
        /// <summary>
        /// Applies the filter and processes the image instance referenced by <see cref="P:Aneka.Samples.ThreadDemo.WarholEffet.Image" />
        /// by remapping the color values according to <see cref="P:Aneka.Examples.ThreadDemo.WarholEffect.Palette" />.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException" ><see cref="P:Aneka.Examples.ThreadDemo.WahrolFilter.Image"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentException" ><see cref="P:Aneka.Examples.ThreadDemo.WahrolFilter.Palette"/> is <see langword="null"/> or empty.</exception>
        public void Apply()
        {
            if (this.image == null) 
            {
                throw new ArgumentNullException("Cannot apply the filter to a null image.", "Image"); 
            }
            if ((this.palette == null) || (this.palette.Length == 0)) 
            {
                throw new ArgumentException("The selected palette is null or empty.", "Palette");
            }
            this.image = this.Filter(this.image, this.palette);
        }
        /// <summary>
        /// Applies the Warhol filter to <paramref name="source"/> by using
        /// the color space identified by <paramref name="palette"/>.
        /// </summary>
        /// <param name="source">A <see cref="T:System.Drawing.Bitmap" /> instance that will be filtered.</param>
        /// <param name="palette">An <see cref="t:System.Array" /> of <see cref="T:System.Drawing.Color" /> valued defining the palette to apply.</param>
        /// <returns>Filtered <see cref="T:System.Drawing.Bitmap" /> representing the filtered image.</returns>
        private Bitmap Filter(Bitmap source, Color[] palette) 
        {
            // reorder the palette first..
            Color[] luminance = new Color[palette.Length];
            for (int i = 0; i < palette.Length; i++) 
            {
                Color palSample = palette[i];
                luminance[i] = palSample;
            }
            for(int i = 1; i < palette.Length; i++)
            for (int j = 0; j < i; j++) 
            {
                if (luminance[j].GetBrightness() > luminance[i].GetBrightness())
                {

                    Color swapColor = luminance[j];
                    luminance[j] = luminance[i];
                    luminance[i] = swapColor;
                }
                
            }

            // now we have to pick up the colors
            // and according to their luminosity
            // putting them into classes...
            // the point is that we want the colors
            // equally distributed to the luminance 
            // palette... so we have to identify the
            // color range of the image and equally divide
            // into four classes



            float max = 0.0f;
            float min = 1.0f;
            float mid = 0.0f;

            Bitmap target = new Bitmap(source.Width, source.Height, source.PixelFormat);
            
            for (int x = 0; x < source.Width; x++)
            for (int y = 0; y < source.Height; y++)
            {
                Color sample = source.GetPixel(x, y);
                float b = sample.GetBrightness();
                if (b < min) 
                {
                    min = b;
                }
                if (b > max) 
                {
                    max = b;
                }
                mid = mid + b;
            }

            // now we can compute the range
            // of colors...
            float delta = max - min / luminance.Length;
            mid = mid / (source.Width * source.Height);

            // we want to center the value of mid
            // into the scale in order to do this
            // we simply change the values of the
            // average brightness in the avg array
            float[] brightness = new float[luminance.Length];
            
            // we fix the top and bottom values
            
            this.Rescale(brightness.Length, 0, mid, min, max, brightness);
            brightness[brightness.Length - 1] = max;
            for (int x = 0; x < source.Width; x++)
            for (int y = 0; y < source.Height; y++)
            {
                Color sample = source.GetPixel(x, y);
                float b = sample.GetBrightness();
                for (int i = 0; i < brightness.Length; i++) 
                {
                    if (b <= brightness[i]) 
                    {
                        target.SetPixel(x, y, luminance[i]);
                        break;
                    }
                }
            }
            return target;
        }
        /// <summary>
        /// <para>
        /// Fills the <paramref name="values"/> array with an equi distributed set of values
        /// ranging from <paramref name="min"/> to <paramref name="max"/> by using <paramref name="midPoint"/>
        /// as a starting point.
        /// </para>
        /// <para>
        /// The method applies recursion on the value of <paramref name="delta"/> that together with
        /// <paramref name="start" /> identifies the subarray to fill at each call. The value of <paramref name="midPoint"/>
        /// is used to set the central value of the subarray. Two subarrays are created by using the central position
        /// and on each subarray <see cref="M:Aneka.Examples.ThreadDemo.WarholThread.Rescale" /> is called by computing
        /// the value of <paramref name="delta"/> as (<paramref name="delta"/> / 2) and defining the new value of
        /// <paramref name="midPoint"/> as the:
        /// <list type="bullet">
        /// <item><paramref name="min"/> + (<paramref name="midPoint"/> - <paramref name="min"/>) /2 for the left subarray.</item>
        /// <item><paramref name="midPoint"/> + (<paramref name="max"/> - <paramref name="midPoint"/>) /2 for the right subarray.</item>
        /// </list>
        /// The values of <paramref name="min"/> and <paramref name="max"/> are set accordingly.
        /// </para>
        /// <para>The recursion terminates when the value of <paramref name="delta"/> becomes zero.</para>
        /// </summary>
        /// <param name="delta">Length of the subarray contained in <paramref name="values"/> that will be filled with thresold brightness values.</param>
        /// <param name="start">Position of the first element of the subarray in <paramref name="values"/>.</param>
        /// <param name="midPoint">Starting (central) value of the brightness that will be used to generate all the other values.</param>
        /// <param name="min">Minimum value of the brightness.</param>
        /// <param name="max">Maximum value of the brightness.</param>
        /// <param name="values"><see cref="T:System.Array"/> that will be filled with brightness values.</param>
        protected void Rescale(int delta, int start, float midPoint, float min, float max, float[] values) 
        {
             if (delta > 0) 
            {
                int newDelta = delta / 2;
                if (start + newDelta < values.Length)
                {
                    values[start + newDelta] = midPoint;
                }
                this.Rescale(newDelta, start,  min + (midPoint - min) / 2, min, midPoint, values);
                int newStart = start + newDelta + 1;
                if (newStart < values.Length)
                {
                    this.Rescale(newDelta, newStart, midPoint + (max - midPoint) / 2, midPoint, max, values);
                }
            }
        }

    }
}
