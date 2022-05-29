using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Diagnostics;

namespace ImageAverage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter file folder..."); // Give the user instructions
            string folder = Console.ReadLine(); // Get the entered filepath

            Stopwatch timer = new Stopwatch();
            timer.Start();

            if (!Directory.Exists(folder)) // Check if the folder exists
                return; // Return

            string resultPath = Path.Combine(folder, "result.png"); // Get file path for result

            string[] files = Directory.GetFiles(folder).Where(file => Path.GetExtension(file).ToLower() == ".png").Where(file => file != resultPath).ToArray(); // Get all files in the target folder with the .png extension

            if (files.Length == 0) // If there are no files with the extension png 
                return; // Return

            Bitmap[] images = new Bitmap[files.Length]; // Generate array bitmaps
            for (int i = 0; i < files.Length; i++) // Itterate over all files
                images[i] = new Bitmap(files[i]); // Populate array

            images = images.Where(image => image.Size == images[0].Size).ToArray(); // Remove images that arent the sime size as the first image

            Bitmap result = new Bitmap(images[0].Width, images[0].Height); // Generate output image

            double progress = 1; // Keep count of the progress
            double total = result.Width * result.Height; // Calculate total amount of pixels

            Console.WriteLine($"Progress... 0%"); // Start displaying progress

            for (int x = 0; x < result.Width; x++) // Itterate over every pixel on the X axis
            {
                for(int y = 0; y < result.Height; y++) // Itterate over every pixel on the Y axis
                {
                    Console.WriteLine($"Progress... {progress++ / total * 100}%"); // Display the progress

                    int[] argb = { 0, 0, 0, 0 }; // Alpha, Red, Green, Blue
                    for(int i = 0; i < images.Length; i++) // Itterate over all the images
                    {
                        Color col = images[i].GetPixel(x, y); // Get the color at the specific pixel

                        argb[0] += col.A; // Add A value to sum
                        argb[1] += col.R; // Add R value to sum
                        argb[2] += col.G; // Add G value to sum
                        argb[3] += col.B; // Add B value to sum
                    }

                    argb = argb.Select(val => val/images.Length).ToArray(); // Divide every element in the array to get the average
                    Color res = Color.FromArgb(argb[0] < 125 ? 0 : 255, argb[1], argb[2], argb[3]); // Generate color based on the average of the sum

                    result.SetPixel(x, y, res); // Set the pixel in the output texture
                }
            }

            try
            {
                if(File.Exists(resultPath)) // Check if the file exists
                    File.Delete(resultPath); // Delete the image if it exists before saving it
                result.Save(resultPath); // Save image to the same folder with the name "result.png"
            } catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red; // Change text color to red
                Console.Error.WriteLine(ex.StackTrace); // Print stack trace
            }
            timer.Stop();

            Console.WriteLine($"Image generated, path: {resultPath}, Time: {timer.Elapsed}"); // Tell the user we are done
            Console.ReadKey(true); // Close the program if a key gets pressed
        }
    }
}
