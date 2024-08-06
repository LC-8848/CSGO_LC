using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// 命名空间
namespace CSGO_LC
{
    class parameter
    {
        /* 辅助参数设置 */
        public const bool isDraw = true; // 是否绘制
        public const bool isAim = true; // 是否自瞄
        public const bool isAimDraw = true; // 是否绘制自瞄范围
        public const int AimRange = 250; // 自瞄范围
        public const int Aimkey = 16; // 自瞄按键 1:左键 2:右键 4:中键 16:shift键 17:ctrl键 18:alt键 (更多按键请移步网上查询键代码)
        public const int AimType = 1; // 0:写入内存 1:模拟鼠标
        public const int Aimbody = 0; // 自瞄部位 0:头部 1:胸部
        public const int AimSpeedx = 52; // x轴自瞄速度，值越大越锁人（仅模拟鼠标方式有效）
        public const int AimSpeedy = 52; // y轴自瞄速度，值越大越锁人（仅模拟鼠标方式有效）
        /* 结束辅助参数设置 */
    }

    // 自己封装的工具类，包含窗口操作、内存操作及一些自写方法等
    class LC
    {
        const int PROCESS_VM_READ = 0x0010; // 读取内存
        const int PROCESS_VM_WRITE = 0x0020; // 写入内存
        const int PROCESS_VM_OPERATION = 0x0008; // 操作内存
        const int TH32CS_SNAPMODULE = 0x00000008; // 模块快照
        const int TH32CS_SNAPMODULE32 = 0x00000010; // 模块快照32位

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(int dwFlags, int th32ProcessID);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        //[DllImport("user32.dll", SetLastError = true)]
        //static extern bool GetWindowRect(IntPtr windowsHandle, out Rectangle windowsRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetClientRect(IntPtr windowsHandle, out Rectangle windowsRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ClientToScreen(IntPtr windowsHandle, out clientScreen windowsRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern short GetAsyncKeyState(int keyCode);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MODULEENTRY32
        {
            public uint dwSize;
            public uint th32ModuleID;
            public uint th32ProcessID;
            public uint GlblcntUsage;
            public uint ProccntUsage;
            public IntPtr modBaseAddr;
            public uint modBaseSize;
            public IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExePath;
        }

        public struct coordinate3D
        {
            public float x;
            public float y;
            public float z;
        }

        public struct coordinate2D
        {
            public float x;
            public float y;
        }

        public struct clientScreen
        {
            public int x;
            public int y;
        }

        // 取窗口左边
        public static int GetWindowsLeft(IntPtr windowsHandle)
        {
            clientScreen tmpRect = new clientScreen();
            if (ClientToScreen(windowsHandle, out tmpRect))
            {
                return tmpRect.x;
            }
            return 0;
        }

        // 取窗口顶边
        public static int GetWindowsTop(IntPtr windowsHandle)
        {
            clientScreen tmpRect = new clientScreen();
            if (ClientToScreen(windowsHandle, out tmpRect))
            {
                return tmpRect.y;
            }
            return 0;
        }

        // 取窗口宽度
        public static int GetWindowsWidth(IntPtr windowsHandle)
        {
            Rectangle tmpRect = new Rectangle();
            if (GetClientRect(windowsHandle, out tmpRect))
            {
                return tmpRect.Width;
            }
            return 0;
        }

        // 取窗口高度
        public static int GetWindowsHeight(IntPtr windowsHandle)
        {
            Rectangle tmpRect = new Rectangle();
            if (GetClientRect(windowsHandle, out tmpRect))
            {
                return tmpRect.Height;
            }
            return 0;
        }

