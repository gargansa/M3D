﻿using M3D.Slicer.General;
using System;
using System.Xml.Serialization;

namespace M3D.SlicerConnectionCura.SlicerSettingsItems
{
  public abstract class SlicerSettingsItem
  {
    [XmlIgnore]
    public bool ReverseGroupToggle;

    public SlicerSettingsItem()
    {
      GroupToggleSetting = null;
    }

    [XmlIgnore]
    public SlicerSettingsItem GroupToggleSetting { get; set; }

    [XmlIgnore]
    public bool IsSettingOn
    {
      get
      {
        if (GroupToggleSetting == null)
        {
          return true;
        }

        var userValue = GroupToggleSetting.TranslateToUserValue();
        var flag = !userValue.Equals("false", StringComparison.InvariantCultureIgnoreCase) && !userValue.Equals("0");
        if (ReverseGroupToggle)
        {
          return !flag;
        }

        return flag;
      }
    }

    public SettingReadResult ReadSlicerSetting(string val)
    {
      return !SetFromSlicerValue(val) ? SettingReadResult.Failed : SettingReadResult.OK_Processed;
    }

    protected abstract bool SetFromSlicerValue(string val);

    protected virtual bool SetFromSlicerValueX(string val)
    {
      return SetFromSlicerValue(val);
    }

    protected virtual bool SetFromSlicerValueY(string val)
    {
      return SetFromSlicerValue(val);
    }

    public abstract SettingItemType GetItemType();

    public abstract string TranslateToSlicerValue();

    public virtual string TranslateToUserValue()
    {
      return TranslateToSlicerValue();
    }

    public virtual void ParseUserValue(string value)
    {
      SetFromSlicerValue(value);
    }

    public abstract bool HasWarning { get; }

    public abstract bool HasError { get; }

    public abstract string GetErrorMsg();

    public abstract SlicerSettingsItem Clone();
  }
}
