using ImageMagick;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace PicResizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        string[] files;
        private List<string> list =new List<string>();

        public List<string> List
        {
            get { return list; }
            set
            {
                list = value;
                this.RaisePropertyChanged("List");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(Dowork);
            t.Start();
        }

        private void TextBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {         
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))            
            {                
                return;            
            }
            files = (string[])e.Data.GetData(DataFormats.FileDrop);
            List.Clear();
            foreach (var file in files)
            {
                List.Add(file);
            }
            listbox1.Items.Refresh();
        }

        private void Dowork()
        {
            using (MagickImage watermark = new MagickImage(@"C:\Users\111\Downloads\watermark.png"))
            {
                for (; List.Count != 0;)
                {
                    var file = List[0];
                    try
                    {
                        

                        using (MagickImage bmi = new MagickImage(file))
                        {
                            var settings = new MagickReadSettings
                            {
                                Font = "方正行黑简体",
                                FontPointsize = 90,
                                FillColor = new MagickColor(0, 0, 0, 80),
                                TextGravity = Gravity.Center,
                                TextDirection = TextDirection.LeftToRight,
                                Width = 2000,
                                BackgroundColor = MagickColors.Transparent,
                                Height = 400, // height of text box
                            };
                            using (var caption = new MagickImage("caption:SJTU大学物理学科营辅导员 专用\n不得转载、二次分发", settings))
                            {
                                // Add the caption layer on top of the background image
                                // at position 590,450
                                bmi.Composite(caption, Gravity.Southwest, CompositeOperator.Over);

                            }

                            bmi.Composite(watermark, Gravity.Southeast, CompositeOperator.Over);

                            //bmi.Resize(1500, 1500 / bmi.Width * bmi.Height);

                            bmi.Write(@"D:\BaiduNetdiskDownload\处理后\" + new System.IO.FileInfo(file).Name, MagickFormat.Jpg);
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show($"{ex.ToString()}");
                    }
                    this.Dispatcher.Invoke(() => {
                        List.RemoveAt(0);
                        listbox1.Items.Refresh(); 
                    });
                }
            }
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

    }
}
