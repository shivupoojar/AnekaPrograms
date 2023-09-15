#region Namespaces

using System;
using System.Collections.Generic;	// Ilist<...> class.
using System.Text;				    // StringBuilder class.

using System.IO;				    // IOException (IO Errors management)
using System.Drawing;			    // Image and Bitmap classes.

using Aneka.Entity;			        // Aneka Common APIs for all models
using Aneka.Threading;			    // Aneka Thread Model
using System.Threading;			    // ThreadStart (AnekaThread initialization)

#endregion

namespace Aneka.Examples.ThreadDemo
{
    /// <summary>
    /// <para>
    /// Class <i><b>WarholApplication</b></i>. This class manages the execution
    /// of the <see cref="T:Aneka.Examples.ThreadDemo.WarholFilter" /> on Aneka
    /// and thus creating a stereo image of a given picture composed by 4 copies
    /// of the same image on which the filter is applied with different settings.
    /// </para>
    /// <para>
    /// In order to speed up the execution fo the filter the <see cref="T:Aneka.Examples.ThreadDemo.WarholApplication"/>
    /// uses the support given Aneka for execution virtualization and parallelize
    /// the execution of the four filters by using the <i>Grid Thread Programming Model</i>.
    /// In particular it uses the following APIs:
    /// - <see cref="T:Aneka.Threading.AnekaThread" /> for remotely execute the <see cref="T:Aneka.Examples.ThreadDemo.WarholFilter" />.
    /// - <see cref="T:Aneka.Entity.AnekaApplication{W,M}"/> for managing the execution of the remote threads.
    /// </para>
    /// <para>
    /// This class constitutes a very simple example on how to configure the 
    /// <see cref="T:Aneka.Entity.AnekaApplication{W,M}"/> class for the <i>Thread
    /// Programming Model</i> and how to use the basic <see cref="T:Aneka.Threading.AnekaThread" /> 
    /// APIs.
    /// </para>
    /// </summary>
    public class WarholApplication
    {
        #region Properties
        /// <summary>
        /// Path to the input file.
        /// </summary>
        protected string inputPath;
        /// <summary>
        /// Gets or sets the path to the input image
        /// file that will be processed by th filter.
        /// </summary>
        public string InputPath 
        {
            get { return this.inputPath; }
            set 
            { 
                if ((value == null) || (value == string.Empty)) 
                {
                    throw new ArgumentException("The InputPath cannot be null or empty!", "InputPath");
                }
                this.inputPath = value;
            }
        }
        /// <summary>
        /// Path to the configuration file.
        /// </summary>
        protected string configPath;
        /// <summary>
        /// Gets or sets the path to the file
        /// containing the <see cref="T:Aneka.Entity.Configuration" />
        /// object used to connect to Aneka.
        /// </summary>
        /// <remarks>
        /// If the property is set to <see langword="null" /> or
        /// <see cref="F:System.String.Empty" /> the default values
        /// are used:
        /// <list type="bullet">
        /// <item><i>tcp://localhost:9090/Aneka</i> for the <see cref="P:Aneka.Entity.Configuration.SchedulerUri" />  property.</item>
        /// <item><i>no authentication</i></item>
        /// </list>
        /// </remarks>
        public string ConfigPath 
        {
            get { return this.configPath; }
            set { this.configPath = value; }
        }
        /// <summary>
        /// Save path for the filtered image.
        /// </summary>
        protected string outputPath; 
        /// <summary>
        /// Gets or sets the name of the
        /// output file (inclusive of the
        /// path) where to save the filtered
        /// image.
        /// </summary>
        /// <remarks>
        /// If this value is <see langword="null" /> 
        /// or <see cref="T:Aneka.String.Empty" /> the
        /// file is saved into the same directory of
        /// <see cref="T:Aneka.Examples.ThreadDemo.WarholApplication.InputPath" />
        /// and the name is assigned by appending the ".warhol" suffix to the
        /// original name before the extension.
        /// </remarks>
        public string OutputPath 
        {
            get { return this.outputPath; }
            set { this.outputPath = value; }
        }
        #endregion
    
        #region Implementation Fields
        /// <summary>
        /// An instance of <see cref="T:Aneka.Entity.Configuration"/> containing the parameter settings
        /// for customizing the execution of the application.
        /// </summary>
        protected Configuration configuration;
        /// <summary>
        /// Reference to the <see cref="T:Aneka.Entity.AnekaApplication" /> instance
        /// that will be used to submit the execution of the <see cref="T:Aneka.Threading.AnekaThread" />
        /// instances used to execute the <see cref="T:Aneka.Examples.ThreadDemo.WahrolFilter" />.
        /// </summary>
        protected AnekaApplication<AnekaThread, ThreadManager> application;

