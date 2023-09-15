using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using Aneka.Entity;
using Aneka.Threading;

namespace Aneka.Examples.ThreadDemo
{
    /// <summary>
    /// <para>
    /// Class <i><b>Program</b></i>. Virtualizes the execution of the 
    /// <see cref="T:Aneka.Examples.ThreadDemo.WarholFilter" /> by using
    /// the Grid Thread Programming Model.
    /// </para>
    /// <para>
    /// The class creates a <see cref="T:Aneka.Entity.AnekaApplication{W,M}"/>
    /// instance configured for the <i>Grid Thread Programming Model</i> and 
    /// provides a virtualization feature to the execution of the <see cref="T:Aneka.Examples.ThreadDemo.WarholFilter" />
    /// The application takes an image as input, creates four copies of it and 
    /// applies the filter on each image parallely, then it waits for the results
    /// and compose the four images into a one single image.
    /// </para>
    /// <para>
    /// The demo shows:
    /// <list type="bullet">
    /// <item>How to use the <see cref="T:Aneka.Examples.AnekaThread" /> APIs.</item>
    /// <item>How to set up the <see cref="T:Aneka.Entity.AnekaApplication{W,M}"/> 
    /// instance for the Grid Thread Programming Model.</item>
    /// <item>How to manage the execution of the <see cref="T:Aneka.Entity.AnekaApplication{W,M}" /> instance.</item>
    /// <item>How to handle events and Process results (<see cref="T:Aneka.Entity.AnekaApplication{W,M}.ApplicationFinished"/>. </item>
    /// </list>
    /// </para>
    /// </summary>
    class Program
    {

        /// <summary>
        /// Main application entry point. Parses the <paramref name="args"/> array and
        /// starts the <see cref="T:Aneka.Examples.ThreadDemo.WarholApplication" /> to
        /// perform the filtering.
        /// </summary>
        /// <param name="args">A <see cref="T:System.Array" /> of <see langword="string" /> 
        /// containing the command line parameters of the application.</param>
        static void Main(string[] args)
        {
            

                    // ok at this point we have the following conditions
                    // 1. inputPath exists
                    // 2. confFile exists
                    // we can start the application..

                    WarholApplication app = new WarholApplication();
            
                    app.InputPath = "G:/My Drive/ANEKA/AnekaPrograms/ThreadExamples/Warholizer/marilyn.jpg";
                    app.OutputPath = "G:/My Drive/ANEKA/AnekaPrograms/ThreadExamples/Warholizer/output.jpg";
                    app.ConfigPath = "G:/My Drive/ANEKA/AnekaPrograms/conf.xml";
                    try
                    {
                        app.Run();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("warholizer: [Error] exception:");
                        Console.WriteLine("\tMessage: " + ex.Message);
                        Console.WriteLine("\tStacktrace: " + ex.StackTrace);
                        Console.WriteLine("EXIT");

                        string path = Util.GetProjectDir("Warhol", true);
                        path = System.IO.Path.Combine(path, IOUtil.GetErrorLogFileName());
                        IOUtil.DumpErrorReport(path, ex, "Aneka Thread Demo - Error log");

                        Console.WriteLine("Error log saved to: {0}", path);
                    }
            finally
            {
                
                Logger.Stop();
            }

        }

    }
            
        }
  
