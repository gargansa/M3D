﻿using M3D.Boot;
using M3D.Spooling.Embedded_Firmware;
using System.Collections.Generic;

namespace M3D.Spooling.Printer_Profiles
{
  internal class ProductProfile
  {
    public readonly uint PID;
    public readonly uint VID;
    public readonly uint m_uFirmwareMaxSizeBytes;
    public readonly byte m_yPaddingByte;
    public readonly ChipData chipData;
    private Dictionary<char, FirmwareDetails> _firmwareList;

    public ProductProfile(uint PID, uint VID, uint uFirmwareMaxSizeBytes, byte yPaddingByte, ChipData chipData, Dictionary<char, FirmwareDetails> firmwareList)
    {
      this.PID = PID;
      this.VID = VID;
      m_uFirmwareMaxSizeBytes = uFirmwareMaxSizeBytes;
      m_yPaddingByte = yPaddingByte;
      _firmwareList = firmwareList;
      this.chipData = chipData;
    }

    public Dictionary<char, FirmwareDetails> FirmwareList
    {
      get
      {
        return _firmwareList;
      }
    }
  }
}
