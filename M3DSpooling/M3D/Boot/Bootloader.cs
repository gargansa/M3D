﻿using M3D.Spooling.Core;
using System;
using System.IO;
using System.Threading;

namespace M3D.Boot
{
  internal class Bootloader
  {
    private ISerialPortIo sp;
    private readonly byte m_yPaddingByte;
    private ChipData mChipData;
    private readonly ushort mEndOfReadableEEPROM;
    private readonly int mBytesPerEEPROMAddress;

    public Bootloader(ISerialPortIo port, byte yPaddingByte, ChipData chipData, ushort endOfReadableEEPROM, int bytesPerEEPROMAddress)
    {
      sp = port;
      m_yPaddingByte = yPaddingByte;
      mChipData = chipData;
      mEndOfReadableEEPROM = endOfReadableEEPROM;
      mBytesPerEEPROMAddress = bytesPerEEPROMAddress;
    }

    public void UploadNewFirmware(byte[] newFirmware, uint test_crc)
    {
      if (!sp.IsOpen)
      {
        throw new Exception("Unable to write program memory.  The serial port is not open.");
      }

      EraseChip();
      SetAddress(0, 0);
      WriteFirmwareToFlash(newFirmware);
      SetAddress(0, 0);
    }

    public bool WriteToEEPROM(ushort startAddress, byte[] bytes)
    {
      SPout(85);
      SPout((byte) ((uint) startAddress >> 8));
      SPout((byte) startAddress);
      var num = (ushort) (bytes.Length / mBytesPerEEPROMAddress);
      SPout((byte) ((uint) num >> 8));
      SPout((byte) num);
      for (var index = 0; index < bytes.Length; ++index)
      {
        SPout(bytes[index]);
      }

      if (ReadBytes(1)[0] != 13)
      {
        throw new Exception("Error writing to EEPROM");
      }

      return true;
    }

    public byte[] ReadAllReadableEEPROM()
    {
      SPout(83);
      byte[] numArray = ReadBytes((mEndOfReadableEEPROM + 1) * mBytesPerEEPROMAddress);
      if (ReadBytes(1)[0] == 13)
      {
        return numArray;
      }

      throw new Exception("Error reading flash");
    }

    public void SPout(byte output)
    {
      sp.Write(new byte[1]{ output }, 0, 1);
    }

    public void SPout(string outputString)
    {
      foreach (byte output in outputString)
      {
        SPout(output);
      }
    }

    public uint GetCRCFromChip(CRC_Type crcType)
    {
      SPout(67);
      if (crcType == CRC_Type.App)
      {
        SPout(65);
      }
      else
      {
        SPout(66);
      }

      var crcBytes = new CRCBytes();
      byte[] numArray = ReadBytes(4);
      crcBytes.Byte1 = numArray[0];
      crcBytes.Byte2 = numArray[1];
      crcBytes.Byte3 = numArray[2];
      crcBytes.Byte4 = numArray[3];
      return crcBytes.Int1;
    }

    private void EraseChip()
    {
      SPout(69);
      if (ReadBytes(1)[0] != 13)
      {
        throw new Exception("Error erasing flash");
      }
    }

    public byte[] ReadBytes(int bytesToRead)
    {
      while (sp.BytesToRead < bytesToRead)
      {
        Thread.Sleep(10);
      }

      byte[] bytes = new byte[bytesToRead];
      sp.Read(bytes, 0, bytesToRead);
      return bytes;
    }

    public void FlushIncomingBytes()
    {
      while (sp.BytesToRead > 0)
      {
        var num = (int)sp.ReadByte();
      }
    }

    private void SetAddress(byte addressByte1, byte addressByte2)
    {
      SPout(65);
      SPout(addressByte1);
      SPout(addressByte2);
      if (ReadBytes(1)[0] != 13)
      {
        throw new Exception("After attempting to set address the micro controller did not reply correctly\r\n");
      }
    }

    private void WriteFirmwareToFlash(byte[] newFirmware)
    {
      var num1 = newFirmware.Length / 2 / mChipData.PageSize;
      if (newFirmware.Length / 2 % mChipData.PageSize != 0)
      {
        ++num1;
      }

      for (var index1 = 0; index1 < num1; ++index1)
      {
        SPout(66);
        SPout((byte) (mChipData.PageSize * 2 >> 8 & byte.MaxValue));
        SPout((byte) (mChipData.PageSize * 2 & byte.MaxValue));
        Thread.Sleep(20);
        byte[] numArray = new byte[2];
        for (var index2 = 0; index2 < mChipData.PageSize * 2; ++index2)
        {
          var num2 = index2 + mChipData.PageSize * index1 * 2;
          SPout(num2 % 2 != 0 ? (num2 - 1 >= newFirmware.Length ? m_yPaddingByte : newFirmware[num2 - 1]) : (num2 + 1 >= newFirmware.Length ? m_yPaddingByte : newFirmware[num2 + 1]));
        }
        if (ReadBytes(1)[0] != 13)
        {
          throw new Exception("Error writing flash memory\n");
        }

        Thread.Sleep(20);
      }
    }

    public bool JumpToApplication()
    {
      try
      {
        SPout(81);
      }
      catch (Exception ex)
      {
        if (!(ex is IOException))
        {
          ErrorLogger.LogErrorMsg("Exception in Bootloader.JumpToApplication " + ex.Message + " " + ex.GetType().ToString(), "Exception");
        }

        return true;
      }
      return true;
    }
  }
}