        /// <summary>
        /// List of <see cref="T:Aneka.Threading.AnekaThread" /> instances
        /// that are currently running.
        /// </summary>
        protected IList<AnekaThread> running;

        /// <summary>
        /// List of the filters that have completed the
        /// execution.
        /// </summary>
        protected IList<WarholFilter> done;

        /// <summary>
        /// Number of copies of the image that compose
        /// one single row of the output image.
        /// </summary>
        protected int repeatX;
        /// <summary>
        /// Number of copies of the image that compose
        /// one single column of the output image.
        /// </summary>
        protected int repeatY;
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates an empty instance of <see cref="T:Aneka.Examples.ThreadDemo.WarholApplication" />.
        /// </summary>
        public WarholApplication() 
        { 
        
        }
        /// <summary>
        /// Performs the distributed filtering by creating
        /// four <see cref=-"T:Aneka.Threading.AnekaThread" />
        /// instances and waiting for their termination. Then
        /// it composes the images back and saves the output.
        /// </summary>
        /// <exception cref="T:System.IO.FileNotFoundException"><paramref name="T:Aneka.Samples.ThreadDemo.WarholApplication.InputPath"/> does not exist.</exception>
        public void Run() 
        {
            if (File.Exists(this.inputPath) == false) 
            {
                throw new FileNotFoundException("InputPath does not exist.", "InputPath");
            }
            try
            {
                // Initializes the AnekaApplication instance.
                this.Init();

                // read the bitmap 
                Bitmap source = new Bitmap(this.inputPath);

                // create one filter for each of the four slices that will
                // compose the final image and starts their execution on 
                // Aneka by wrapping them into AnekaThread instances...
                this.StartExecution(source);

                // wait for all threads to complete...
                this.WaitForCompletion();
                  
                // collect the processed images and compose them
                // into one single image.
                this.ComposeResult(source); 
                

            }
            finally
            {
                // we ensure that the application closes properly
                // before leaving the method...
                if (this.application != null) 
                {
                    if (this.application.Finished == false) 
                    {
                        this.application.StopExecution();

                    }
                    Console.WriteLine("Application execution finished");
                }
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Loads the <see cref="T:Aneka.Entity.Configuration" /> and 
        /// initializes the <see cref="T:Aneka.Entity.AnekaApplication{W,M}" /> 
        /// instance.
        /// </summary>
        protected void Init()
        {
            if (string.IsNullOrEmpty(this.configPath) == true)
            {
                // we get the default configuration...
                this.configuration = Configuration.GetConfiguration();
            }
            else
            {
                this.configuration = Configuration.GetConfiguration(this.configPath);
            }

            this.application = new AnekaApplication<AnekaThread, ThreadManager>(this.configuration);

        }
        /// <summary>
        /// <para>
        /// Starts the execution of the <see cref="T:Aneka.Threading.AnekaThread" /> 
        /// instances. 
        /// </para>
        /// <para>
        /// This method createas a set of <see cref="T:Aneka.Examples.ThreadDemo.WarholFilter" />
        /// instances and configure them with a <see cref="T:Aneka.Threading.AnekaThread" /> 
        /// instance. All the threads are added to a local running queue and then the <see cref="T:Aneka.Threading.AnekaThread.Start" />
        /// is invoked. The <see cref="T:Aneka.Examples.ThreadDemo.WarholFilter" /> are cnfigured with
        /// the <see cref="T:System.Drawing.Bitmap" /> <paramref name="source"/> as input image.
        /// </para>
        /// </summary>
        /// <param name="source"><see cref="T:System.Drawing.Bitmap" /> instance representing the input image of the filters.</param>
        protected void StartExecution(Bitmap source)
        {
            this.running = new List<AnekaThread>();
            WarholFilter[] filters = this.CreateFilters(source);

            // creates an AnekaThread for each filter
            foreach (WarholFilter filter in filters)
            {
                AnekaThread thread = new AnekaThread(new ThreadStart(filter.Apply), application);
                thread.Start();
                this.running.Add(thread);
            }
        }
        /// <summary>
        /// Collects the single images processed by the filters
        /// and compose them into a single image by juxtapposing
        /// the filters result.
        /// </summary>
        /// <param name="source">a <see cref="T:System.Drawing.Bitmap" /> representing the input image to the filter.</param>
        protected void ComposeResult(Bitmap source)
        {
            Bitmap output = new Bitmap(source.Width * this.repeatX, source.Height * this.repeatY, source.PixelFormat);
           
            Graphics graphics = Graphics.FromImage(output);
            int row = 0, col = 0;

            foreach (WarholFilter filter in this.done) 
            {
                // NOTE: uncomment the follwoing two lines if you want to have the single
                //       output of the files saved to disk.
                // string fileName = this.GetNewName(this.inputPath, String.Format("{0}.{1}",row,col));
                // filter.Image.Save(fileName);
                graphics.DrawImage(filter.Image, row * source.Width, col * source.Height);
                row++;
                if (row == this.repeatX) 
                {
                    row = 0;
                    col++;
                }
            }
            graphics.Dispose();
            if (string.IsNullOrEmpty(this.outputPath) == true) 
            {

                this.outputPath = this.GetNewName(this.inputPath, "warhol");
            }
            output.Save(this.outputPath);
        }


        /// <summary>
        /// Waits that all the threads submitted to Aneka
        /// successfully complete their execution. When this
        /// method returns the list of running threads is empty
        /// and the list of done threads is equal to the number
        /// of submitted threads.
        /// </summary>
        protected void WaitForCompletion()
        {
            this.done = new List<WarholFilter>();
            bool bSomeToGo = true;
            while (bSomeToGo == true)
            {
                foreach (AnekaThread thread in this.running)
                {
                    thread.Join();
                }

                for (int i = 0; i < this.running.Count; i++)
                {
                    AnekaThread thread = this.running[i];
                    if (thread.State == WorkUnitState.Completed)
                    {
                        this.running.RemoveAt(i);
                        i--;
                        WarholFilter filter = (WarholFilter) thread.Target;
                        this.done.Add(filter);
                        Console.WriteLine("-|Thread output: {0} |Application Name: {1} |Node Id: {2} |Submission Time: {3} | Completion Time:{4} | Maximum Exec. Time:{5} |", filter.Image.GetHashCode(), thread.ApplicationId, thread.NodeId, thread.SubmissionTime, thread.CompletionTime, thread.MaximumExecutionTime);

                    }
                    
                    else
                    {
                        // it must be failed...
                        thread.Start();
                    }

                }

                bSomeToGo = this.running.Count > 0;
            }
        }
        /// <summary>
        /// Creates the filters that will be used to 
        /// produce the images that will compose the
        /// output image. This method also sets the
        /// number of columns and rows that final
        /// output image will be composed of.
        /// </summary>
        /// <param name="source">a <see cref="T:System.Drawing.Bitmap" /> representing the input image to the filter.</param>
        /// <returns>A <see cref="T:System.Array" /> of <see cref="T:Aneka.Examples.ThreadDemo.WarholFilter" /> instances. </returns>
        protected virtual WarholFilter[] CreateFilters(Bitmap source)
        {
            WarholFilter[] filters = new WarholFilter[4];

            WarholFilter one = new WarholFilter();
            one.Image = source;
            one.Palette = WarholFilter.FuchsiaGreenWhite;
            filters[0] = one;
            

            WarholFilter two = new WarholFilter();
            two.Image = source;
            two.Palette = WarholFilter.YellowGreenNavy;
            filters[1] = two;

            WarholFilter three = new WarholFilter();
            three.Image = source;
            three.Palette = WarholFilter.FuchsiaOrangeBlue;
            filters[2] = three;

            WarholFilter four = new WarholFilter();
            four.Image = source;
            four.Palette = WarholFilter.GreenOrangeGainsboro;
            filters[3] = four;

            this.repeatX = 2;
            this.repeatY = 2;

            return filters;
        }
        /// <summary>
        /// Creates a new name from the given file <paramref name="name"/> and
        /// which includes the given <paramref name="suffix"/> before the file
        /// extension.
        /// </summary>
        /// <param name="name">A <see langword="string" /> containing the source file name.</param>
        /// <param name="suffix">A <see langword="string" /> containing the suffix to append to the file.</param>
        /// <returns>A <see langword="string" /> containing the new name.</returns>
        protected string GetNewName(string name, string suffix)
        {

            string pathTarget = Path.GetDirectoryName(name);
            string destName = String.Format("{0}_{1}.{2}", Path.GetFileNameWithoutExtension(name), suffix, Path.GetExtension(name));
            pathTarget = Path.Combine(pathTarget, destName);

            return pathTarget;
        }
        #endregion
    }
}
