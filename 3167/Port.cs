using System;
using System.IO.Ports;
using System.Threading;

public class Port
{
    static bool working;//传输是否结束
    static SerialPort port = new SerialPort();//串口
    static bool InitFin = true;//串口初始化是否正常进行
    static void SelectPort()
    {
        Console.Clear();
        string temp;
        Console.WriteLine("选择端口");
        Console.WriteLine("\tCOM1");
        Console.WriteLine("\tCOM2");
        while (true)
        {
            Console.WriteLine("二选一（输入#退出）：");
            temp = Console.ReadLine();
            if (temp == "#") { InitFin = false; return; }
            if (temp != "COM1" && temp != "COM2")
            {
                Console.WriteLine("输入错误，请重新输入！");
                continue;
            }
            else break;
        }
        port.PortName = temp;
    }
    static void SelectBaundRate()
    {
        int[] baudrate = { 300, 600, 1200, 2400, 4800, 9600, 19200 };
        int i;
        Console.Clear();
        if (InitFin)
        {
            Console.WriteLine("选择波特率：");
            for (i = 0; i < 7; i++) Console.WriteLine("{0}", baudrate[i]);
            Console.WriteLine("选择任一波特率（推荐选择9600）（输入0退出）：");
            while (true)
            {
                i = int.Parse(Console.ReadLine());
                if (i == 0) { InitFin = false; return; }
                else if (i != 300 && i != 600 && i != 1200 && i != 2100 && i != 4800 && i != 9600 && i != 19200)
                {
                    Console.WriteLine("输入错误，请重新输入！");
                    Console.WriteLine("重新输入：");
                    continue;
                }
                else break;
            }
            port.BaudRate = i;
        }
        else return;
    }
    static void SelectPariety()
    {
        Console.Clear();
        string parity;
        string[] options = { "None", "Odd", "Even" };
        Console.WriteLine("选择奇偶校验模式：(选择模式前序号，输入0退出)");
        Console.WriteLine("\t1.无");
        Console.WriteLine("\t2.奇");
        Console.WriteLine("\t3.偶");
        while (true)
        {
            Console.WriteLine("请输入：");
            parity = Console.ReadLine();
            if (parity == "0")
            {
                InitFin = false;
                return;
            }
            else if (parity != "1" && parity != "2" && parity != "3")
            {
                Console.WriteLine("输入错误，请重新输入！");
                continue;
            }
            else break;
        }
        parity = options[int.Parse(parity)-1];
        port.Parity = (Parity)Enum.Parse(typeof(Parity), parity, true);
    }
    static void SelectDataBits()
    {
        Console.Clear();
        string dataBits;
        while(true)
        {
            Console.WriteLine("选择数据位数(推荐8)(输入0直接退出):");
            dataBits = Console.ReadLine();
            if (dataBits == "0")
            {
                InitFin = false;
                return;
            }
            else if (int.Parse(dataBits) < 5 || int.Parse(dataBits) > 8)
            {
                Console.WriteLine("输入错误，请重新输入！");
                continue;
            }
            else break;
        }
        port.DataBits = int.Parse(dataBits.ToUpperInvariant());
    }
    static void SelectStopBits()
    {
        Console.Clear();
        string stopBits;
        if (InitFin)
        {
            while (true)
            {
                Console.WriteLine("输入停止位（1或2 输入0退出）：");
                stopBits = Console.ReadLine();
                if (stopBits == "0")
                {
                    InitFin = false;
                    return;
                }
                if (int.Parse(stopBits) != 1 && int.Parse(stopBits) != 2)
                {
                    Console.WriteLine("输入错误，请重新输入！");
                    continue;
                }
                else break;
            }
            if (int.Parse(stopBits) == 1) port.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One", true);
            else port.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "Two", true);
        }
        else return;
    }
    public static void Read()//读人操作
    {
        while (working)
        {
            try
            {
                string message = port.ReadLine();
                Console.WriteLine(message);
            }
            catch (TimeoutException) { }
        }
    }




    public static void Main()
    {
        string name;
        string message;
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Thread readThread = new Thread(Read);
        SelectPort();
        SelectBaundRate();
        SelectPariety();
        SelectDataBits();
        SelectStopBits();
        port.Open();
        working = true;
        readThread.Start();
        Console.Clear();
        Console.Write("端口名称: ");
        name = Console.ReadLine();
        Console.Clear();
        Console.WriteLine("输入 # 结束");
        while (working)
        {
            message = Console.ReadLine();

            if (stringComparer.Equals("#", message))
            {
                working = false;
            }
            else
            {
                port.WriteLine(
                    String.Format("<{0}>: {1}", name, message));
            }
        }

        readThread.Join();
        port.Close();
    }
 }