        // 读64位整数
        public static long ReadInt64(IntPtr processHandle, IntPtr address)
        {
            byte[] buffer = new byte[sizeof(long)];
            if (!ReadProcessMemory(processHandle, address, buffer, buffer.Length, out int bytesRead))
            {
                return -1;
                //throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            return BitConverter.ToInt64(buffer, 0);
        }

        // 读32位整数
        public static int ReadInt32(IntPtr processHandle, IntPtr address)
        {
            byte[] buffer = new byte[sizeof(int)];
            if (!ReadProcessMemory(processHandle, address, buffer, buffer.Length, out int bytesRead))
            {
                return -1;
                //throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            return BitConverter.ToInt32(buffer, 0);
        }

        // 读16位整数
        public static short ReadInt16(IntPtr processHandle, IntPtr address)
        {
            byte[] buffer = new byte[sizeof(short)];
            if (!ReadProcessMemory(processHandle, address, buffer, buffer.Length, out int bytesRead))
            {
                return -1;
                //throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            return BitConverter.ToInt16(buffer, 0);
        }

        // 读32位浮点数
        public static float ReadFloat(IntPtr processHandle, IntPtr address)
        {
            byte[] buffer = new byte[sizeof(float)];
            if (!ReadProcessMemory(processHandle, address, buffer, buffer.Length, out int bytesRead))
            {
                return -1;
                //throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            return BitConverter.ToSingle(buffer, 0);
        }

        // 读64位浮点数
        public static double ReadDouble(IntPtr processHandle, IntPtr address)
        {
            byte[] buffer = new byte[sizeof(double)];
            if (!ReadProcessMemory(processHandle, address, buffer, buffer.Length, out int bytesRead))
            {
                return -1;
                //throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            return BitConverter.ToDouble(buffer, 0);
        }

        // 写64位整数
        public static bool WriteInt64(IntPtr processHandle, IntPtr address, long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteProcessMemory(processHandle, address, buffer, buffer.Length, out int bytesRead);
        }

        // 写32位整数
        public static bool WriteInt32(IntPtr processHandle, IntPtr address, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteProcessMemory(processHandle, address, buffer, buffer.Length, out int bytesRead);
        }

        // 写16位整数
        public static bool WriteInt16(IntPtr processHandle, IntPtr address, short value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteProcessMemory(processHandle, address, buffer, buffer.Length, out int bytesRead);
        }

        // 写32位浮点数
        public static bool WriteFloat(IntPtr processHandle, IntPtr address, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteProcessMemory(processHandle, address, buffer, buffer.Length, out int bytesRead);
        }

        // 写64位浮点数
        public static bool WriteDouble(IntPtr processHandle, IntPtr address, double value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteProcessMemory(processHandle, address, buffer, buffer.Length, out int bytesRead);
        }

        // 写字节集
        public static bool WriteBytes(IntPtr processHandle, IntPtr address, byte[] buffer)
        {
            return WriteProcessMemory(processHandle, address, buffer, buffer.Length, out int bytesRead);
        }

        // 读矩阵数组
        public static void ReadMatrixArray(IntPtr processHandle, IntPtr MatrixHead, out float[] returnArray)
        {
            float[] TmpMatrix = new float[16];
            for (int i = 0; i < 16; ++i)
            {
                TmpMatrix[i] = ReadFloat(processHandle, (IntPtr)(MatrixHead + i * 0X4));
            }
            returnArray = TmpMatrix;
        }

        // 获取模块句柄
        public static IntPtr GetModuleHandle(int processId, string moduleName)
        {
            IntPtr snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, processId);
            if (snapshot == IntPtr.Zero || (int)snapshot <= 0)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }

            MODULEENTRY32 moduleEntry = new MODULEENTRY32 { dwSize = (uint)Marshal.SizeOf(typeof(MODULEENTRY32)) };
            if (Module32First(snapshot, ref moduleEntry))
            {
                do
                {
                    if (moduleEntry.szModule.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                    {
                        CloseHandle(snapshot);
                        return moduleEntry.hModule;
                    }
                } while (Module32Next(snapshot, ref moduleEntry));
            }

            CloseHandle(snapshot);
            return IntPtr.Zero;
        }

        // 通过进程名取进程ID（注：进程名请勿加上.exe后缀）
        public static int GetProcessIdByName(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                throw new InvalidOperationException($"未找到名为 {processName} 的进程，请尝试打开进程或删除代码中进程名后的.exe后缀。");
            }
            return processes[0].Id;
        }

        // 通过进程ID取窗口句柄
        public static IntPtr GetWindowsHandleByProcessId(int processId)
        {
            Process process = Process.GetProcessById(processId);
            return process.MainWindowHandle;
        }

        // 通过进程ID取进程句柄
        public static IntPtr GetprocessHandleById(int processId)
        {
            return OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, false, processId);
        }

        // 世界坐标转屏幕坐标
        public static Rectangle WorldToScreen(coordinate3D peopleCoordinate, float[] MatrixArray, int GameWindowsW, int GameWindowsH, int type)
        {
            Rectangle tmp = new Rectangle(-1, -1, -1, -1);
            float cameraZ = MatrixArray[8] * peopleCoordinate.x + MatrixArray[9] * peopleCoordinate.y + MatrixArray[10] * peopleCoordinate.z + MatrixArray[11];
            float scaling = 1 / cameraZ;
            if (scaling >= 0)
            {
                float cameraX = GameWindowsW / 2 + (MatrixArray[0] * peopleCoordinate.x + MatrixArray[1] * peopleCoordinate.y + MatrixArray[2] * peopleCoordinate.z + MatrixArray[3]) * scaling * GameWindowsW / 2;
                float cameraY1 = GameWindowsH / 2 - (MatrixArray[4] * peopleCoordinate.x + MatrixArray[5] * peopleCoordinate.y + MatrixArray[6] * (peopleCoordinate.z - 8) + MatrixArray[7]) * scaling * GameWindowsH / 2;
                float cameraY2 = GameWindowsH / 2 - (MatrixArray[4] * peopleCoordinate.x + MatrixArray[5] * peopleCoordinate.y + MatrixArray[6] * (peopleCoordinate.z + 78) + MatrixArray[7]) * scaling * GameWindowsH / 2;
                tmp.Height = (int)(cameraY1 - cameraY2);
                tmp.Width = (int)(tmp.Height * 0.526515151552f);
                tmp.X = (int)(cameraX - tmp.Width / 2);
                tmp.Y = (int)(cameraY1 - tmp.Height);
                if (type == 773 || type == 774 || type == 775)
                {
                    tmp.Height /= 6;
                    tmp.Height *= 4;
                    tmp.Y += tmp.Height * 1 / 3;
                }
            }
            return tmp;
        }

        // 取二维向量长
        public static double ComputeCoordinate2D(coordinate2D coor)
        {
            return Math.Sqrt(coor.x * coor.x + coor.y * coor.y);
        }

        // 取三维向量长
        public static double ComputeCoordinate3D(coordinate3D coor)
        {
            return Math.Sqrt(coor.x * coor.x + coor.y * coor.y + coor.z * coor.z);
        }

        // 一键计算屏幕自瞄坐标
        public static coordinate2D AutoAiming(coordinate3D myself, coordinate3D enemy, int type, int Aimbody)
        {
            coordinate3D difference;
            difference.x = myself.x - enemy.x;
            difference.y = myself.y - enemy.y;
            difference.z = myself.z - enemy.z + Aimbody * 10;
            if (type == 773 || type == 774 || type == 775)
            {
                difference.z += 12;
            }
            coordinate2D a = new coordinate2D();
            if (difference.x > 0 && difference.y > 0)
            {
                a.x = (float)(Math.Atan(difference.y / difference.x) / Math.PI * 180 - 180);
            }
            if (difference.x <= 0 && difference.y > 0)
            {
                a.x = (float)(Math.Atan(difference.y / difference.x) / Math.PI * 180);
            }
            if (difference.x <= 0 && difference.y < 0)
            {
                a.x = (float)(Math.Atan(difference.y / difference.x) / Math.PI * 180);
            }
            if (difference.x >= 0 && difference.y < 0)
            {
                a.x = (float)(Math.Atan(difference.y / difference.x) / Math.PI * 180 + 180);
            }
            coordinate2D difference2;
            difference2.x = difference.x;
            difference2.y = difference.y;
            double vectorLength = ComputeCoordinate2D(difference2);
            a.y = (float)((Math.Atan(difference.z / vectorLength)) / Math.PI * 180);
            return a;
        }

        [DllImport("user32")]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        //移动鼠标 
        public const int MOUSEEVENTF_MOVE = 0x0001;
        //模拟鼠标左键按下 
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //模拟鼠标左键抬起 
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        //模拟鼠标右键按下 
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起 
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;
        //模拟鼠标中键按下 
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //模拟鼠标中键抬起 
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        //标示是否采用绝对坐标 
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        //模拟鼠标滚轮滚动操作，必须配合dwData参数
        public const int MOUSEEVENTF_WHEEL = 0x0800;
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const uint WS_EX_LAYERED = 0x00080000;
        private const uint WS_EX_TRANSPARENT = 0x00000020;
        private const uint LWA_COLORKEY = 0x00000001;

        private int Game_Pid; // 游戏进程ID
        private IntPtr Windows_Handle, Game_Handle; // 游戏窗口句柄和进程句柄
        private Timer _timer; // 刷新定时器
        private Timer _timer_close; // 判断定时器
        private void Form1_Load(object sender, EventArgs e)
        {
            // 获取进程ID
            Game_Pid = LC.GetProcessIdByName("csgo");
            // 获取进程句柄
            Game_Handle = LC.GetprocessHandleById(Game_Pid);
            // 获取窗口句柄
            Windows_Handle = LC.GetWindowsHandleByProcessId(Game_Pid);
            // 获取窗口位置和大小
            Left = LC.GetWindowsLeft(Windows_Handle);
            Top = LC.GetWindowsTop(Windows_Handle);
            Width = LC.GetWindowsWidth(Windows_Handle);
            Height = LC.GetWindowsHeight(Windows_Handle);

            BackColor = Color.Gray; // 设置窗口背景色为灰色
            TransparencyKey = Color.Gray; // 设置透明色为灰色
            FormBorderStyle = FormBorderStyle.None; // 设置窗口无边框
            TopMost = true; // 置顶窗口
            Paint += paint; // 添加绘制事件处理程序
            DoubleBuffered = true; // 开启双缓冲

            // 设置窗口为透明且点击穿透
            uint initialStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, initialStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);

            // 基址初始化
            Initialize_Base_Address();

            // 初始化刷新定时器
            _timer = new Timer();
            // 设置定时器间隔为1/144秒，即每秒144帧
            _timer.Interval = 1000 / 144;
            // 添加定时器事件处理程序
            _timer.Tick += new EventHandler(OnTimerTick);
            // 启动定时器
            _timer.Start();

            // 初始化判断定时器
            _timer_close = new Timer();
            // 设置定时器间隔为520毫秒
            _timer_close.Interval = 520;
            // 添加定时器事件处理程序
            _timer_close.Tick += new EventHandler(OnTimerCloseTick);
            // 启动定时器
            _timer_close.Start();
        }

        private IntPtr client, engine, server, matrix_header, people_array_header, mouse_base; // 模块基地址、矩阵头、玩家数组头、鼠标基地址
        private void Initialize_Base_Address()
        {
            // 获取模块基址
            client = LC.GetModuleHandle(Game_Pid, "client.dll");
            engine = LC.GetModuleHandle(Game_Pid, "engine.dll");
            server = LC.GetModuleHandle(Game_Pid, "server.dll");
            // 定义矩阵头地址
            matrix_header = client + 0x4D94834;
            // 定义玩家数组头地址
            people_array_header = server + 0xA7B5D4;
            // 定义鼠标基址
            mouse_base = engine + 0x58EFE4;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            // 调用 Invalidate 方法来触发重绘
            Invalidate();
        }

        private void OnTimerCloseTick(object sender, EventArgs e)
        {
            // 判断游戏进程是否存在
            if (Process.GetProcessesByName("csgo").Length <= 0)
            {
                // 关闭辅助
                Process.GetCurrentProcess().Kill();
            }
            // 判断End按键是否按下
            if (LC.GetAsyncKeyState(35) != 0)
            {
                // 汇编还原鼠标输入
                LC.WriteBytes(Game_Handle, engine + 0xB1A90, new byte[] { 0xF3, 0x0F, 0x11, 0x80, 0x94, 0x4D, 0x00, 0x00 });
                LC.WriteBytes(Game_Handle, engine + 0xB1A1E, new byte[] { 0xF3, 0x0F, 0x11, 0x80, 0x90, 0x4D, 0x00, 0x00 });
                // 关闭辅助
                Process.GetCurrentProcess().Kill();
            }
        }

        private Pen Pen_White = new Pen(Color.FromArgb(210, 255, 255, 255), 1); // 白色画笔
        private Pen Pen_Black = new Pen(Color.FromArgb(255, 0, 0, 0), 1); // 黑色画笔
        private Pen Pen_Green = new Pen(Color.FromArgb(255, 0, 255, 0), 1); // 绿色画笔
        private Pen Pen_Red = new Pen(Color.FromArgb(255, 255, 0, 0), 1); // 红色画笔
        private Pen Pen_Blue = new Pen(Color.FromArgb(255, 0, 0, 255), 1); // 蓝色画笔
        private long people_pointer = 0; // 玩家指针
        private void paint(object sender, PaintEventArgs e)
        {
            // 刷新窗口位置和大小
            Left = LC.GetWindowsLeft(Windows_Handle);
            Top = LC.GetWindowsTop(Windows_Handle);
            Width = LC.GetWindowsWidth(Windows_Handle);
            Height = LC.GetWindowsHeight(Windows_Handle);
            // 设置文本抗锯齿方式
            e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            // 绘制文本
            e.Graphics.DrawString("CSGO_冷辰", new Font("Arial", 8), new SolidBrush(Color.FromArgb(255, 0, 255, 0)), 0, 0);
            // 设置图形抗锯齿方式
            /*e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;*/
            // 初始化游戏数据
            Invalidate_Data(e);
        }

        private float[] matrix = new float[16]; // 矩阵数组
        private int people_count, hp, hp_max, my_camp, camp, type, text_size, aim_speed_x, aim_speed_y, aim_type; // 玩家数量、血量、最大血量、敌我阵营、状态、文本大小、自瞄速度、自瞄敌人状态
        private LC.coordinate3D my_coordinate, people_coordinate, aim_coordinate, aim_difference_coordinate; // 自身坐标、敌人坐标、自瞄坐标、自瞄坐标差
        private Rectangle rect, aim_rect; // 矩形、自瞄矩形
        private double aim_min, aim_tmp; // 自瞄最小值、自瞄临时值
        private LC.coordinate2D difference_coordinate, mouse_coordinate; // 坐标差、鼠标坐标
        private long mouse_address; // 鼠标地址
        private void Invalidate_Data(PaintEventArgs e)
        {
            // 初始化自瞄变量
            aim_min = parameter.AimRange / 2;
            aim_coordinate = new LC.coordinate3D();
            // 获取矩阵数组
            LC.ReadMatrixArray(Game_Handle, matrix_header, out matrix);
            // 获取玩家数量
            people_count = LC.ReadInt32(Game_Handle, server + 0xB24740);
            // 以玩家数量为循环次数
            for (int i = 0; i < people_count; i++)
            {
                // 获取玩家数组指针
                people_pointer = LC.ReadInt32(Game_Handle, people_array_header + i * 0x18);
                people_pointer = LC.ReadInt32(Game_Handle, (IntPtr)people_pointer + 0x1C);
                people_pointer = LC.ReadInt32(Game_Handle, (IntPtr)people_pointer + 0xC);
                // 获取玩家坐标
                people_coordinate.x = LC.ReadFloat(Game_Handle, (IntPtr)people_pointer + 0x1DC);
                people_coordinate.y = LC.ReadFloat(Game_Handle, (IntPtr)people_pointer + 0x1E0);
                people_coordinate.z = LC.ReadFloat(Game_Handle, (IntPtr)people_pointer + 0x1E4);
                // 获取玩家血量、最大血量、阵营、类型
                hp = LC.ReadInt32(Game_Handle, (IntPtr)people_pointer + 0x230);
                hp_max = LC.ReadInt32(Game_Handle, (IntPtr)people_pointer + 0x22C);
                camp = LC.ReadInt32(Game_Handle, (IntPtr)people_pointer + 0x314);
                type = LC.ReadInt16(Game_Handle, (IntPtr)people_pointer + 0xD8);
                // 判断是否为自身
                if (i == 0)
                {
                    // 获取自身坐标和阵营
                    my_coordinate = people_coordinate;
                    my_camp = camp;
                }
                else
                {
                    // 判断是否为敌人及是否存活
                    if (hp > 0 && camp != my_camp)
                    {
                        // 判断是否开启绘制
                        if (parameter.isDraw)
                        {
                            // 获取屏幕绘制坐标
                            rect = LC.WorldToScreen(people_coordinate, matrix, Width, Height, type);
                            // 绘制矩形
                            e.Graphics.DrawRectangle(Pen_Green, rect.X, rect.Y, rect.Width, rect.Height);
                            // 绘制线条
                            e.Graphics.DrawLine(Pen_White, Width / 2, 0, rect.X + rect.Width / 2, rect.Y);
                            // 计算字体大小
                            text_size = (int)Math.Round((double)(8 * rect.Width / 20));
                            // 字体大小限制
                            text_size = text_size > 30 ? 30 : text_size;
                            // 绘制血量
                            if (text_size >= 8)
                            {
                                e.Graphics.DrawString("血量: " + hp + "/" + hp_max, new Font("Arial", text_size), new SolidBrush(Color.FromArgb(255, 255, 255, 255)), rect.X, rect.Y - text_size * 1.5f);
                            }
                        }
                        // 判断是否开启自瞄
                        if (parameter.isAim)
                        {
                            // 计算距离
                            difference_coordinate.x = rect.X + rect.Width / 2 - Width / 2;
                            difference_coordinate.y = rect.Y + rect.Height * 1 / 5 - Height / 2;
                            // 筛选自瞄对象
                            aim_tmp = LC.ComputeCoordinate2D(difference_coordinate);
                            // 刷新最小距离
                            if (aim_tmp < aim_min)
                            {
                                // 记录最小距离
                                aim_min = aim_tmp;
                                // 记录自瞄坐标
                                aim_coordinate = people_coordinate;
                                // 记录自瞄矩形
                                aim_rect = rect;
                                // 记录坐标差值
                                aim_difference_coordinate.x = my_coordinate.x - people_coordinate.x;
                                aim_difference_coordinate.y = my_coordinate.y - people_coordinate.y;
                                aim_difference_coordinate.z = my_coordinate.z - people_coordinate.z;
                                // 记录自瞄敌人状态
                                aim_type = type;
                            }
                        }
                    }
                }
            }
            // 判断是否开启自瞄
            if (parameter.isAim)
            {
                // 绘制自瞄范围
                if (parameter.isAimDraw)
                {
                    // 绘制圆形
                    e.Graphics.DrawEllipse(Pen_White, Width / 2 - parameter.AimRange / 2, Height / 2 - parameter.AimRange / 2, parameter.AimRange, parameter.AimRange);
                }
                // 判断自瞄按键
                if (LC.GetAsyncKeyState(parameter.Aimkey) != 0)
                {
                    // 检查自瞄坐标
                    if (aim_coordinate.x != 0 && aim_coordinate.y != 0 && aim_coordinate.z != 0)
                    {
                        // 判断自瞄类型
                        if (parameter.AimType == 0)
                        {
                            // 汇编noop鼠标输入
                            LC.WriteBytes(Game_Handle, engine + 0xB1A90, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
                            LC.WriteBytes(Game_Handle, engine + 0xB1A1E, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
                            // 计算鼠标坐标
                            mouse_coordinate = LC.AutoAiming(my_coordinate, aim_coordinate, aim_type, parameter.Aimbody);
                            // 写入自瞄数据
                            mouse_address = LC.ReadInt32(Game_Handle, mouse_base);
                            LC.WriteFloat(Game_Handle, (IntPtr)mouse_address + 0x4D94, mouse_coordinate.x);
                            LC.WriteFloat(Game_Handle, (IntPtr)mouse_address + 0x4D90, mouse_coordinate.y);
                        }
                        // 判断自瞄类型
                        if (parameter.AimType == 1)
                        {
                            // 设置初始鼠标移动速度
                            aim_speed_x = parameter.AimSpeedx;
                            aim_speed_y = parameter.AimSpeedy;
                            // x轴速度限制
                            while ((int)Math.Round((double)Math.Abs(aim_rect.X + aim_rect.Width / 2 - Width / 2)) < aim_speed_x)
                            {
                                aim_speed_x = (int)Math.Round((double)(aim_speed_x / 2));
                            }
                            // y轴速度限制
                            while ((int)Math.Round((double)Math.Abs(aim_rect.Y + aim_rect.Height * (2 + parameter.Aimbody) / 10 - Height / 2)) < aim_speed_y)
                            {
                                aim_speed_y = (int)Math.Round((double)(aim_speed_y / 2));
                            }
                            // 移动鼠标
                            LC.mouse_event(LC.MOUSEEVENTF_MOVE, (int)Math.Round((double)(aim_rect.X + aim_rect.Width / 2 - Width / 2)) > 0 ? aim_speed_x : -aim_speed_x, (int)Math.Round((double)(aim_rect.Y + aim_rect.Height * (2 + parameter.Aimbody) / 10 - Height / 2)) > 0 ? aim_speed_y : -aim_speed_y, 0, 0);
                        }
                    }
                    else
                    {
                        // 判断自瞄类型
                        if (parameter.AimType == 0)
                        {
                            // 汇编还原鼠标输入
                            LC.WriteBytes(Game_Handle, engine + 0xB1A90, new byte[] { 0xF3, 0x0F, 0x11, 0x80, 0x94, 0x4D, 0x00, 0x00 });
                            LC.WriteBytes(Game_Handle, engine + 0xB1A1E, new byte[] { 0xF3, 0x0F, 0x11, 0x80, 0x90, 0x4D, 0x00, 0x00 });
                        }
                    }
                }
                else
                {
                    // 判断自瞄类型
                    if (parameter.AimType == 0)
                    {
                        // 汇编还原鼠标输入
                        LC.WriteBytes(Game_Handle, engine + 0xB1A90, new byte[] { 0xF3, 0x0F, 0x11, 0x80, 0x94, 0x4D, 0x00, 0x00 });
                        LC.WriteBytes(Game_Handle, engine + 0xB1A1E, new byte[] { 0xF3, 0x0F, 0x11, 0x80, 0x90, 0x4D, 0x00, 0x00 });
                    }
                }
            }
        }
    }
}
