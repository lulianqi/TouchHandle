﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TouchHandle.myTool
{
    class myWinio
    {
        private const int KBC_KEY_CMD = 0x64;
        private const int KBC_KEY_DATA = 0x60;
        [DllImport("WinIo32.dll")]
        private static extern bool InitializeWinIo();
        [DllImport("WinIo32.dll")]
        private static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal, byte bSize);
        [DllImport("WinIo32.dll")]
        private static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal, byte bSize);
        [DllImport("WinIo32.dll")]
        private static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize, IntPtr PhysicalMemoryHandle);
        [DllImport("WinIo32.dll")]
        private static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle, byte pbLinAddr);
        [DllImport("WinIo32.dll")]
        private static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);
        [DllImport("WinIo32.dll")]
        private static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);
        [DllImport("WinIo32.dll")]
        private static extern void ShutdownWinIo();
        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(uint Ucode, uint uMapType);


        private myWinio()
        {
            IsInitialize = true;
        }
        public static void Initialize()
       {
           //ShutdownWinIo();
            if (InitializeWinIo())
            {
                KBCWait4IBE();
                IsInitialize = true;
            }
        }
        public static void Shutdown()
        {
            if (IsInitialize)
                ShutdownWinIo();
            IsInitialize = false;
        }

        private static bool IsInitialize { get; set; }

        ///等待键盘缓冲区为空
        private static void KBCWait4IBE()
        {
            int dwVal = 0;
            do
            {
                bool flag = GetPortVal((IntPtr)0x64, out dwVal, 1);
            }
            while ((dwVal & 0x2) > 0);
        }
        /// 模拟键盘标按下
        public static void KeyDown(Keys vKeyCoad)
        {
            if (!IsInitialize) return;

            int btScancode = 0;
            btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)btScancode, 1);
        }
        /// 模拟键盘弹出
        public static void KeyUp(Keys vKeyCoad)
        {
            if (!IsInitialize) return;

            int btScancode = 0;
            btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);
        }
    }
}
