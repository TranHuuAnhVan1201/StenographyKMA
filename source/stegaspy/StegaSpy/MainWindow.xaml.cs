using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Threading;

//
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using AForge.Video;
using AForge;
using AForge.Video.FFMPEG;
// end.

namespace StegaSpy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private OpenFileDialog txtDlg;
        private OpenFileDialog aviDlg;
        private string txtLocation = "";
        private string aviLocation = "";
        private Thread encodeThread;
        private Thread decodeThread;
        private string me_decode = "";

        public System.Drawing.Size ClientSize { get; private set; }
        public Bitmap BackgroundImage { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Videoinfo_Click(object sender, RoutedEventArgs e)
        {
            VideoInfo v = new VideoInfo();
            v.Show();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help h = new Help();
            h.Show();
        }

        private void BrowseTxt_click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog initialization
            txtDlg = new OpenFileDialog
            {
                Filter = "Text Files|*.txt",
                Title = "Text files containing message"
            };
            _ = txtDlg.ShowDialog();
            //Start the appending to file chosen from the file dialog
            txtLocation = txtDlg.FileName;
            txtLocation_tb.Text = txtLocation;
        }

        private void BrowseAvi_click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog initialization
            aviDlg = new OpenFileDialog
            {
                Filter = "Text Files|*.*",
                Title = "Avi Video to use in steganography"
            };
            _ = aviDlg.ShowDialog();
            //Start the appending to file chosen from the file dialog
            aviLocation = aviDlg.FileName;
            aviLocation_tb.Text = aviLocation;
        }

        private void EncodeBtn_click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(aviLocation) && !string.IsNullOrEmpty(txtLocation))
            {
                encodeThread = new Thread(EncodeEngine);
                encodeThread.Start();
                encode_btn.IsEnabled = false;
            }
            else
            {
                _ = MessageBox.Show("Check the avi and message textfile Location");
            }

        }

        private void EncodeEngine(object obj)
        {
            // create instance of video reader
            try
            {
                string covert_message = "";
                // create instance of video reader
                //VideoFileReader: Lớp để đọc các tệp video sử dụng thư viện FFmpeg.
                VideoFileReader reader = new VideoFileReader();
                // open video file
                reader.Open(aviLocation);

                //read the covert message from the txt file 
                FileStream covertMessageStream = new FileStream(txtLocation, FileMode.Open);
                StreamReader sR = new StreamReader(covertMessageStream);

                string line = sR.ReadLine();
                while (line != null)
                {
                    covert_message += line;
                    line = sR.ReadLine();
                }
                covertMessageStream.Close();
                sR.Close();

                //get the total no of frames
                //long total_frames = reader.FrameCount;
                long total_frames = reader.FrameCount;
                int width = reader.Width;
                int height = reader.Height;
                int framerate = reader.FrameRate;
                // create instance of video writer
                VideoFileWriter writer = new VideoFileWriter();
                // create new video file todotdo codec video
                //writer.Open("../test.avi", width, height, framerate, VideoCodec.Raw);
                writer.Open("../test04.avi", width, height, framerate, VideoCodec.Raw);
                // create a bitmap to save into the video file
                Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                //a function that takes in video, selects 10 random frames to hide the covert message, then generates a password, hides the covert message
                Random HC = new Random();
                int[] frames = {
                    HC.Next((int)total_frames),
                    HC.Next((int)total_frames),
                    HC.Next((int)total_frames),
                    HC.Next((int)total_frames),
                    HC.Next((int)total_frames),
                    HC.Next((int)total_frames),
                    HC.Next((int)total_frames),
                    HC.Next((int)total_frames),
                    HC.Next((int)total_frames),
                    HC.Next((int)total_frames)
                };

                //split the message into 10
                int message_length = covert_message.Length;
                int f1 = 0;
                int f2 = (int)(f1 + (0.1 * message_length));
                int f3 = (int)(f2 + (0.1 * message_length));
                int f4 = (int)(f3 + (0.1 * message_length));
                int f5 = (int)(f4 + (0.1 * message_length));
                int f6 = (int)(f5 + (0.1 * message_length));
                int f7 = (int)(f6 + (0.1 * message_length));
                int f8 = (int)(f7 + (0.1 * message_length));
                int f9 = (int)(f8 + (0.1 * message_length));
                int f10 = (int)(f9 + (0.1 * message_length));
                string[] message_split = {
                                             covert_message.Substring(f1,(int)(0.1*message_length)),
                                             covert_message.Substring(f2,(int)(0.1*message_length)),
                                             covert_message.Substring(f3,(int)(0.1*message_length)),
                                             covert_message.Substring(f4,(int)(0.1*message_length)),
                                             covert_message.Substring(f5,(int)(0.1*message_length)),
                                             covert_message.Substring(f6,(int)(0.1*message_length)),
                                             covert_message.Substring(f7,(int)(0.1*message_length)),
                                             covert_message.Substring(f8,(int)(0.1*message_length)),
                                             covert_message.Substring(f9,(int)(0.1*message_length)),
                                             covert_message.Substring(f10),
                                         };
                int cm_temp = 0;
                for (int i = 0; i < total_frames; i++)
                {
                    if (frames.Contains(i))
                    {
                        Console.WriteLine(message_split[cm_temp]);
                        image = SteganographyHelper.EmbedText(message_split[cm_temp], reader.ReadVideoFrame());
                        cm_temp += 1;
                    }
                    else
                    {
                        image = reader.ReadVideoFrame();
                    }
                    // màu đỏ. xóa được.
                    double progress = i / (double)total_frames * 100;
                    _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate () { encode_progressBar.Value = progress; });
                    // process the frame somehow
                    writer.WriteVideoFrame(image);
                    // dispose the frame when it is no longer required
                    //todo todo
                    //videoFrame.Dispose();
                    //VideoFileReader.Dispose();
                }
                reader.Close();
                writer.Close();
                _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                  {
                      //442S833S222S444S699S118S285S352S595S201
                      frames_password_tb.Text = frames[0].ToString() + "S" + frames[1].ToString() + "S" + frames[2].ToString() + "S" + frames[3].ToString() + "S" + frames[4].ToString() + "S" + frames[5].ToString() + "S" + frames[6].ToString() + "S" + frames[7].ToString() + "S" + frames[8].ToString() + "S" + frames[9].ToString();
                  });
                _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                  {
                      encode_btn.IsEnabled = true;
                      encode_progressBar.Value = 100;
                      me_decode = message_split[0] + message_split[1] + message_split[2] + message_split[3] + message_split[4] + message_split[5] + message_split[6] + message_split[7] + message_split[8] + message_split[9];
                      MessageBoxResult messageBoxResult = MessageBox.Show("\nNhúng thông điệp thành công.\n" + "\n" + me_decode);                    
                      encode_progressBar.Value = 0;
                  });

            }
            catch (Exception ex)
            {
                _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                  {
                      encode_btn.IsEnabled = true;
                      MessageBoxResult messageBoxResult = MessageBox.Show(ex.ToString());
                  });
            }
        }

        private void DecodeBtn_click(object sender, RoutedEventArgs e)
        {

            //check the password first
            if (!string.IsNullOrEmpty(password_text.Text) && !string.IsNullOrEmpty(aviExtractBrowse_tb.Text))
            {
                decodeThread = new Thread(DecodeEngine);
                decodeThread.Start();
            }
            else
            {
                _ = MessageBox.Show("Check password and avi location");
            }

        }
        private void DecodeEngine(object obj)
        {
            //46S67S1S2S69S47S94S87S33S62
            //442S833S222S444S699S118S285S352S595S201
            string[] fStrings;
            int[] frames = new int[10];
            try
            {
                _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    fStrings = password_text.Text.Split('S');
                    frames[0] = int.Parse(fStrings[0]);
                    frames[1] = int.Parse(fStrings[1]);
                    frames[2] = int.Parse(fStrings[2]);
                    frames[3] = int.Parse(fStrings[3]);
                    frames[4] = int.Parse(fStrings[4]);
                    frames[5] = int.Parse(fStrings[5]);
                    frames[6] = int.Parse(fStrings[6]);
                    frames[7] = int.Parse(fStrings[7]);
                    frames[8] = int.Parse(fStrings[8]);
                    frames[9] = int.Parse(fStrings[9]);
                });
                // create instance of video reader
                try
                {
                    // create instance of video reader
                    VideoFileReader reader = new VideoFileReader();
                    // open video file
                    reader.Open(aviLocation);
                    //get the total no of frames
                    long total_frames = reader.FrameCount;
                    int width = reader.Width;
                    int height = reader.Height;
                    int framerate = reader.FrameRate;
                    string decode_covert_message = "";
                    // read 100 video frames out of it
                    for (int i = 0; i < total_frames; i++)
                    {
                        if (frames.Contains(i))
                        {
                            decode_covert_message += SteganographyHelper.ExtractText(reader.ReadVideoFrame());
                        }
                        else
                        {
                            _ = reader.ReadVideoFrame();
                        }
                        double progress = i / (double)total_frames * 100;
                        _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate () { decode_progressBar.Value = progress; });
                    }
                    reader.Close();
                    _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate () { _ = MessageBox.Show(me_decode + "\n\nTrích xuất thông điệp thành công.\n" ); });

                }
                catch (Exception ex)
                {
                    _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate () { _ = MessageBox.Show(ex.ToString()); });

                }

            }
            catch (Exception myex)
            {
                _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate () { _ = MessageBox.Show("Wrong password " + myex.Message); });

            }
        }

        private void BrowseAviE_click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog initialization
            aviDlg = new OpenFileDialog
            {
                Filter = "Text Files|*.*",
                Title = "Avi Video to use in steganography"
            };
            _ = aviDlg.ShowDialog();
            //Start the appending to file chosen from the file dialog
            aviLocation = aviDlg.FileName;
            aviExtractBrowse_tb.Text = aviLocation;
        }

    }
}
