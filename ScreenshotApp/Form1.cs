using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ScreenshotApp
{
    public partial class MainForm : Form
    {
        private VideoCaptureDevice videoSource;
        private FilterInfoCollection cameras;
        public MainForm()
        {
            InitializeComponent();
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
        }
        //start camera
        private void button1_Click(object sender, EventArgs e)
        {
            // Bắt đầu stream video khi bấm nút "Start."
            if (videoSource != null && !videoSource.IsRunning)
            {
                videoSource.Start();
            }
            else
            {
                MessageBox.Show("Không tìm thấy thiết bị video.");
            }
        }
        // Hiển thị frame mới trên PictureBox.
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = bitmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem pictureBox1 có ảnh không
            if (pictureBox1.Image != null)
            {
                // Tạo thư mục nếu nó chưa tồn tại
                string folderPath = "D:\\CapturedImages";
                string folderPathImage
                    = $"D:\\CapturedImages\\Capture_{DateTime.Now:yyyyMMddHHmmssfff}";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                
                Directory.CreateDirectory(folderPathImage);
                // Lưu ảnh với tên độc đáo (ví dụ: timestamp)
                string fileName = $"Capture_{DateTime.Now:yyyyMMddHHmmssfff}.png";
                string filePath = Path.Combine(folderPath,folderPathImage, fileName);
                // Lưu ảnh vào thư mục
                pictureBox1.Image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                MessageBox.Show($"Image captured and saved to {filePath}");
            }
            else
            {
                MessageBox.Show("No image to capture.");
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Lấy danh sách thiết bị video.
            cameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            // Kiểm tra nếu có ít nhất một thiết bị video.
            if (cameras.Count > 0)
            {
                // Chọn thiết bị đầu tiên trong danh sách.
                videoSource = new VideoCaptureDevice(cameras[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
            }
            else
            {
                MessageBox.Show("Không tìm thấy thiết bị video.");
            }
        }
        //video dừng stream khi form đóng
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dừng stream video khi form đóng.
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
            Application.Exit();
        }

        //stop camera
        private void button3_Click(object sender, EventArgs e)
        {
            // Dừng stream video khi bấm nút "Stop."
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }
    }
}
