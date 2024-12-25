using DGVPrinterHelper;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AttendanceMangementSystem
{
    public partial class AttenadanceList : Form
    {
        List<Student> students = new List<Student>();
        List<BluetoothDeviceInfo> _bluetoothDevices = new List<BluetoothDeviceInfo>();
        Student student = new Student();
        bool _isSearchingForDevices;
        int index;

        new Dictionary<string, string> StudentCollegiateIDs = new Dictionary<string, string>()
        {
            {"2021190000", "Mohamed Saad Ali" },
            {"2021190001", "Yasser Mohamed AbdelHamid" }
        };

        public bool isSearchingForDevices
        {
            get => _isSearchingForDevices;
            set
            {
                _isSearchingForDevices = value;
                btnSearch.Enabled = !_isSearchingForDevices;
            }
        }

        public AttenadanceList()
        {
            InitializeComponent();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            _bluetoothDevices.Clear();
            isSearchingForDevices = true;
            try
            {
                guna2ProgressIndicator1.BringToFront();
                guna2ProgressIndicator1.AutoStart = true;
                var bluetoothDevices = await SearchDevicesAsync();
                _bluetoothDevices.AddRange(bluetoothDevices);
                PopulateDataGridViewAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Couldn't search for bluetooth devices: { ex.Message}");
            }
            isSearchingForDevices = false;
            guna2ProgressIndicator1.AutoStart = false;
            btnSearch.BringToFront();
        }

        public async Task<BluetoothDeviceInfo[]> SearchDevicesAsync()
        {
            var bluetoothClient = new BluetoothClient();
            var bluetoothDevices = await Task.Run(() => bluetoothClient.DiscoverDevices());
            bluetoothClient.Close();
            return bluetoothDevices;
        }

        private bool isNotExist(BluetoothAddress deviceAddress)
        {
            foreach(var student in students)
            {
                if (student.BluetoothAddress == deviceAddress.ToString())
                    return false;
            }
            return true;
        }

        void PopulateDataGridViewAsync()
        {
            foreach(var device in _bluetoothDevices)
            {
                if (isNotExist(device.DeviceAddress))
                {
                    Student student = new Student
                    {
                        Name = StudentNameCollegiateIDMatching(device.DeviceName),
                        CollegiateID = device.DeviceName,
                        BluetoothAddress = device.DeviceAddress.ToString(),
                    };
                    students.Add(student);
                }
            }
            AttendanceList.DataSource = students.Select(s => new { s.CollegiateID, s.Name, s.ID, s.BluetoothAddress }).ToList();
        }

        string StudentNameCollegiateIDMatching(string CollegiateID)
        {
            foreach(var entry in StudentCollegiateIDs)
            {
                if(entry.Key == CollegiateID)
                {
                    return entry.Value;
                }
            }
            return CollegiateID;
        }

        [DllImport("BluetoothAPIs.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern UInt32 BluetoothRemoveDevice(
  [param: In, Out] ref BLUETOOTH_ADDRESS pAddress);
        UInt32 Unpair(Int64 Address)
        {
            BLUETOOTH_ADDRESS Addr = new BLUETOOTH_ADDRESS();
            Addr.ullLong = Address;
            return BluetoothRemoveDevice(ref Addr);
        }

        private void Print()
        {
            DGVPrinter printer = new DGVPrinter();
            printer.Title = "Attendance List"; //Header
            if (txtBoxInstructorName.Text.Trim() == String.Empty)
                printer.SubTitle = dtpCurrentDate.Text + '\n';
            else
                printer.SubTitle = txtBoxInstructorName.Text.Trim() + '\n' + dtpCurrentDate.Text + '\n';
            printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            printer.PageNumbers = true;
            printer.PageNumberInHeader = false;
            printer.PorportionalColumns = true;
            printer.HeaderCellAlignment = StringAlignment.Near;
            printer.Footer = "FCI SVU";
            printer.FooterSpacing = 15;
            printer.PrintDataGridView(AttendanceList);
        }

        private void AttenadanceList_Load(object sender, EventArgs e)
        {
            //_ = PopulateDataGridViewAsync();

            dtpCurrentDate.Text = DateTime.Now.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BluetoothClient client = new BluetoothClient();
            List<BluetoothDeviceInfo> devices = client.DiscoverDevices().ToList();
            foreach (var d in devices)
            {
                Unpair(d.DeviceAddress.ToInt64());
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Student student = new Student();
            Incrementer incrementer = new Incrementer();
            student.Name = StudentNameTextBox.Text;
            student.BluetoothAddress = Convert.ToString(incrementer.Address);   
            students.Add(student);
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            PopulateDataGridView();
            StudentNameTextBox.Clear();
        }

        private void AttendanceList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if(AttendanceList.CurrentRow.Index != -1)
                {
                    try
                    {
                        student.BluetoothAddress = AttendanceList.CurrentRow.Cells["BluetoothAddress"].Value.ToString();
                        index = students.FindIndex(s => s.BluetoothAddress == student.BluetoothAddress);
                        student = students.FirstOrDefault(s => s.BluetoothAddress == student.BluetoothAddress);
                        //student = students.Where(s => s.ID == student.ID).FirstOrDefault();
                        StudentNameTextBox.Text = student.Name;
                        AttendanceList.Enabled = false;
                    }
                    catch
                    {

                    }

                    btnDelete.Enabled = true;
                    btnUpdate.Enabled = true;
                    btnAdd.Enabled = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                students.RemoveAt(index);
                AttendanceList.Enabled = true;
                //students.RemoveAt(index);         
                //BluetoothDeviceInfo deviceInfo;
                //deviceInfo = _bluetoothDevices.Where(d => d.DeviceAddress.ToString() == student1.BluetoothAddress).FirstOrDefault();
                //_bluetoothDevices.Remove(deviceInfo);            
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
            PopulateDataGridView();
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            btnAdd.Enabled = true;
            StudentNameTextBox.Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            students[index].Name = StudentNameTextBox.Text;
            PopulateDataGridView();
            AttendanceList.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            btnAdd.Enabled = true;
            StudentNameTextBox.Clear();
        }

        void PopulateDataGridView()
        {
            AttendanceList.DataSource = students.Select(s => new { s.CollegiateID, s.Name, s.ID, s.BluetoothAddress }).ToList();
        }

    }
}
